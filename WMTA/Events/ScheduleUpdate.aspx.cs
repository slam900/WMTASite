using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Events
{
    public partial class ScheduleUpdate : System.Web.UI.Page
    {
        private string auditionSearch = "AuditionData"; //tracks data returned by latest audition search
        private string scheduleData = "ScheduleData";
        private string eventSchedule = "EventSchedule";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                checkPermissions();

                Session[auditionSearch] = null;
                Session[scheduleData] = null;
                Session[eventSchedule] = null;
                loadYearDropdown();
                loadDistrictDropdown();
            }
        }

        /*
         * Pre:
         * Post: If the user is not logged in they will be redirected to the welcome screen
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("../Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("A")))
                    Response.Redirect("../Default.aspx");
            }
        }

        /*
         * Pre:
         * Post: Loads the appropriate years in the dropdown
         */
        private void loadYearDropdown()
        {
            int firstYear = DbInterfaceStudentAudition.GetFirstAuditionYear();

            for (int i = DateTime.Now.Year + 1; i >= firstYear; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        /*
         * Pre:
         * Post:  If the current user is not an administrator, the district
         *        dropdown is filtered to contain only the current
         *        user's district
         */
        private void loadDistrictDropdown()
        {
            User user = (User)Session[Utility.userRole];

            if (!user.permissionLevel.Contains('A')) //if the user is a district admin, add only their district
            {
                //get own district dropdown info
                string districtName = DbInterfaceStudent.GetStudentDistrict(user.districtId);

                //add new items to dropdown
                ddlDistrictSearch.Items.Add(new ListItem(districtName, user.districtId.ToString()));

                ddlDistrictSearch.SelectedIndex = 1;

                //load the audition
                selectAudition();
            }
            else //if the user is an administrator, add all districts
            {
                ddlDistrictSearch.DataSource = DbInterfaceAudition.GetDistricts();

                ddlDistrictSearch.DataTextField = "GeoName";
                ddlDistrictSearch.DataValueField = "GeoId";

                ddlDistrictSearch.DataBind();
            }
        }

        /*
         * Pre: 
         * Post: The entered audition is searched for and selected.  If a matching schedule
         *       slot is found, some summary data is shown on the page
         */
        protected void btnSelectAudition_Click(object sender, EventArgs e)
        {
            int auditionId = -1;

            if (Int32.TryParse(txtAuditionId.Text, out auditionId))
            {
                ScheduleSlot scheduleSlot = DbInterfaceStudentAudition.GetStudentAuditionSchedule(auditionId);

                if (scheduleSlot != null)
                {
                    lblAuditionInformation.Text = "Student: " + scheduleSlot.StudentName + ", Start Time: " + FormatTime(scheduleSlot.StartTime) + ", Judge: " + scheduleSlot.JudgeName;
                    lblSelectedAuditionId.Text = auditionId.ToString();
                }
                else
                {
                    showWarningMessage("No matching audition schedule slots were found.");
                }
            }
            else
            {
                showWarningMessage("Please enter a numeric audition id.");
            }
        }

        /*
         * Pre:
         * Post: Returns a AM/PM string of the input time
         */
        private string FormatTime(TimeSpan time)
        {
            int hour = time.Hours;
            string minutes = time.Minutes.ToString();
            string amPm = "AM";

            if (hour > 12)
            {
                hour = hour - 12;
                amPm = "PM";
            }

            // Add 0 in front of single-digit minutes
            if (minutes.Length == 1)
                minutes = "0" + minutes;

            return hour + ":" + minutes + " " + amPm;
        }

        /*
         * Pre:
         * Post: Load the current times of the selected judge or clear the times if no judge is selected
         */
        protected void ddlAuditionJudges_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTimes.Items.Clear();
            ddlTimes.Items.Add(new ListItem("", ""));

            if (ddlAuditionJudges.SelectedIndex > 0)
            {
                int judgeId = Convert.ToInt32(ddlAuditionJudges.SelectedValue);

                List<Tuple<int, string>> judgeTimes = DbInterfaceScheduling.LoadJudgeTimes(judgeId, Convert.ToInt32(lblAuditionId.Text));

                foreach (Tuple<int, string> time in judgeTimes)
                {
                    ddlTimes.Items.Add(new ListItem(time.Item2, time.Item1.ToString()));
                }
            }
        }

        /*
         * Pre:
         * Post: Switch the selected audition to the chosen time and update the schedule table
         */
        protected void btnMoveAudition_Click(object sender, EventArgs e)
        {
            int auditionId = -1;

            // Make sure an audition id, judge, and time slot are selected
            if (!txtAuditionId.Text.Equals("") && Int32.TryParse(txtAuditionId.Text, out auditionId) && ddlAuditionJudges.SelectedIndex > 0 && ddlTimes.SelectedIndex > 0)
            {
                int auditionOrgId = DbInterfaceStudentAudition.GetAuditionOrgIdByStudentAudition(auditionId);
                Audition audition = DbInterfaceAudition.LoadAuditionData(auditionOrgId); 

                EventSchedule fullSchedule = DbInterfaceScheduling.LoadScheduleData(auditionOrgId);
                fullSchedule.MoveAudition(auditionId, Convert.ToInt32(ddlTimes.SelectedValue), audition);

                // Update auditions in the schedule table
                DataTable schedule = (DataTable)Session[scheduleData];
                for (int i = 0; i < schedule.Rows.Count; i++)
                {
                    string audId = schedule.Rows[i]["Audition Id"].ToString();
                    ScheduleSlot slot = fullSchedule.ScheduleSlots.Where(s => s.AuditionId.ToString().Equals(audId)).FirstOrDefault();

                    // Update start time and judges in table
                    if (slot != null)
                    {
                        schedule.Rows[i]["Start Time"] = slot.StartTime;
                        schedule.Rows[i]["Judge Name"] = slot.JudgeName;
                        schedule.Rows[i]["Judge Id"] = slot.JudgeId;
                        schedule.Rows[i]["Grade"] = slot.Grade;
                        schedule.Rows[i]["Type"] = slot.AuditionType;
                        schedule.Rows[i]["Track"] = slot.AuditionTrack;
                        schedule.Rows[i]["Time Preference"] = slot.TimePreference;
                        schedule.Rows[i]["Student Id"] = slot.StudentId;
                        schedule.Rows[i]["Audition Length"] = slot.Minutes;
                    }
                }

                // Sort the table
                DataView dataView = schedule.DefaultView;
                dataView.Sort = "Judge Name ASC, Start Time ASC"; 
                schedule = dataView.ToTable();

                gvSchedule.DataSource = schedule;
                gvSchedule.DataBind();

                Session[scheduleData] = schedule;
                Session[eventSchedule] = fullSchedule;
            }
            else if (auditionId == -1)
            {
                showWarningMessage("Please select an audition to move.");
            }
            else if (ddlAuditionJudges.SelectedIndex <= 0 || ddlTimes.SelectedIndex <= 0)
            {
                showWarningMessage("Please select a judge and time slot to move the selected audition to.");
            }
        }

        /*
         * Pre:
         * Post: Commit the updated schedule and show a message saying it has been commited
         */
        protected void btnSave_Click(object sender, EventArgs e)
        {
            EventSchedule fullSchedule = (EventSchedule)Session[eventSchedule];

            if (fullSchedule != null && fullSchedule.UpdateSchedule())
            {
                showSuccessMessage("The schedule was successfully updated");
            }
            else
                showErrorMessage("The schedule could not be updated");
        }

        /*
         * Pre:  The AuditionId field must be empty or contain an integer
         * Post: Auditions the match the search criteria are displayed
         */
        protected void btnAuditionSearch_Click(object sender, EventArgs e)
        {
            int districtId = -1, year = -1;

            if (!ddlDistrictSearch.SelectedValue.ToString().Equals(""))
                districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);
            else //if the user did not select a district, but they are not a district admin, only search their district
            {
                User user = (User)Session[Utility.userRole];

                if (!user.permissionLevel.Contains('A'))
                    districtId = user.districtId;
            }

            if (!ddlYear.SelectedValue.ToString().Equals("")) year = Convert.ToInt32(ddlYear.SelectedValue);

            searchAuditions(gvAuditionSearch, districtId, year, auditionSearch);
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post: The input parameters are used to search for existing auditions.  Matchin audition
         *       information is displayed in the input gridview
         * @param gridview is the gridview in which the search results will be displayed
         * @param auditionType is the type of audition being searched for - district, badger keyboard, or badger Vocal/Instrumental
         * @param district is the district id of the audition being searched for
         * @param year is the year of the audition being searched for
         */
        private bool searchAuditions(GridView gridview, int districtId, int year, string session)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceAudition.GetAuditionSearchResults("", "District", districtId, year);

                //If there are results in the table, display them.  Otherwise clear current
                //results and return false
                if (table != null && table.Rows.Count > 0)
                {
                    gridview.DataSource = table;
                    gridview.DataBind();

                    //save the data for quick re-binding upon paging
                    Session[session] = table;
                }
                else
                {
                    showInfoMessage("No events were found matching the search criteria.");

                    clearGridView(gridview);
                    result = false;
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("ScheduleUpdate", "searchAuditions", "gridView: " + gridview + ", districtId: " +
                                 districtId + ", year: " + year + ", session: " + session, "Message: " + e.Message +
                                 "   StackTrace: " + e.StackTrace, -1);
            }

            return result;
        }

        protected void gvAuditionSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAuditionSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        protected void gvAuditionSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvAuditionSearch, e);
        }

        protected void gvAuditionSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectAudition();
        }

        private void selectAudition()
        {
            User user = (User)Session[Utility.userRole];

            if (!user.permissionLevel.Contains('A') && ddlDistrictSearch.SelectedIndex > 0)
            {
                upAuditionSearch.Visible = false;
                upSelectAudition.Visible = true;
                upViewSchedule.Visible = true;
                //pnlButtons.Visible = true;

                int year = DateTime.Today.Year;
                if (DateTime.Today.Month >= 6 && !Utility.reportSuffix.Equals("Test"))
                    year = year + 1;

                ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(year.ToString()));

                int auditionOrgId = DbInterfaceAudition.GetAuditionOrgId(Convert.ToInt32(ddlDistrictSearch.SelectedValue), year);
                lblAudition.Text = ddlDistrictSearch.SelectedItem.Text + " " + ddlYear.Text + " Schedule";
                lblAuditionId.Text = auditionOrgId.ToString();

                loadScheduleInformation(auditionOrgId);
            }
            else if (user.permissionLevel.Contains('A'))
            {
                upAuditionSearch.Visible = false;
                upSelectAudition.Visible = true;
                upViewSchedule.Visible = true;
                //pnlButtons.Visible = true;

                int index = gvAuditionSearch.SelectedIndex;

                if (index >= 0 && index < gvAuditionSearch.Rows.Count)
                {
                    ddlDistrictSearch.SelectedIndex =
                                ddlDistrictSearch.Items.IndexOf(ddlDistrictSearch.Items.FindByText(
                                gvAuditionSearch.Rows[index].Cells[2].Text));
                    ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(
                                            gvAuditionSearch.Rows[index].Cells[3].Text));

                    lblAudition.Text = ddlDistrictSearch.SelectedItem.Text + " " + ddlYear.Text + " Schedule";
                    lblAuditionId.Text = gvAuditionSearch.Rows[index].Cells[1].Text;

                    loadScheduleInformation(Convert.ToInt32(lblAuditionId.Text));
                }
            }
        }

        /*
         * Pre:  audition must exist as the id of an audition in the system
         * Post: The existing data for the audition associated with the auditionId 
         *       is loaded to the page.
         * @param auditionId is the id of the audition being edited
         */
        private void loadScheduleInformation(int auditionId)
        {
            LoadAuditionJudges(DbInterfaceAudition.LoadAuditionData(auditionId));

            //load schedule table
            DataTable schedule = DbInterfaceScheduling.LoadSchedule(auditionId);

            if (schedule != null && schedule.Rows.Count > 0)
            {
                gvSchedule.DataSource = schedule;
                gvSchedule.DataBind();

                Session[scheduleData] = schedule;
            }
            else
            {
                showErrorMessage("Error: The schedule could not be loaded.  Please make sure it has been created and saved.");
                Session[scheduleData] = null;
            }
        }

        private void LoadAuditionJudges(Audition audition)
        {
            ddlAuditionJudges.Items.Clear();
            ddlAuditionJudges.Items.Add(new ListItem("", ""));

            try
            {
                List<Judge> judges = audition.GetEventJudges(true);

                //Load each judge to the dropdown
                foreach (Judge judge in judges)
                {
                    ddlAuditionJudges.Items.Add(new ListItem(judge.id + ": " + judge.lastName + ", " + judge.firstName, judge.id.ToString()));
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the event's judges.");
                Utility.LogError("ScheduleUpdate", "LoadAuditionJudges", "auditionId: " + audition.auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        protected void gvSchedule_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSchedule.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvSchedule, e);
        }

        /*
         * Pre:   The tables must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[auditionSearch];
                gvAuditionSearch.DataSource = data;
                gvAuditionSearch.DataBind();

                data = (DataTable)Session[scheduleData];
                gvSchedule.DataSource = data;
                gvSchedule.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("ScheduleUpdate", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        protected void btnClearAuditionSearch_Click(object sender, EventArgs e)
        {
            clearAuditionSearch();
        }

        /*
         * Pre:
         * Post: The Audition Search section is cleared
         */
        private void clearAuditionSearch()
        {
            ddlDistrictSearch.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            gvAuditionSearch.DataSource = null;
            gvAuditionSearch.DataBind();
        }

        /*
         * Pre:  The input must be a gridview that exists on the current page
         * Post: The background of the header row is set
         * @param gv is the gridView that will have its header row color changed
         * @param e are the event args for the event fired by the row being bound to data
         */
        private void setHeaderRowColor(GridView gv, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in gv.HeaderRow.Cells)
                {
                    cell.BackColor = Color.Black;
                    cell.ForeColor = Color.White;
                }
            }
        }

        /*
         * Pre: The GridView gv must exist on the current form
         * Post:  The data binding of the GridView is cleared, causing the table to be cleared
         * @param gv is the GridView to be cleared
         */
        private void clearGridView(GridView gv)
        {
            gv.DataSource = null;
            gv.DataBind();
        }

        #region Messages

        /*
         * Pre:
         * Post: Displays the input error message in the top-left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showErrorMessage(string message)
        {
            lblErrorMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowError", "showMainError()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input warning message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showWarningMessage(string message)
        {
            lblWarningMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowWarning", "showWarningMessage()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input success message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showSuccessMessage(string message)
        {
            lblSuccessMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowSuccess", "showSuccessMessage()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input informational message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showInfoMessage(string message)
        {
            lblInfoMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowInfo", "showInfoMessage()", true);
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Schedule", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            showErrorMessage("Error: An error occurred.");

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
        #endregion Messages

    }
}
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
    public partial class CreateBadgerAudition : System.Web.UI.Page
    {
        private Utility.Action action;
        /* session variables */
        private string auditionSearch = "AuditionData"; //tracks data returned by latest audition search

        protected void Page_Load(object sender, EventArgs e)
        {
            initializeAction();
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[auditionSearch] = null;

                loadYearDropdown();
                initializePage();
            }
        }

        /*
         * Pre:
         * Post: The action being performed is initialized.  Defaulted to Add
         */
        private void initializeAction()
        {
            string actionIndicator = Request.QueryString["action"];
            if (actionIndicator == null || actionIndicator.Equals(""))
            {
                action = Utility.Action.Add;
            }
            else
            {
                action = (Utility.Action)Convert.ToInt32(actionIndicator);
            }
        }

        /*
         * Pre:
         * Post: Initialize the page for adding, editing, or deleting based on user selection
         */
        protected void initializePage()
        {
            //initialize page based on action
            if (action == Utility.Action.Add)
            {
                upAuditionSearch.Visible = false;
                ddlDistrict.Enabled = true;
            }
            else if (action == Utility.Action.Edit)
            {
                upAuditionSearch.Visible = true;
                pnlMain.Visible = false;
                upButtons.Visible = false;
                legend.InnerText = "Edit Badger Event";
                ddlDistrict.Enabled = false;
            }
            else if (action == Utility.Action.Delete)
            {
                upAuditionSearch.Visible = true;
                pnlMain.Visible = false;
                upButtons.Visible = false;
                legend.InnerText = "Delete Badger Event";
                //disableControls();

                btnSubmit.Attributes.Add("onclick", "return confirm('Are you sure that you wish to permanently delete this event and all associated data?');");
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
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("S") || user.permissionLevel.Contains("A")))
                    Response.Redirect("/Default.aspx");
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
         * Pre:  The AuditionId field must be empty or contain an integer
         * Post: Auditions the match the search criteria are displayed
         */
        protected void btnAuditionSearch_Click(object sender, EventArgs e)
        {
            int districtId = -1, year = -1;

            if (!ddlDistrictSearch.SelectedValue.ToString().Equals("")) districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);
            if (!ddlYear.SelectedValue.ToString().Equals("")) year = Convert.ToInt32(ddlYear.SelectedValue);

            searchAuditions(gvAuditionSearch, districtId, year, auditionSearch);
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post: The input parameters are used to search for existing auditions.  Matchin audition
         *       information is displayed in the input gridview
         * @param gridview is the gridview in which the search results will be displayed
         * @param district is the district id of the audition being searched for
         * @param year is the year of the audition being searched for
         */
        private bool searchAuditions(GridView gridview, int districtId, int year, string session)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceAudition.GetAuditionSearchResults("", "State", districtId, year);

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
                    clearGridView(gridview);
                    result = false;
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("Create Badger Audition", "searchAuditions", "gridView: " + gridview.ID + ", districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Pre:
         * Post: If all entered data is valid, a new audition is created in the database
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Audition audition;

            //make sure the entered data is valid before adding or updating
            if (dataIsValid())
            {
                int districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
                int numJudges = Convert.ToInt32(txtNumJudges.Text);
                string chairperson = ddlChairPerson.SelectedValue;
                DateTime auditionDate, freezeDate;
                DateTime.TryParse(txtDate.Value, out auditionDate);
                DateTime.TryParse(txtFreezeDate.Value, out freezeDate);
                TimeSpan startTime, endTime;

                //get start time
                string tempTime;
                if (ddlAmPmStart.SelectedValue.Equals("AM"))
                {
                    tempTime = ddlHourStart.SelectedValue + ":" + ddlMinutesStart.SelectedValue + ":00";
                }
                else if (ddlHourStart.SelectedValue.ToString().Equals("12"))
                {
                    tempTime = ddlHourStart.SelectedValue + ":" + ddlMinutesStart.SelectedValue + ":00";
                }
                else
                {
                    tempTime = (Convert.ToInt32(ddlHourStart.SelectedValue) + 12).ToString() + ":" + ddlMinutesStart.SelectedValue + ":00";
                }
                TimeSpan.TryParse(tempTime, out startTime);

                //get end time
                if (ddlAmPmEnd.SelectedValue.Equals("AM"))
                {
                    tempTime = ddlHourEnd.SelectedValue + ":" + ddlMinutesEnd.SelectedValue + ":00";
                }
                else if (ddlHourEnd.SelectedValue.ToString().Equals("12"))
                {
                    tempTime = ddlHourEnd.SelectedValue + ":" + ddlMinutesEnd.SelectedValue + ":00";
                }
                else
                {
                    tempTime = (Convert.ToInt32(ddlHourEnd.SelectedValue) + 12).ToString() + ":" + ddlMinutesEnd.SelectedValue + ":00";
                }
                TimeSpan.TryParse(tempTime, out endTime);

                bool duetsAllowed = false;
                if (ddlDuets.SelectedValue.Equals("Yes"))
                    duetsAllowed = true;

                //if a new audition is being created and the same audition doesn't already exist, add it
                if (action == Utility.Action.Add && !DbInterfaceAudition.AuditionExists(districtId, auditionDate.Year))
                {
                    audition = new Audition(districtId, numJudges, txtVenue.Text, chairperson, "",
                                            auditionDate, freezeDate, startTime, endTime, duetsAllowed);

                    //if the audition was successfully created, display a success message
                    if (audition.auditionId != -1)
                    {
                        displaySuccessMessage();
                    }
                    else
                    {
                        showErrorMessage("Error: The audition could not be created. Please make sure all entered data is valid.");
                    }
                }
                //update the information of an existing audition
                else if (action == Utility.Action.Edit)
                {
                    int auditionId = Convert.ToInt32(txtIdHidden.Text);

                    audition = new Audition(auditionId, districtId, numJudges, txtVenue.Text,
                                            chairperson, "", auditionDate, freezeDate, startTime,
                                            endTime, duetsAllowed);

                    if (audition.auditionId != -1 && audition.updateInDatabase())
                    {
                        displaySuccessMessage();
                    }
                    else
                    {
                        showErrorMessage("Error: The audition could not be updated.");
                    }
                }
                //display an error message if the audition already exists
                else
                {
                    showWarningMessage("An audition for this venue already exists.");
                }
            }
        }

        /*
         * Pre:
         * Post: Determines whether all required fields contain data and all
         *       data is in a valid format
         * @returns true if all required fields contain data and all
         *          data is in a valid format
         */
        private bool dataIsValid()
        {
            bool result = true;
            DateTime dateVal, auditionDate;

            //make sure the number of judges is a positive integer
            int num;
            bool isNum = Int32.TryParse(txtNumJudges.Text, out num);
            if (!isNum || num < 0)
            {
                showWarningMessage("The number of judges must be a positive integer.");
                result = false;
            }

            //make sure freeze date is before audition date
            DateTime date;
            if (!DateTime.TryParse(txtDate.Value, out date))
            {
                showWarningMessage("The Date must be in the form mm/dd/yyyy.");
                result = false;
            }
            else if (!DateTime.TryParse(txtFreezeDate.Value, out date))
            {
                showWarningMessage("The Freeze Date must be in the form mm/dd/yyyy.");
                result = false;
            }
            else if (DateTime.Parse(txtFreezeDate.Value) >= DateTime.Parse(txtDate.Value))
            {
                showWarningMessage("The Freeze Date must be before the Audition Date.");
                result = false;
            }

            //make sure end time is after start time
            if (!endAfterStart())
            {
                showWarningMessage("The Start Time must be before the End Time.");
                result = false;
            }

            //if duets are enabled, make sure there isn't already a state audition for the upcoming year
            //that already has duets enabled
            if (ddlDuets.SelectedValue.Equals("Yes") && !ddlDistrict.SelectedItem.Text.Contains("Non"))
            {
                if (DbInterfaceAudition.StateDuetSiteExists(DateTime.Parse(txtDate.Value).Year, Convert.ToInt32(ddlDistrict.SelectedValue)))
                {
                    showWarningMessage("There is already a Badger Keyboard audition with duets enabled.");
                    result = false;
                }
            }

            return result;
        }

        /*
         * Pre:
         * Post: Determines whether the end time is after the start time.  This function assumes
         *       that the start and end times must be at least an hour apart
         * @returns true if the end time is after the start time by at least 1 hour and false otherwise
         */
        private bool endAfterStart()
        {
            bool result = true;

            //if the start time is not in the morning or the end time 
            //is not in the afternoon, make sure the times are valid
            if (!(ddlAmPmEnd.SelectedValue.Equals("PM") && ddlAmPmStart.SelectedValue.Equals("AM")))
            {
                //if the start time is in the afternoon and end time is in the morning return false
                if (ddlAmPmEnd.SelectedValue.Equals("AM") && ddlAmPmStart.SelectedValue.Equals("PM"))
                {
                    result = false;
                }
                //if the AM/PM values are the same and the start hour is greater than the end hour return false
                else if (Convert.ToInt32(ddlHourEnd.SelectedValue) < Convert.ToInt32(ddlHourStart.SelectedValue))
                {
                    result = false;
                }
            }

            return result;
        }

        /*
         * Pre:
         * Post: Show a success message to the user and clear the page
         */
        private void displaySuccessMessage()
        {
            clearPage();

            if (action != Utility.Action.Add)
            {
                pnlMain.Visible = false;
                pnlButtons.Visible = false;
                upAuditionSearch.Visible = true;
            }

            if (action == Utility.Action.Add)
                showSuccessMessage("The audition was successfully created.");
            else if (action == Utility.Action.Edit)
                showSuccessMessage("The audition was successfully updated.");
        }

        /*
         * Pre:   
         * Post:  The page of gvAuditionSearch is changed
         */
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
            clearAllExceptSearch();

            int index = gvAuditionSearch.SelectedIndex;

            if (index >= 0 && index < gvAuditionSearch.Rows.Count)
            {
                ddlDistrictSearch.SelectedIndex =
                            ddlDistrictSearch.Items.IndexOf(ddlDistrictSearch.Items.FindByText(
                            gvAuditionSearch.Rows[index].Cells[2].Text));
                ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(
                                        gvAuditionSearch.Rows[index].Cells[3].Text));
                loadAuditionData(Convert.ToInt32(gvAuditionSearch.Rows[index].Cells[1].Text));

                pnlMain.Visible = true;
                upButtons.Visible = true;
                upAuditionSearch.Visible = false;
            }
        }

        /*
         * Pre:  audition must exist as the id of an audition in the system
         * Post: The existing data for the audition associated with the auditionId 
         *       is loaded to the page.
         * @param auditionId is the id of the audition being edited
         */
        private void loadAuditionData(int auditionId)
        {
            Audition audition = null;

            try
            {
                audition = DbInterfaceAudition.LoadAuditionData(auditionId);

                //load data to page
                if (audition != null)
                {
                    txtIdHidden.Text = audition.auditionId.ToString();
                    ddlDistrict.DataBind();
                    ddlDistrict.SelectedIndex =
                                          ddlDistrict.Items.IndexOf(ddlDistrict.Items.FindByValue(
                                          audition.districtId.ToString()));
                    txtVenue.Text = audition.venue;
                    txtNumJudges.Text = audition.numJudges.ToString();
                    ddlChairPerson.DataBind();
                    ddlChairPerson.SelectedIndex = ddlChairPerson.Items.IndexOf(
                                                ddlChairPerson.Items.FindByValue(audition.chairpersonId));

                    //set start time
                    if (audition.startTime.Hours < 10)
                    {
                        ddlHourStart.SelectedValue = "0" + audition.startTime.Hours.ToString();
                        ddlAmPmStart.SelectedValue = "AM";
                    }
                    else if (audition.startTime.Hours < 12)
                    {
                        ddlHourStart.SelectedValue = audition.startTime.Hours.ToString();
                        ddlAmPmStart.SelectedValue = "AM";
                    }
                    else if (audition.startTime.Hours > 12)
                    {
                        int hours = audition.startTime.Hours - 12;

                        if (hours < 10)
                        {
                            ddlHourStart.SelectedValue = "0" + hours.ToString();
                        }
                        else
                        {
                            ddlHourStart.SelectedValue = hours.ToString();
                        }

                        ddlAmPmStart.SelectedValue = "PM";
                    }
                    else
                    {
                        ddlHourStart.SelectedValue = "12";
                        ddlAmPmStart.SelectedValue = "PM";
                    }

                    if (audition.startTime.Minutes > 0)
                        ddlMinutesStart.SelectedValue = audition.startTime.Minutes.ToString();
                    else
                        ddlMinutesStart.SelectedValue = "00";


                    //set end time
                    if (audition.endTime.Hours < 10)
                    {
                        ddlHourEnd.SelectedValue = "0" + audition.endTime.Hours.ToString();
                        ddlAmPmEnd.SelectedValue = "AM";
                    }
                    else if (audition.endTime.Hours < 12)
                    {
                        ddlHourEnd.SelectedValue = audition.endTime.Hours.ToString();
                        ddlAmPmEnd.SelectedValue = "AM";
                    }
                    else if (audition.endTime.Hours > 12)
                    {
                        int hours = audition.endTime.Hours - 12;

                        if (hours < 10)
                        {
                            ddlHourEnd.SelectedValue = "0" + hours.ToString();
                        }
                        else
                        {
                            ddlHourEnd.SelectedValue = hours.ToString();
                        }

                        ddlAmPmEnd.SelectedValue = "PM";
                    }
                    else
                    {
                        ddlHourEnd.SelectedValue = "12";
                        ddlAmPmEnd.SelectedValue = "PM";
                    }

                    if (audition.endTime.Minutes > 0)
                        ddlMinutesEnd.SelectedValue = audition.endTime.Minutes.ToString();
                    else
                        ddlMinutesEnd.SelectedValue = "00";

                    if (audition.duetsAllowed)
                        ddlDuets.SelectedValue = "Yes";
                    else
                        ddlDuets.SelectedValue = "No";

                    //dates must be in form of YYYY-MM-DD
                    string month = audition.auditionDate.Month.ToString();
                    string day = audition.auditionDate.Day.ToString();

                    if (month.Length == 1) month = "0" + month;
                    if (day.Length == 1) day = "0" + day;

                    txtDate.Value = audition.auditionDate.ToShortDateString();

                    month = audition.freezeDate.Month.ToString();
                    day = audition.freezeDate.Day.ToString();

                    if (month.Length == 1) month = "0" + month;
                    if (day.Length == 1) day = "0" + day;

                    txtFreezeDate.Value = audition.freezeDate.ToShortDateString();
                }
                else
                {
                    showErrorMessage("Error: The audition information could not be loaded.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the audition data.");

                Utility.LogError("Create Badger Audition", "loadAuditionData", "auditionId: " + auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:   The StudentData table must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[auditionSearch];
                gvAuditionSearch.DataSource = data;
                gvAuditionSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Create Badger Audition", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
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

        /*
         * Pre:
         * Post: The controls on the page are cleared
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: The Audition Search section is cleared
         */
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
         * Pre:
         * Post: The controls on the page are cleared
         */
        private void clearPage()
        {
            clearAuditionSearch();

            if (action != Utility.Action.Add)
                pnlMain.Visible = false;

            ddlDistrict.SelectedIndex = 0;
            txtVenue.Text = "";
            txtNumJudges.Text = "";
            ddlChairPerson.SelectedIndex = 0;
            txtDate.Value = "";
            ddlHourStart.SelectedValue = "08";
            ddlMinutesStart.SelectedValue = "00";
            ddlAmPmStart.SelectedValue = "AM";
            ddlHourEnd.SelectedValue = "04";
            ddlMinutesEnd.SelectedValue = "00";
            ddlAmPmEnd.SelectedValue = "PM";
            txtFreezeDate.Value = "";

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RefreshDatepickers", "refreshDatePickers()", true);
        }

        /*
         * Pre:
         * Post: All controls other than the ones contained in the audition
         *       search section are cleared
         */
        private void clearAllExceptSearch()
        {
            ddlDistrict.SelectedIndex = 0;
            txtVenue.Text = "";
            txtNumJudges.Text = "";
            ddlChairPerson.SelectedIndex = 0;
            txtDate.Value = "";
            ddlHourStart.SelectedValue = "08";
            ddlMinutesStart.SelectedValue = "00";
            ddlAmPmStart.SelectedValue = "AM";
            ddlHourEnd.SelectedValue = "04";
            ddlMinutesEnd.SelectedValue = "00";
            ddlAmPmEnd.SelectedValue = "PM";
            txtFreezeDate.Value = "";

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RefreshDatepickers", "refreshDatePickers()", true);
        }

        /*
         * Pre:
         * Post:  If the a Non-Keyboard site is selected, set the Duets Allowed dropdown to
         *        yes and disable it.  Otherwise set it to no and enable it
         */
        protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDistrict.SelectedItem.Text.Contains("Non"))
            {
                ddlDuets.SelectedValue = "Yes";
                ddlDuets.Enabled = false;
            }
            else
            {
                ddlDuets.Enabled = true;
                ddlDuets.SelectedValue = "No";
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RefreshDatepickers", "refreshDatePickers()", true);
        }

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
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Create Badger Audition", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class CreateDistrictAudition : System.Web.UI.Page
    {
        /* session variables */
        private string creatingNew = "CreatingNew"; //tracks whether an audition is being created or edited
        private string completed = "Completed";     //tracks whether the audition has been submitted
        private string auditionSearch = "AuditionData"; //tracks data returned by latest audition search

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[creatingNew] = null;
                Session[completed] = null;
                Session[auditionSearch] = null;
                loadYearDropdown();
                loadDistrictDropdown();
            }

            if (Page.IsPostBack && Session[completed] != null)
            {
                pnlSuccess.Visible = true;
                pnlMain.Visible = false;
                Session[completed] = null;
            }
            else if (Page.IsPostBack)
            {
                pnlSuccess.Visible = false;
                pnlButtons.Visible = true;
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

                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("A")))
                    Response.Redirect("/WelcomeScreen.aspx");
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
         *        dropdowns are filtered to containing only the current
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
                ddlDistrict.Items.Add(new ListItem(districtName, user.districtId.ToString()));
                ddlDistrictSearch.Items.Add(new ListItem(districtName, user.districtId.ToString()));
            }
            else //if the user is an administrator, add all districts
            {
                ddlDistrict.DataSource = DbInterfaceAudition.GetDistricts();
                ddlDistrictSearch.DataSource = DbInterfaceAudition.GetDistricts();

                ddlDistrict.DataTextField = "GeoName";
                ddlDistrict.DataValueField = "GeoId";
                ddlDistrictSearch.DataTextField = "GeoName";
                ddlDistrictSearch.DataValueField = "GeoId";

                ddlDistrict.DataBind();
                ddlDistrictSearch.DataBind();
            }
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
         * @param auditionType is the type of audition being searched for - district, badger keyboard, or badger non-keyboard
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
                    clearGridView(gridview);
                    result = false;
                }
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "An error occurred during the search";
                lblErrorMsg.Visible = true;

                Utility.LogError("Create District Audition", "searchAuditions", "gridView: " + gridview + ", districtId: " +
                                 districtId + ", year: " + year + ", session: " + session, "Message: " + e.Message +
                                 "   StackTrace: " + e.StackTrace, -1);
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
            clearErrors();

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
                string tempTime = txtStartTime.Value;
                tempTime = tempTime.Substring(0, tempTime.Length - 2) + ":00 " + tempTime.Substring(tempTime.Length - 2, 2);
                if (tempTime.Length == 10) tempTime = "0" + tempTime;
                if (tempTime.Substring(tempTime.Length - 2, 2).ToUpper().Equals("PM"))
                    tempTime = Utility.ConvertToPm(tempTime);
                tempTime = tempTime.Substring(0, tempTime.Length - 2);
                TimeSpan.TryParse(tempTime, out startTime);

                //get end time
                tempTime = txtEndTime.Value;
                tempTime = tempTime.Substring(0, tempTime.Length - 2) + ":00 " + tempTime.Substring(tempTime.Length - 2, 2);
                if (tempTime.Substring(tempTime.Length - 2, 2).ToUpper().Equals("PM"))
                    tempTime = Utility.ConvertToPm(tempTime);
                tempTime = tempTime.Substring(0, tempTime.Length - 2);
                TimeSpan.TryParse(tempTime, out endTime);

                //if a new audition is being created and the same audition doesn't already exist, add it
                if ((bool)Session[creatingNew] && !DbInterfaceAudition.AuditionExists(districtId, auditionDate.Year))
                {
                    audition = new Audition(districtId, numJudges, txtVenue.Text, chairperson,
                                            ddlTheorySeries.Text, auditionDate, freezeDate,
                                            startTime, endTime, true);

                    //if the audition was successfully created, display a success message
                    if (audition.auditionId != -1)
                    {
                        displaySuccessMessageAndOptions();
                        Session[completed] = true;
                    }
                    else
                    {
                        lblErrorMsg.Text = "The audition could not be created. Please make sure " +
                                           "all entered data is valid.";
                        lblErrorMsg.Visible = true;
                    }
                }
                //update the information of an existing audition
                else if (!(bool)Session[creatingNew])
                {
                    int auditionId = Convert.ToInt32(txtIdHidden.Text);

                    audition = new Audition(auditionId, districtId, numJudges, txtVenue.Text,
                                            chairperson, ddlTheorySeries.SelectedValue, auditionDate,
                                            freezeDate, startTime, endTime, true);

                    if (audition.auditionId != -1 && audition.updateInDatabase())
                    {
                        displaySuccessMessageAndOptions();
                        Session[completed] = true;
                    }
                    else
                    {
                        lblErrorMsg.Text = "An error occurred while updating the audition.";
                        lblErrorMsg.Visible = true;
                    }
                }
                //display an error message if the audition already exists
                else
                {
                    lblErrorMsg.Text = "An audition for this venue already exists";
                    lblErrorMsg.Visible = true;
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
            DateTime dateVal;

            //make sure a district is chosen
            if (ddlDistrict.SelectedValue.ToString().Equals(""))
            {
                lblDistrictError.Visible = true;
                result = false;
            }

            //make sure a venue is entered
            if (txtVenue.Text.Equals(""))
            {
                lblVenueError.Visible = true;
                result = false;
            }

            //make sure the number of judges is a positive integer
            if (txtNumJudges.Text.Equals(""))
            {
                lblNumJudgesError.Visible = true;
                result = false;
            }
            else
            {
                int num;
                bool isNum;

                isNum = Int32.TryParse(txtNumJudges.Text, out num);

                if (!isNum || num < 0)
                {
                    lblNumJudgesError.Visible = true;
                    result = false;
                }
            }

            //make sure a chairperson is selected
            if (ddlChairPerson.SelectedIndex <= 0)
            {
                lblChairpersonError.Visible = true;
                result = false;
            }

            //make sure a theory test series is selected
            if (ddlTheorySeries.SelectedIndex <= 0)
            {
                lblTheoryTestError.Visible = true;
                result = false;
            }

            //make sure a valid date is entered
            if (!DateTime.TryParse(txtDate.Value, out dateVal))
            {
                lblDateError.Visible = true;
                result = false;
            }
            else
                lblDateError.Visible = false;

            //make sure valid freeze date is entered
            if (!DateTime.TryParse(txtFreezeDate.Value, out dateVal))
            {
                lblFreezeDateError.Visible = true;
                result = false;
            }
            else
                lblFreezeDateError.Visible = false;

            //make sure freeze date is before audition date
            if (!lblDateError.Visible && !lblFreezeDateError.Visible &&
                DateTime.Parse(txtFreezeDate.Value) >= DateTime.Parse(txtDate.Value))
            {
                lblFreezeDateError2.Visible = true;
                lblFreezeDateError.Visible = true;
                result = false;
            }
            else
            {
                lblFreezeDateError2.Visible = false;
                lblFreezeDateError2.Visible = false;
            }


            //make sure a valid start time is entered
            if (!DateTime.TryParse(txtStartTime.Value, out dateVal))
            {
                lblStartTimeError.Visible = true;
                result = false;
            }

            //make sure a valid end time is entered
            if (!DateTime.TryParse(txtEndTime.Value, out dateVal))
            {
                lblEndTimeError.Visible = true;
                result = false;
            }

            //make sure end time is after start time
            if (!lblStartTimeError.Visible && !lblEndTimeError.Visible &&
                DateTime.Parse(txtStartTime.Value) >= DateTime.Parse(txtEndTime.Value))
            {
                lblTimeError.Visible = true;
                result = false;
            }

            return result;
        }

        /*
         * Pre:
         * Post: All controls are hidden, the user is told that the audition was created,
         *       and are given the options to create another new audition or go back to
         *       the menu/welcome page
         */
        private void displaySuccessMessageAndOptions()
        {
            clearPage();

            lblSuccess.Visible = true;
            pnlSuccess.Visible = true;
            pnlMain.Visible = false;
            pnlButtons.Visible = false;
            pnlAuditionSearch.Visible = false;

            if ((bool)Session[creatingNew])
                lblSuccess.Text = "The audition was successfully created";
            else
                lblSuccess.Text = "The audition was successfully updated";
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
            pnlMain.Visible = true;
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
                    ddlDistrict.SelectedIndex =
                                          ddlDistrict.Items.IndexOf(ddlDistrict.Items.FindByValue(
                                          audition.districtId.ToString()));
                    txtVenue.Text = audition.venue;
                    txtNumJudges.Text = audition.numJudges.ToString();
                    ddlChairPerson.SelectedIndex = ddlChairPerson.Items.IndexOf(
                                                ddlChairPerson.Items.FindByValue(audition.chairpersonId));
                    ddlTheorySeries.SelectedIndex = ddlTheorySeries.Items.IndexOf(
                                                ddlTheorySeries.Items.FindByValue(audition.theoryTestSeries));
                    txtStartTime.Value = audition.startTime.ToString();
                    txtEndTime.Value = audition.endTime.ToString();

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
                    lblErrorMsg.Text = "The audition information could not be loaded";
                    lblErrorMsg.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "An error occurred while loading the audition data";
                lblErrorMsg.Visible = true;

                Utility.LogError("Create District Audition", "loadAuditionData", "auditionId: " + auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
                Utility.LogError("Create District Audition", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
                    cell.BackColor = Color.FromArgb(204, 204, 255);
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
            clearErrors();
            clearAuditionSearch();

            if (Session[creatingNew] != null && !(bool)Session[creatingNew])
                pnlMain.Visible = false;

            ddlDistrict.SelectedIndex = 0;
            txtVenue.Text = "";
            txtNumJudges.Text = "";
            ddlChairPerson.SelectedIndex = 0;
            ddlTheorySeries.SelectedIndex = 0;
            txtDate.Value = "";
            txtStartTime.Value = "";
            txtEndTime.Value = "";
            txtFreezeDate.Value = "";
        }

        /*
         * Pre:
         * Post: All controls other than the ones contained in the audition
         *       search section are cleared
         */
        private void clearAllExceptSearch()
        {
            clearErrors();

            ddlDistrict.SelectedIndex = 0;
            txtVenue.Text = "";
            txtNumJudges.Text = "";
            ddlChairPerson.SelectedIndex = 0;
            ddlTheorySeries.SelectedIndex = 0;
            txtDate.Value = "";
            txtStartTime.Value = "";
            txtEndTime.Value = "";
            txtFreezeDate.Value = "";
        }

        /*
         * Pre:
         * Post: The errors on the page are cleared
         */
        private void clearErrors()
        {
            lblDistrictError.Visible = false;
            lblVenueError.Visible = false;
            lblNumJudgesError.Visible = false;
            lblChairpersonError.Visible = false;
            lblTheoryTestError.Visible = false;
            lblDateError.Visible = false;
            lblTimeError.Visible = false;
            lblStartTimeError.Visible = false;
            lblEndTimeError.Visible = false;
            lblFreezeDateError.Visible = false;
            lblFreezeDateError2.Visible = false;
            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
        }

        /*
         * Pre:
         * Post: Initialize the page for adding or editing based on user selection
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            clearPage();

            pnlSuccess.Visible = false;
            pnlButtons.Visible = true;
            pnlMain.Visible = true;

            if (ddlUserOptions.SelectedValue.Equals("Create New"))
            {
                Session[creatingNew] = true;
                pnlAuditionSearch.Visible = false;
                ddlDistrict.Enabled = true;
            }
            else if (ddlUserOptions.SelectedValue.Equals("Edit Existing"))
            {
                Session[creatingNew] = false;
                pnlAuditionSearch.Visible = true;
                pnlMain.Visible = true;
                ddlDistrict.Enabled = false;
            }
        }

        /*
         * Pre:
         * Post: The user is brought back to the main screen
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * The following methods clear error messages associated with the
         * controls being changed
         */
        protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDistrictError.Visible = false;
        }
        protected void txtVenue_TextChanged(object sender, EventArgs e)
        {
            lblVenueError.Visible = false;
        }
        protected void txtNumJudges_TextChanged(object sender, EventArgs e)
        {
            lblNumJudgesError.Visible = false;
        }
        protected void ddlChairPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblChairpersonError.Visible = false;
        }
        protected void ddlTheorySeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTheoryTestError.Visible = false;
        }
        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            //lblDateError.Visible = false;
        }
        protected void txtStartTime_TextChanged(object sender, EventArgs e)
        {
            lblStartTimeError.Visible = false;
            lblTimeError.Visible = false;
        }
        protected void txtEndTime_TextChanged(object sender, EventArgs e)
        {
            lblEndTimeError.Visible = false;
            lblTimeError.Visible = false;
        }
        protected void txtFreezeDate_TextChanged(object sender, EventArgs e)
        {
            lblFreezeDateError.Visible = false;
            lblFreezeDateError2.Visible = false;
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Create District Audition", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            lblErrorMsg.Text = "An error occurred";
            lblErrorMsg.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
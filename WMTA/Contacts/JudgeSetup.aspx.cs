using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Contacts
{
    public partial class JudgeSetup : System.Web.UI.Page
    {
        private Contact contact;
        private Judge judge;
        //Session variables
        private string contactSearch = "ContactData";
        private string contactVar = "Contact";
        private string judgeVar = "Judge";

        protected void Page_Load(object sender, EventArgs e)
        {
            //clear session variables and set state to WI
            if (!Page.IsPostBack)
            {
                checkPermissions();
                loadDistrictDropdown();
                loadYearDropdown();

                Session[contactSearch] = null;
                Session[contactVar] = null;
                Session[judgeVar] = null;
            }

            //if a contact object has been instantiated, reload
            if (Page.IsPostBack && Session[contactVar] != null)
                contact = (Contact)Session[contactVar];
            //if a judge object has been instantiated, reload
            if (Page.IsPostBack && Session[judgeVar] != null)
                judge = (Judge)Session[judgeVar];
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

            if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S'))) //if the user is a district admin, add only their district
            {
                //get own district dropdown info
                string districtName = DbInterfaceStudent.GetStudentDistrict(user.districtId);

                //add new item to dropdown and select it
                ddlDistrict.Items.Add(new ListItem(districtName, user.districtId.ToString()));
                ddlDistrict.SelectedIndex = 1;
            }
            else //if the user is an administrator, add all districts
            {
                ddlDistrict.DataSource = DbInterfaceAudition.GetDistricts();

                ddlDistrict.DataTextField = "GeoName";
                ddlDistrict.DataValueField = "GeoId";

                ddlDistrict.DataBind();
            }
        }

        /*
         * Pre:   The ContactId field must be empty or contain an integer
         * Post:  Judges matching the search criteria are displayed. The error message is also reset.
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtContactId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                //if the search does not return any result, display a message saying so
                if (!searchJudgeContacts(gvSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, contactSearch))
                {
                    showInfoMessage("The search did not return any results.");
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvSearch);
                showWarningMessage("A Contact Id must be numeric.");
            }
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing contacts.  Matching contact 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @returns true if results were found and false otherwise
         */
        private bool searchJudgeContacts(GridView gridView, string id, string firstName, string lastName, string session)
        {
            DataTable table;
            bool result = true;

            try
            {
                if (!id.Equals(""))
                    table = DbInterfaceJudge.GetJudgeSearchResults(id, firstName, lastName);
                else
                    table = DbInterfaceJudge.GetJudgeSearchResults("", firstName, lastName);

                //If there are results in the table, display them.  Otherwise clear current
                //results and return false
                if (table != null && table.Rows.Count > 0)
                {
                    gridView.DataSource = table;
                    gridView.DataBind();
                    gridView.HeaderRow.BackColor = Color.Black;

                    //save the data for quick re-binding upon paging
                    Session[session] = table;
                }
                else if (table != null && table.Rows.Count == 0)
                {
                    clearGridView(gridView);
                    result = false;
                }
                else if (table == null)
                {
                    showErrorMessage("Error: An error occurred during the search. Please make sure all entered data is valid.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search. Please make sure all entered data is valid.");

                //log issue in database
                Utility.LogError("Judge Setup", "searchJudgeContacts", gridView.ID + ", " + id + ", " + firstName + ", " + lastName + ", " + session, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        protected void gvSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvSearch.SelectedIndex;
            clearData();

            if (index >= 0 && index < gvSearch.Rows.Count)
            {
                txtContactId.Text = gvSearch.Rows[index].Cells[1].Text;
                lblName.Text = gvSearch.Rows[index].Cells[3].Text + ", " + gvSearch.Rows[index].Cells[2].Text;
                lblId.Text = txtContactId.Text;

                pnlContactSearch.Visible = false;
                pnlFullPage.Visible = true;
            }
        }

        /*
         * Submit the changes to the judge's preferences for the selected audition
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int contactId = -1;
            bool success = true;

            if (ddlDistrict.SelectedIndex > 0 && ddlYear.SelectedIndex > 0 && Int32.TryParse(lblId.Text, out contactId))
            {
                int auditionOrgId = DbInterfaceAudition.GetAuditionOrgId(Convert.ToInt32(ddlDistrict.SelectedValue), Convert.ToInt32(ddlYear.SelectedValue));

                // Delete all current preferences for the audition before updating
                DbInterfaceJudge.DeleteJudgePreferences(contactId, auditionOrgId);

                // Add new audition tracks
                foreach (ListItem item in chkLstTrack.Items)
                {
                    if (item.Selected && item.Value.Equals("District")) // Convert District to D2, D2NM, and D3
                    {
                        success = success && DbInterfaceJudge.AddJudgeAuditionLevel(contactId, auditionOrgId, "D2");
                        success = success && DbInterfaceJudge.AddJudgeAuditionLevel(contactId, auditionOrgId, "D2NM");
                        success = success && DbInterfaceJudge.AddJudgeAuditionLevel(contactId, auditionOrgId, "D3");
                    }
                    else if (item.Selected)
                    {
                        success = success && DbInterfaceJudge.AddJudgeAuditionLevel(contactId, auditionOrgId, "State");
                    }
                }

                // Add new audition types
                foreach (ListItem item in chkLstType.Items)
                {
                    if (item.Selected)
                        success = success && DbInterfaceJudge.AddJudgeAuditionType(contactId, auditionOrgId, item.Value);
                }

                // Add new comp levels
                foreach (ListItem item in chkLstCompLevel.Items)
                {
                    if (item.Selected)
                        success = success && DbInterfaceJudge.AddJudgeLevel(contactId, auditionOrgId, item.Value);
                }

                // Add new instruments
                foreach (ListItem item in chkLstInstrument.Items)
                {
                    if (item.Selected)
                        success = success && DbInterfaceJudge.AddJudgeInstrument(contactId, auditionOrgId, item.Value);
                }

                // Add new time preferences
                //foreach (ListItem item in chkLstTime.Items)
                //{
                //    if (item.Selected)
                //        success = success && DbInterfaceJudge.AddJudgeTime(contactId, auditionOrgId, Convert.ToInt32(item.Value));
                //}
            }
            else
            {
                showWarningMessage("Please select a judge, district, and year.");
                success = false;
            }

            if (success)
            {
                showSuccessMessage("The judge's preferences were successfully updated.");
                clearPage();
            }
            else
            {
                showErrorMessage("There was an error updating the judge's preferences");
            }
        }

        protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearPreferences();
            LoadJudgeAndAuditionData();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearPreferences();
            LoadJudgeAndAuditionData();
        }

        /*
         * Load the judge's preferences and the audition's time ranges
         */
        private void LoadJudgeAndAuditionData()
        {
            if (ddlDistrict.SelectedIndex > 0 && ddlYear.SelectedIndex > 0)
            {
                int auditionOrgId = DbInterfaceAudition.GetAuditionOrgId(Convert.ToInt32(ddlDistrict.SelectedValue), Convert.ToInt32(ddlYear.SelectedValue));

                if (auditionOrgId != -1)
                {
                    //LoadAuditionTimes(auditionOrgId);
                    LoadJudgePreferences(auditionOrgId);

                    pnlJudges.Visible = true;
                    pnlButtons.Visible = true;
                }
                else
                    showWarningMessage("The audition for the selected year has not been created.");
            }
            else
            {
                pnlJudges.Visible = false;
                pnlButtons.Visible = false;
            }
        }

        /*
         * Load the time options for the selected audition
         */
        //private void LoadAuditionTimes(int auditionOrgId)
        //{
        //    DataTable timePreferences = DbInterfaceAudition.LoadJudgeTimePreferenceOptions(auditionOrgId);
        //    chkLstTime.DataSource = timePreferences;
        //    chkLstTime.DataBind();
        //}

        /*
         * Load the preferences of the selected judge for the selected year and audition district
         */
        private void LoadJudgePreferences(int auditionOrgId)
        {
            int contactId = Convert.ToInt32(lblId.Text);
            List<string> auditionLevels = DbInterfaceJudge.LoadJudgeAuditionLevels(contactId, auditionOrgId);
            List<string> types = DbInterfaceJudge.LoadJudgeAuditionTypes(contactId, auditionOrgId);
            List<string> levels = DbInterfaceJudge.LoadJudgeLevels(contactId, auditionOrgId);
            List<string> instruments = DbInterfaceJudge.LoadJudgeInstruments(contactId, auditionOrgId);
            List<string> timeIds = DbInterfaceJudge.LoadJudgeTimes(contactId, auditionOrgId);

            // Load audition levels
            foreach (string level in auditionLevels)
            {
                int idx = -1;

                if (level.Equals("D2") || level.Equals("D2NM") || level.Equals("D3")) // Convert D2, D2NM, and D3 to District
                {
                    idx = chkLstTrack.Items.IndexOf(new ListItem("District"));

                    if (idx >= 0) chkLstTrack.Items.FindByValue("District").Selected = true;
                }
                else
                {
                    idx = chkLstTrack.Items.IndexOf(new ListItem(level));

                    if (idx >= 0) chkLstTrack.Items.FindByValue(level).Selected = true;
                }
            }

            // Load audition types
            foreach (string type in types)
            {
                int idx = -1;

                idx = chkLstType.Items.IndexOf(new ListItem(type));

                if (idx >= 0) chkLstType.Items.FindByValue(type).Selected = true;
            }

            // Load levels
            foreach (string level in levels)
            {
                if (chkLstCompLevel.Items.Count == 0) chkLstCompLevel.DataBind();

                ListItem temp = chkLstCompLevel.Items.FindByValue(level.Trim());

                if (temp != null)
                    chkLstCompLevel.Items.FindByValue(temp.Value).Selected = true;
            }

            // Load instruments
            foreach (string instrument in instruments)
            {
                int idx = -1;

                if (chkLstInstrument.Items.Count == 0) chkLstInstrument.DataBind();

                idx = chkLstInstrument.Items.IndexOf(new ListItem(instrument));

                if (idx >= 0) chkLstInstrument.Items.FindByValue(instrument).Selected = true;
            }

            // Load times
            //foreach (string timeId in timeIds)
            //{
            //    if (chkLstTime.Items.Count == 0) chkLstTime.DataBind();

            //    ListItem temp = chkLstTime.Items.FindByValue(timeId.Trim());

            //    if (temp != null)
            //        chkLstTime.Items.FindByValue(temp.Value).Selected = true;
            //}
        }

        /*
         * Pre:   
         * Post:  The page of gvSearch is changed
         */
        protected void gvSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        /*
         * Pre:   The contact search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[contactSearch];
                gvSearch.DataSource = data;
                gvSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Manage Contacts", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvSearch, e);
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        private void clearPage()
        {
            clearData();
            clearContactSearch();
            pnlFullPage.Visible = false;
            pnlButtons.Visible = false;
            pnlContactSearch.Visible = true;
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            clearContactSearch();
        }

        /*
         * Pre:
         * Post: Clears the Contact Search section
         */
        private void clearContactSearch()
        {
            txtContactId.Text = "";
            txtFirstNameSearch.Text = "";
            txtLastNameSearch.Text = "";
            gvSearch.DataSource = null;
            gvSearch.DataBind();
        }

        /*
         * Pre:
         * Post:  Clears entered data, error messages, and error
         *        highlighting on the page
         */
        private void clearData()
        {
            //clear text and selections
            lblId.Text = "";
            lblName.Text = "";
            ddlDistrict.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;

            // Hide everything except search
            pnlFullPage.Visible = false;
            pnlButtons.Visible = false;
            pnlJudges.Visible = false;
            pnlContactSearch.Visible = true;

            clearPreferences();

            Session[contactSearch] = null;
            Session[contactVar] = null;
            Session[judgeVar] = null;
        }

        /*
         * Clear the judge preferences
         */
        private void clearPreferences()
        {
            foreach (ListItem item in chkLstType.Items)
                item.Selected = false;
            foreach (ListItem item in chkLstTrack.Items)
                item.Selected = false;
            foreach (ListItem item in chkLstCompLevel.Items)
                item.Selected = false;
            foreach (ListItem item in chkLstInstrument.Items)
                item.Selected = false;
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
         * Post: Displays the input informational message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showInfoMessage(string message)
        {
            lblInfoMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowInfo", "showInfoMessage()", true);
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
            Utility.LogError("Manage Contacts", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            showErrorMessage("Error: An error occurred");

            //Pass error on to error page
            Server.Transfer("../ErrorPage.aspx", true);
        }

        #endregion Messages
    }
}
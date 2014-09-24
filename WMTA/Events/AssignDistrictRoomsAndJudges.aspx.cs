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
    public partial class AssignDistrictRoomsAndJudges : System.Web.UI.Page
    {
        /* session variables */
        private string auditionSearch = "AuditionData"; //tracks data returned by latest audition search

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session[auditionSearch] = null;

                loadYearDropdown();
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
         * Post: Perform an audition search with the input criteria.  Display results in gridview
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
                DataTable table = DbInterfaceAudition.GetAuditionSearchResults("", "", districtId, year);

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
                    showInfoMessage("The search did not return any results.");
                    clearGridView(gridview);
                    result = false;
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("AssignDistrictRoomsAndJudges", "searchAuditions", "gridView: " + gridview.ID + ", districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        protected void gvAuditionSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            //clear all
            int index = gvAuditionSearch.SelectedIndex;

            if (index >= 0 && index < gvAuditionSearch.Rows.Count)
            {
                int auditionId = Convert.ToInt32(gvAuditionSearch.Rows[index].Cells[1].Text);

                //populate event information
                ddlDistrictSearch.SelectedIndex = 
                            ddlDistrictSearch.Items.IndexOf(ddlDistrictSearch.Items.FindByText(
                            gvAuditionSearch.Rows[index].Cells[2].Text));

                ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(
                            gvAuditionSearch.Rows[index].Cells[3].Text));

                loadAuditionData(auditionId);
                loadRooms(auditionId);

                //load theory test rooms
                //load judges
                //load judge rooms

                pnlMain.Visible = true;
            }
        }

        /*
         * Pre:  audition must exist as the id of an audition in the system
         * Post: The existing data for the audition associated with the auditionId 
         *       is loaded to the page.
         * @param auditionId is the id of the audition being scheduled
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
                    lblAuditionSite.Text = audition.venue;
                    lblAuditionDate.Text = audition.auditionDate.ToShortDateString();
                }
                else
                {
                    showErrorMessage("Error: The audition information could not be loaded.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the audition data.");

                Utility.LogError("Assign District Rooms and Judges", "loadAuditionData", "auditionId: " + auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        private void loadRooms(Audition audition)
        {
            try
            {
                audition.GetRooms();

            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the event's rooms.");
                Utility.LogError("Assign District Rooms and Judges", "loadRooms", "auditionId: " + auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        protected void btnAddRoom_Click(object sender, EventArgs e)
        {

        }

        protected void btnRemoveRoom_Click(object sender, EventArgs e)
        {

        }

        protected void gvRooms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnAddTestRoom_Click(object sender, EventArgs e)
        {

        }

        protected void btnRemoveTestRoom_Click(object sender, EventArgs e)
        {

        }

        protected void gvTestRooms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnAddJudge_Click(object sender, EventArgs e)
        {

        }

        protected void btnRemoveJudge_Click(object sender, EventArgs e)
        {

        }

        protected void gvJudges_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnAddJudgeRoom_Click(object sender, EventArgs e)
        {

        }

        protected void btnRemoveJudgeRoom_Click(object sender, EventArgs e)
        {

        }

        protected void gvJudgeRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void ddlJudge_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlAuditionJudges_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlJudgeRoom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PageIndexChanging(GridView gv, GridViewPageEventArgs e)
        {
            gv.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        /*
         * Pre:   The audition search table must have been previously defined
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
                Utility.LogError("AssignDistrictRoomsAndJudges", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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

        #region gridview events
        protected void gvAuditionSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndexChanging(gvAuditionSearch, e);
        }

        protected void gvRooms_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndexChanging(gvRooms, e);
        }

        protected void gvTestRooms_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndexChanging(gvTestRooms, e);
        }

        protected void gvJudges_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndexChanging(gvJudges, e);
        }

        protected void gvJudgeRooms_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndexChanging(gvJudgeRooms, e);
        }

        protected void gvAuditionSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvAuditionSearch, e);
        }

        protected void gvRooms_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvRooms, e);
        }

        protected void gvTestRooms_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvTestRooms, e);
        }

        protected void gvJudges_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvJudges, e);
        }

        protected void gvJudgeRooms_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvJudgeRooms, e);
        }
        #endregion gridview events

        /// <summary>
        /// Clear the search fields
        /// </summary>
        protected void btnClearAuditionSearch_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        /// <summary>
        /// Clear the search fields
        /// </summary>
        private void ClearSearch()
        {
            ddlDistrictSearch.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
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
            Utility.LogError("Assign District Rooms and Judges", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
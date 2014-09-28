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
        private string auditionSession = "Audition";
        private string roomsTable = "Rooms", theoryRoomsTable = "TheoryRoomsTable";
        private string theoryRooms = "TheoryRooms", judgeRooms = "JudgeRooms";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session[auditionSearch] = null;
                Session[auditionSession] = null;
                Session[roomsTable] = null;
                Session[theoryRoomsTable] = null;
                Session[theoryRooms] = null;
                Session[judgeRooms] = null;

                loadYearDropdown();
            }

            // If there were rooms added before the postback, add them back to the table
            if (Page.IsPostBack && Session[roomsTable] != null)
            {
                TableRow[] rowArray = (TableRow[])Session[roomsTable];

                for (int i = 1; i < rowArray.Length; i++)
                    tblRooms.Rows.Add(rowArray[i]);
            }

            // If there were theoy rooms added, add them back to the table
            if (Page.IsPostBack && Session[theoryRoomsTable] != null)
            {
                TableRow[] rowArray = (TableRow[])Session[theoryRoomsTable];

                for (int i = 1; i < rowArray.Length; i++)
                    tblTheoryRooms.Rows.Add(rowArray[i]);
            }

            // Reload the available theory test rooms
            if (Page.IsPostBack && Session[theoryRooms] != null)
            {
                ListItem[] itemArray = (ListItem[])Session[theoryRooms];

                for (int i = 1; i < itemArray.Length; i++)
                    ddlRoom.Items.Add(new ListItem(itemArray[i].Text));
            }

            // Reload the available judging rooms
            if (Page.IsPostBack && Session[judgeRooms] != null)
            {
                ListItem[] itemArray = (ListItem[])Session[judgeRooms];

                for (int i = 1; i < itemArray.Length; i++)
                    ddlJudgeRoom.Items.Add(new ListItem(itemArray[i].Text));
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

                Audition audition = loadAuditionData(auditionId);
                LoadRooms(audition);
                LoadTheoryRooms(audition);
                loadJudges(audition);
                
                //load judges
                //load judge rooms

                Session[auditionSession] = audition;
                pnlMain.Visible = true;
                upAuditionSearch.Visible = false;
            }
        }

        /*
         * Pre:  audition must exist as the id of an audition in the system
         * Post: The existing data for the audition associated with the auditionId 
         *       is loaded to the page.
         * @param auditionId is the id of the audition being scheduled
         * @returns the audition data
         */
        private Audition loadAuditionData(int auditionId)
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

            return audition;
        }

        /*
         * Pre:
         * Post: Load all available rooms for an existing audition
         */
        private void LoadRooms(Audition audition)
        {
            clearRooms();

            try
            {
                List<string> rooms = audition.GetRooms();

                // Load each room to the table and all room dropdowns
                foreach (string room in rooms)
                {
                    AddRoom(room);
                }

                if (rooms != null && rooms.Count > 0)
                    pnlRooms.Visible = true;
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the event's rooms.");
                Utility.LogError("Assign District Rooms and Judges", "loadRooms", "auditionId: " + audition.auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Load the theory rooms for an existing audition
         */
        private void LoadTheoryRooms(Audition audition)
        {
            clearTheoryRooms();

            try
            {
                List<Tuple<string, string>> theoryRooms = audition.GetTheoryRooms();

                // Load each room to the table
                foreach (Tuple<string, string> room in theoryRooms)
                {
                    AddTheoryRoom(room.Item1, room.Item2);
                }

                if (theoryRooms != null & theoryRooms.Count > 0)
                    pnlTheoryRooms.Visible = true;
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the event's theory test rooms.");
                Utility.LogError("Assign District Rooms and Judges", "loadTheoryRooms", "auditionId: " + audition.auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        private void LoadJudges(Audition audition)
        {
            clearJudges();

            try
            {

            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while loading the event's judges.");
                Utility.LogError("AssignDistrictRoomsAndJudges", "LoadJudges", "auditionId: " + audition.auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Add the new room to the table if it doesn't already exist
         */
        protected void btnAddRoom_Click(object sender, EventArgs e)
        {
            string room = txtRoom.Text;

            if (!room.Equals("") && !RoomExists(room))
            {
                AddRoom(room);

                pnlRooms.Visible = true;
            }
            else
            {
                showWarningMessage("Please enter a room name.");
            }
        }

        /*
         * Pre:
         * Post: Add a row to the rooms table and dropdowns with the specified room name
         */
        private void AddRoom(string room)
        {
            TableRow row = new TableRow();
            TableCell chkBoxCell = new TableCell();
            TableCell roomCell = new TableCell();
            CheckBox chkBox = new CheckBox();

            // Add a checkbox to column 1
            chkBoxCell.Controls.Add(chkBox);

            // Add the room name to column 2
            roomCell.Text = room;

            // Add the cells to the new row
            row.Cells.Add(chkBoxCell);
            row.Cells.Add(roomCell);

            // Add the new row to the table
            tblRooms.Rows.Add(row);

            // Add the room to the theory and judging rooms dropdown
            ddlRoom.Items.Add(new ListItem(room));
            ddlJudgeRoom.Items.Add(new ListItem(room));

            // Save the updated table to the session
            saveTableToSession(tblRooms, roomsTable);
            saveDropdownToSession(ddlRoom, theoryRooms);
            saveDropdownToSession(ddlJudgeRoom, judgeRooms);
        }

        /*
         * Pre: 
         * Post: Add a row the theory rooms table for the specified test and room
         */
        private void AddTheoryRoom(string theoryTest, string room)
        {
            TableRow row = new TableRow();
            TableCell chkBoxCell = new TableCell();
            TableCell testCell = new TableCell();
            TableCell roomCell = new TableCell();
            CheckBox chkBox = new CheckBox();

            // Add a checkbox to column 1
            chkBoxCell.Controls.Add(chkBox);

            // Add the test and room names to columns 2 and 3
            testCell.Text = theoryTest;
            roomCell.Text = room;

            // Add the cells to the new row
            row.Cells.Add(chkBoxCell);
            row.Cells.Add(testCell);
            row.Cells.Add(roomCell);

            // Add the new row to the table
            tblTheoryRooms.Rows.Add(row);

            // Save the updated table to the session
            saveTableToSession(tblTheoryRooms, theoryRoomsTable);
        }

        /*
         * Pre:
         * Post: Determine if the input room is already in the table
         * @returns true if it exists and false otherwise
         */
        private bool RoomExists(string room)
        {
            for (int i = 0; i < tblRooms.Rows.Count; i++)
            {
                if (tblRooms.Rows[i].Cells[1].Text.Equals(room))
                {
                    showInfoMessage("The specified room has already been added.");
                    return true;
                }
            }

            return false;
        }

        /*
         * Pre:
         * Post: Determine if the input test is already in the table
         * @returns true if it exists and false otherwise
         */
        private bool TheoryTestExists(string test)
        {
            for (int i = 0; i < tblTheoryRooms.Rows.Count; i++)
            {
                if (tblTheoryRooms.Rows[i].Cells[1].Text.Equals(test))
                {
                    showInfoMessage("The specified theory test has already been added.");
                    return true;
                }
            }

            return false;
        }

        /*
         * Pre:
         * Post: Remove selected rooms
         */
        protected void btnRemoveRoom_Click(object sender, EventArgs e)
        {
            bool roomSelected = false;

            // Remove any checked rows
            for (int i = 1; i < tblRooms.Rows.Count; i++)
            {
                if (((CheckBox)tblRooms.Rows[i].Cells[0].Controls[0]).Checked)
                {
                    tblRooms.Rows.Remove(tblRooms.Rows[i]);
                    roomSelected = true;
                    i--;
                }
            }

            // Display a message if no room was selected
            if (!roomSelected)
            {
                showWarningMessage("Please select a room to remove.");
            }
            else // Save changes
            {
                saveTableToSession(tblRooms, roomsTable);
            }
        }
    

        protected void tblRooms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /*
         * Pre:
         * Post: Adds the new theory test room to the table, if the test
         *       has not already been assigned a room
         */
        protected void btnAddTestRoom_Click(object sender, EventArgs e)
        {
            string test = ddlTheoryTest.SelectedValue.ToString();
            string room = ddlRoom.SelectedValue.ToString();

            if (!test.Equals("") && !room.Equals("") && !TheoryTestExists(test))
            {
                AddTheoryRoom(test, room);

                pnlRooms.Visible = true;
            }
            else if (test.Equals(""))
            {
                showWarningMessage("Please select a theory test.");
            }
            else if (room.Equals(""))
            {
                showWarningMessage("Please select a room for the theory test.");
            }
        }

        /*
         * Pre:
         * Post: Remove selected theory test rooms
         */
        protected void btnRemoveTestRoom_Click(object sender, EventArgs e)
        {
            bool roomSelected = false;

            // Remove any checked rows
            for (int i = 1; i < tblTheoryRooms.Rows.Count; i++)
            {
                if (((CheckBox)tblTheoryRooms.Rows[i].Cells[0].Controls[0]).Checked)
                {
                    tblTheoryRooms.Rows.Remove(tblTheoryRooms.Rows[i]);
                    roomSelected = true;
                    i--;
                }
            }

            // Display a message if no room was selected
            if (!roomSelected)
            {
                showWarningMessage("Please select a theory test room to remove.");
            }
            else // Save changes
            {
                saveTableToSession(tblTheoryRooms, theoryRoomsTable);
            }
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

        /*
         * Pre:
         * Post: The table in the input is saved to a session variable
         * @table is the table being saved
         * @session is the name of the session variable
         */
        private void saveTableToSession(Table table, string session)
        {
            TableRow[] rowArray = new TableRow[table.Rows.Count];
            table.Rows.CopyTo(rowArray, 0);
            Session[session] = rowArray;
        }

        /*
         * Pre:
         * Post: The dropdown list in the input is saved to a session variable
         * @ddl is the dropdown list being saved
         * @session is the name of the session variable
         */
        private void saveDropdownToSession(DropDownList ddl, string session)
        {
            ListItem[] itemArray = new ListItem[ddl.Items.Count];
            ddl.Items.CopyTo(itemArray, 0);
            Session[session] = itemArray;
        }

        #region gridview events
        protected void gvAuditionSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndexChanging(gvAuditionSearch, e);
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

        protected void gvJudges_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvJudges, e);
        }

        protected void gvJudgeRooms_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvJudgeRooms, e);
        }
        #endregion gridview events

        /*
         * Pre:
         * Post: Clear the search fields
         */
        protected void btnClearAuditionSearch_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        /*
         * Pre:
         * Post: Clear the search fields
         */
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

        private void clearRooms()
        {
            // Clear the rooms table
            while (tblRooms.Rows.Count > 1)
            {
                tblRooms.Rows.RemoveAt(1);
            }

            // Clear the theory test room dropdown
            ddlRoom.Items.Clear();
            ddlRoom.Items.Add(new ListItem(""));

            // Clear the judging room dropdown
            ddlJudgeRoom.Items.Clear();
            ddlJudgeRoom.Items.Add(new ListItem(""));
            // Save changes to session
            saveTableToSession(tblRooms, roomsTable);
            saveDropdownToSession(ddlRoom, theoryRooms);
        }

        private void clearTheoryRooms()
        {
            // Clear the theory rooms table
            while (tblTheoryRooms.Rows.Count > 1)
            {
                tblTheoryRooms.Rows.RemoveAt(1);
            }

            // Save changes to session
            saveTableToSession(tblTheoryRooms, theoryRoomsTable);
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
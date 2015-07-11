using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Events
{
    public partial class DeleteCoordinations : System.Web.UI.Page
    {
        private DistrictAudition audition;
        //session variables
        private string auditionVar = "Audition";
        private string studentSearch = "StudentData";
        private string coordinateTable = "CoordinateTable";

        protected void Page_Load(object sender, EventArgs e)
        {
            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[auditionVar] = null;
                Session[studentSearch] = null;
                Session[coordinateTable] = null;

                loadYearDropdown();
                checkPermissions();
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[auditionVar] != null)
            {
                audition = (DistrictAudition)Session[auditionVar];
            }

            //if there were coordinates selected before the postback, add them 
            //back to the table
            if (Page.IsPostBack && Session[coordinateTable] != null)
            {
                TableRow[] rowArray = (TableRow[])Session[coordinateTable];

                for (int i = 1; i < rowArray.Length; i++)
                    tblCoordinates.Rows.Add(rowArray[i]);
            }
        }

        /*
         * Pre:
         * Post: Loads the appropriate years in the dropdown
         */
        private void loadYearDropdown()
        {
            int firstYear = DbInterfaceStudentAudition.GetFirstAuditionYear();

            for (int i = DateTime.Now.Year; i >= firstYear; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
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
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students are displayed that match the search criteria (student id, first name, and last name).
         *        The error message is also reset.
         */
        protected void btnStudentSearch_Click(object sender, EventArgs e)
        {
            int num;
            string id = txtStudentId.Text;
            bool isNum = int.TryParse(id, out num);

            if (isNum || txtStudentId.Text.Equals(""))
            {
                User user = (User)Session[Utility.userRole];
                int districtId = -1;

                //if user is district admin get their district
                if (!user.permissionLevel.Contains('A'))
                    districtId = user.districtId;

                searchStudents(gvStudentSearch, txtStudentId.Text, txtFirstName.Text, txtLastName.Text, studentSearch, districtId);
            }
            else
            {
                clearGridView(gvStudentSearch);
                showWarningMessage("A Student Id must be numeric");
            }

            cboAudition.Items.Clear();
            cboAudition.Items.Add(new ListItem("", ""));
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students.  Matching student 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param session is the name of the session variable storing the student search table data
         * @districtId is the district in which to search for students, -1 indicates all districts
         * @returns true if results were found and false otherwise
         */
        private bool searchStudents(GridView gridView, string id, string firstName, string lastName, string session, int districtId)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceStudent.GetStudentSearchResults(id, firstName, lastName, districtId);

                //If there are results in the table, display them.  Otherwise clear current
                //results and return false
                if (table != null && table.Rows.Count > 0)
                {
                    gridView.DataSource = table;
                    gridView.DataBind();

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
                    showErrorMessage("Error: An error occurred during the search.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("District Point Entry", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Pre:   gvStudentSearch must contain more than one page
         * Post:  The page of gvStudentSearch is changed
         */
        protected void gvStudentSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudentSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvStudentSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in gvStudentSearch.HeaderRow.Cells)
                {
                    cell.BackColor = Color.Black;
                    cell.ForeColor = Color.White;
                }
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
                DataTable data = (DataTable)Session["StudentData"];
                gvStudentSearch.DataSource = data;
                gvStudentSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("District Point Entry", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:   The selected index must be a positive number less than the number of rows
         *        in the gridView
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearAllExceptSearch();

            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                string id = gvStudentSearch.Rows[index].Cells[1].Text;

                txtStudentId.Text = id;
                lblStudId.InnerText = id;

                loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text), true);
            }
        }

        /*
         * Pre:  studentId must exist as a StudentId in the system
         * Post: The existing data for the student associated to the studentId 
         *       is loaded to the page.
         * @param studentId is the StudentId of the student being registered
         */
        private Student loadStudentData(int studentId, bool initialLoad)
        {
            Student student = DbInterfaceStudent.LoadStudentData(studentId);

            //get eligible auditions
            if (student != null)
            {
                //load student name
                lblStudId.InnerText = student.id.ToString();
                txtFirstName.Text = student.firstName;
                txtLastName.Text = student.lastName;
                lblStudent.Text = student.firstName + " " + student.lastName;

                upAuditions.Visible = true;
                upStudentSearch.Visible = false;
            }
            else
            {
                showErrorMessage("An error occurred while loading the student's audition data.");
            }

            return student;
        }

        private void loadAuditions()
        {
            int studentId = -1;

            if (ddlYear.SelectedIndex > 0 && ddlAuditionType.SelectedIndex > 0 && Int32.TryParse(lblStudId.InnerText, out studentId))
            {
                DataTable table = DbInterfaceStudentAudition.GetDistrictOrStateAuditionsForDropdownByYear(studentId, Convert.ToInt32(ddlYear.SelectedValue), ddlAuditionType.SelectedValue.Equals("District"));
                cboAudition.DataSource = null;
                cboAudition.Items.Clear();
                cboAudition.DataSourceID = "";

                if (table.Rows.Count > 0)
                {
                    cboAudition.DataSource = table;
                    cboAudition.DataTextField = "DropDownInfo";
                    cboAudition.DataValueField = "AuditionId";
                    cboAudition.Items.Add(new ListItem(""));
                    cboAudition.DataBind();
                }
                else
                {
                    showWarningMessage("This student has no editable auditions for the selected year.");
                }
            }
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadAuditions();
        }

        protected void ddlAuditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadAuditions();
        }

        protected void cboAudition_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlCoordinates.Visible = true;
            
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
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
        {
            clearStudentSearch();
        }

        /*
         * Pre: 
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        private void clearStudentSearch()
        {
            txtStudentId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            gvStudentSearch.DataSource = null;
            gvStudentSearch.DataBind();
        }

        /*
         * Pre:
         * Post: Clears all data except student search
         */
        private void clearAllExceptSearch()
        {
            ddlYear.SelectedIndex = 0;

            lblStudent.Text = "";
            lblStudId.InnerText = "";
            ddlYear.SelectedIndex = 0;
            ddlAuditionType.SelectedIndex = -1;
            clearCoordinates();
            cboAudition.Items.Clear();
        }

        /*
         * Pre:
         * Post: Clears the coordinates table
         */
        private void clearCoordinates()
        {
            //clear the coordinates saved in the table
            while (tblCoordinates.Rows.Count > 1)
                tblCoordinates.Rows.Remove(tblCoordinates.Rows[tblCoordinates.Rows.Count - 1]);


            Session[coordinateTable] = null;
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
            Utility.LogError("District Point Entry", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

        }
    }
}
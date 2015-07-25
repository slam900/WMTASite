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
    public partial class CoordinateStudents : System.Web.UI.Page
    {
        /* session variables */
        private string studentSearch = "StudentData";
        private string coordinateTable = "CoordinateTable";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[studentSearch] = null;
                Session[coordinateTable] = null;
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
         * Post: If the user is not logged in they will be redirected to the welcome screen
         */
        private void checkPermissions()
        {
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
        }

        /*
         * Pre:
         * Post: If the entered data is valid, the coordination is set between the two students
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool success = true;

            if (dataIsValid())
            {
                string reason = ddlReason.SelectedValue;
                bool isDistrictAudition = ddlAuditionType.SelectedValue.Equals("District");

                // Put all students into a list
                List<Tuple<Student, StudentCoordinate>> students = new List<Tuple<Student, StudentCoordinate>>();
                for (int i = 1; i < tblCoordinates.Rows.Count; i++)
                {
                    Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(tblCoordinates.Rows[i].Cells[1].Text));

                    // Get the student's coordinates and add to list
                    if (student != null)
                    {
                        StudentCoordinate coordinate = new StudentCoordinate(student, reason, true, isDistrictAudition);
                        students.Add(new Tuple<Student, StudentCoordinate>(student, coordinate));

                    }
                    else
                        success = false;
                }

                // Coordinate all students
                for (int i = 0; i < students.Count - 1; i++) 
                {
                    StudentCoordinate student1Coordinates = students.ElementAt(i).Item2;

                    for (int j = i + 1; j < students.Count; j++) // Coordinate student1 with all of the remaining students in the list
                    {
                        StudentCoordinate student2Coordinates = students.ElementAt(j).Item2;

                        foreach (int student1Id in student1Coordinates.auditionIds)
                        {
                            foreach (int student2Id in student2Coordinates.auditionIds)
                                success = success && DbInterfaceStudentAudition.CreateAuditionCoordinate(student1Id, student2Id, reason);
                        }

                    }
                }

                //display message depending on whether or not the operation was successful
                if (success)
                {
                    showSuccessMessage("The students were successfully coordinated.");
                    clearPage();
                }
                else
                {
                    showErrorMessage("Error: An error occurred while coordinating the students.");
                }
            }
        }

        /*
         * Pre:
         * Post: Returns true if the entered data is valid and false otherwise.
         *       Both students must be selected and a reason must be chosen
         */
        private bool dataIsValid()
        {
            bool valid = true;

            if (tblCoordinates.Rows.Count < 3)
            {
                showErrorMessage("You must select at least 2 students to coordinate.");
                valid = false;
            }

            return valid;
        }

        /*
         * Pre:   
         * Post:  The selected student is added to the list of coordinates
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)  
            {
                string id = gvStudentSearch.Rows[index].Cells[1].Text;
                string firstName = gvStudentSearch.Rows[index].Cells[2].Text;
                string lastName = gvStudentSearch.Rows[index].Cells[3].Text;

                // Add the student to the table if they aren't already there
                if (!StudentExists(id))
                {
                    //load student data to avoid the bug where ' shows up as &#39; if the data is just taken from the gridview
                    Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                    if (student != null)
                    {
                        firstName = student.firstName;
                        lastName = student.lastName;
                    }

                    //load search fields
                    txtStudentId.Text = id;
                    txtFirstName.Text = firstName;
                    txtLastName.Text = lastName;

                    // Add student to table
                    AddCoordinate(id, firstName, lastName);
                    clearStudentSearch();
                }
                else
                    showWarningMessage("The student has already been added.");

            }
        }

        /*
         * Determines whether or not the student has already been added to the table
         */
        private bool StudentExists(string id)
        {
            bool exists = false;

            for (int i = 1; !exists && i < tblCoordinates.Rows.Count; i++)
            {
                if (tblCoordinates.Rows[i].Cells[1].Text.Equals(id))
                    exists = true;
            }

            return exists;
        }

        /*
         * Add the student info to the table
         */
        private void AddCoordinate(string id, string firstName, string lastName)
        {
            TableRow row = new TableRow();
            TableCell chkBoxCell = new TableCell();
            TableCell studIdCell = new TableCell();
            TableCell nameCell = new TableCell();
            CheckBox chkBox = new CheckBox();

            //set cell values
            chkBoxCell.Controls.Add(chkBox);
            studIdCell.Text = id;
            nameCell.Text = lastName + ", " + firstName;

            //add cells to new row
            row.Cells.Add(chkBoxCell);
            row.Cells.Add(studIdCell);
            row.Cells.Add(nameCell);

            //add new row to table
            tblCoordinates.Rows.Add(row);

            //save table to session variable as an array
            saveTableToSession(tblCoordinates, coordinateTable);
        }

        /*
         * The selected students are removed from the list of coordinates
         */
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            bool coordinateSelected = false;

            // Remove each checked row
            for (int i = 1; i < tblCoordinates.Rows.Count; i++)
            {
                if (((CheckBox)tblCoordinates.Rows[i].Cells[0].Controls[0]).Checked)
                {
                    tblCoordinates.Rows.Remove(tblCoordinates.Rows[i]);
                    coordinateSelected = true;
                    i--;                
                }
            }

            // If no coordinates were selected, display a message
            if (!coordinateSelected)
                showWarningMessage("Please select a coordinate to remove.");
            else
                saveTableToSession(tblCoordinates, coordinateTable);
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
         * Post:  The page of gvStudentSearch is changed
         */
        protected void gvStudentSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudentSearch.PageIndex = e.NewPageIndex;
            BindStudent1SessionData();
        }

        /*
         * Pre:   The student search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindStudent1SessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[studentSearch];
                gvStudentSearch.DataSource = data;
                gvStudentSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Coordinate Students", "BindStudent1SessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvStudentSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvStudentSearch, e);
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
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students matching the search criteria are displayed (student id, first name, 
         *        and last name). The error message is also reset.
         */
        protected void btnSearchStudent_Click(object sender, EventArgs e)
        {
            string id = txtStudentId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                if (userIsTeacherOnly()) //if the current user is a teacher, search only their students
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchOwnStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, ((User)Session[Utility.userRole]).contactId))
                    {
                        showInfoMessage("The search did not return any results.");
                    }
                }
                else //if current user is a district admin search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, getDistrictIdForPermissionLevel()))
                    {
                        showInfoMessage("The search did not return any results.");
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudentSearch);
                showWarningMessage("A Student Id must be numeric");
            }
        }

        /*
         * Pre:
         * Post: Determines whether the current user's highest permission level is 'Teacher'
         * @returns true if the user's highest permission level is 'Teacher' and false otherwise
         */
        private bool userIsTeacherOnly()
        {
            User user = (User)Session[Utility.userRole];

            return user.permissionLevel.Contains('T') && !user.permissionLevel.Contains('D') && !user.permissionLevel.Contains('S');
        }

        /*
         * Pre:
         * Post: If the current user's permission level should only allow them to edit/view contacts
         *       of a certain district, that district id is returned.  Otherwise -1 is returned.
         * @returns the district id that should be viewable by the current user or -1 for all districts
         */
        private int getDistrictIdForPermissionLevel()
        {
            User user = (User)Session[Utility.userRole];
            int districtId = -1;

            if (user.permissionLevel.Contains('D') && !(user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
                districtId = user.districtId;

            return districtId;
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students.  Matching student 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param districtId is the id of the district to search in
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
                    showWarningMessage("Error: An error occurred during the search");
                }
            }
            catch (Exception e)
            {
                showWarningMessage("Error: An error occurred during the search");

                Utility.LogError("Coordinate Students", "searchStudents", "gridview: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students for the currently logged
         *        in teacher.  Matching student information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param teacherContactId is the id of the current teacher
         * @returns true if results were found and false otherwise
         */
        private bool searchOwnStudents(GridView gridView, string id, string firstName, string lastName, string session, int teacherContactId)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceStudent.GetStudentSearchResultsForTeacher(id, firstName, lastName, teacherContactId);

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
                    showWarningMessage("Error: An error occurred during the search");
                }
            }
            catch (Exception e)
            {
                showWarningMessage("Error: An error occurred during the search");

                Utility.LogError("Coordinate Students", "searchOwnStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Clear page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Clears the data on the page
         */
        private void clearPage()
        {
            clearStudentSearch();
            ddlReason.SelectedIndex = 0;
            ddlAuditionType.SelectedIndex = 0;
            tblCoordinates.Rows.Clear();
            Session[coordinateTable] = null;
        }

        /*
         * Clear Student Search section
         */
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
        {
            clearStudentSearch();
        }

        /*
         * Clear Student Search section
         */
        private void clearStudentSearch()
        {
            txtStudentId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";

            clearGridView(gvStudentSearch);
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
            Utility.LogError("Coordinate Students", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
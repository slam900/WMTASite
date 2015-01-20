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
        private string student1Search = "Student1Data";
        private string student2Search = "Student2Data";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[student1Search] = null;
                Session[student2Search] = null;
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

                //if current user is teacher, only show that teacher's students in the student dropdowns
                if (user.permissionLevel.Contains('T') && !(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('D') || user.permissionLevel.Contains('S')))
                    showTeacherStudentsOnly(user.contactId);
                //if current user is a district admin, only show that district's students in the student dropdowns
                else if (user.permissionLevel.Contains('D') && !(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')))
                    showDistrictStudentsOnly(user.districtId);
            }
        }

        /*
         * Pre:  The current user's highest permission level is teacher
         * Post: The only student's shown in the student dropdowns belong to th current teacher
         * @param contactId is the id of the teachers whose students should be shown
         */
        private void showTeacherStudentsOnly(int contactId)
        {
            DataTable table = DbInterfaceStudent.GetStudentSearchResultsForTeacher("", "", "", contactId);

            ddlStudent1.DataSourceID = null;
            ddlStudent1.DataSource = table;
            ddlStudent1.DataBind();

            ddlStudent2.DataSourceID = null;
            ddlStudent2.DataSource = table;
            ddlStudent2.DataBind();
        }

        /*
         * Pre:  The current user's highest permission level is district admin
         * Post: The only student's shown in the student dropdowns belong to th current district admin
         * @param districtId is the id of the district to get students from
         */
        private void showDistrictStudentsOnly(int districtId)
        {
            DataTable table = DbInterfaceStudent.GetStudentSearchResults("", "", "", districtId);

            ddlStudent1.DataSourceID = null;
            ddlStudent1.DataSource = table;
            ddlStudent1.DataBind();

            ddlStudent2.DataSourceID = null;
            ddlStudent2.DataSource = table;
            ddlStudent2.DataBind();
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

                //get student data
                Student student1 = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(lblStudent1Id.Text));
                Student student2 = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(lblStudent2Id.Text));

                //get coordinate data
                if (student1 != null & student2 != null)
                {
                    StudentCoordinate coord1 = new StudentCoordinate(student1, reason, true, isDistrictAudition);
                    StudentCoordinate coord2 = new StudentCoordinate(student2, reason, true, isDistrictAudition);

                    //coordinate each audition between the two students
                    foreach (int i in coord1.auditionIds)
                    {
                        foreach (int j in coord2.auditionIds)
                            success = success && DbInterfaceStudentAudition.CreateAuditionCoordinate(i, j, reason);
                    }
                }
                else
                    success = false;
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

        /*
         * Pre:
         * Post: Returns true if the entered data is valid and false otherwise.
         *       Both students must be selected and a reason must be chosen
         */
        private bool dataIsValid()
        {
            bool valid = true;

            //make sure the students are different
            if (ddlStudent1.SelectedIndex == ddlStudent2.SelectedIndex)
            {
                showWarningMessage("The two students you have selected are the same.  Please choose a different student.");
                valid = false;
            }

            return valid;
        }

        //Show search for Student 1
        protected void btnStudent1Search_Click(object sender, EventArgs e)
        {
            pnlStudent1Search.Visible = true;
            btnStudent1Search.Visible = false;
        }

        //Show search for Student 2
        protected void btnStudent2Search_Click(object sender, EventArgs e)
        {
            pnlStudent2Search.Visible = true;
            btnStudent2Search.Visible = false;
        }

        /*
         * Pre:   
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudent1Search_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvStudent1Search.SelectedIndex;

            if (index >= 0 && index < gvStudent1Search.Rows.Count)
            {
                string id = gvStudent1Search.Rows[index].Cells[1].Text;
                string firstName = gvStudent1Search.Rows[index].Cells[2].Text;
                string lastName = gvStudent1Search.Rows[index].Cells[3].Text;

                //load student data to avoid the bug where ' shows up as &#39; if the data is just taken from the gridview
                Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                if (student != null)
                {
                    firstName = student.firstName;
                    lastName = student.lastName;
                }

                //load search fields
                txtStudent1Id.Text = id;
                txtFirstName1.Text =
                txtLastName1.Text =
                lblStudent1Id.Text = id;

                //select student from dropdown
                ddlStudent1.SelectedValue = id;

                //hide search area
                pnlStudent1Search.Visible = false;
                clearStudent1Search();
                btnStudent1Search.Visible = true;
            }
        }

        /*
         * Pre:   
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudent2Search_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvStudent2Search.SelectedIndex;

            if (index >= 0 && index < gvStudent2Search.Rows.Count)
            {
                string id = gvStudent2Search.Rows[index].Cells[1].Text;
                string firstName = gvStudent2Search.Rows[index].Cells[2].Text;
                string lastName = gvStudent2Search.Rows[index].Cells[3].Text;

                //load student data to avoid the bug where ' shows up as &#39; if the data is just taken from the gridview
                Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                if (student != null)
                {
                    firstName = student.firstName;
                    lastName = student.lastName;
                }

                //load search fields
                txtStudent2Id.Text = id;
                txtFirstName2.Text = firstName;
                txtLastName2.Text = lastName;
                lblStudent2Id.Text = id;

                //select student from dropdown
                ddlStudent2.SelectedValue = txtStudent2Id.Text;

                //hide search area
                pnlStudent2Search.Visible = false;
                clearStudent2Search();
                btnStudent2Search.Visible = true;
            }
        }

        /*
         * Pre:   
         * Post:  The page of gvStudent1Search is changed
         */
        protected void gvStudent1Search_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudent1Search.PageIndex = e.NewPageIndex;
            BindStudent1SessionData();
        }

        /*
         * Pre:   
         * Post:  The page of gvStudent2Search is changed
         */
        protected void gvStudent2Search_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudent2Search.PageIndex = e.NewPageIndex;
            BindStudent2SessionData();
        }

        /*
         * Pre:   The student 1 search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindStudent1SessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[student1Search];
                gvStudent1Search.DataSource = data;
                gvStudent1Search.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Coordinate Students", "BindStudent1SessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:   The student 2 search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindStudent2SessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[student2Search];
                gvStudent2Search.DataSource = data;
                gvStudent2Search.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Coordinate Students", "BindStudent2SessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvStudent1Search_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvStudent1Search, e);
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvStudent2Search_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvStudent2Search, e);
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
        protected void btnSearchStudent1_Click(object sender, EventArgs e)
        {
            string id = txtStudent1Id.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                if (userIsTeacherOnly()) //if the current user is a teacher, search only their students
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchOwnStudents(gvStudent1Search, id, txtFirstName1.Text, txtLastName1.Text, student1Search, ((User)Session[Utility.userRole]).contactId))
                    {
                        showInfoMessage("The search did not return any results.");
                    }
                }
                else //if current user is a district admin search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudent1Search, id, txtFirstName1.Text, txtLastName1.Text, student1Search, getDistrictIdForPermissionLevel()))
                    {
                        showInfoMessage("The search did not return any results.");
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudent1Search);
                showWarningMessage("A Student Id must be numeric");
            }
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students matching the search criteria are displayed (student id, first name, 
         *        and last name). The error message is also reset.
         */
        protected void btnSearchStudent2_Click(object sender, EventArgs e)
        {
            string id = txtStudent2Id.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                if (userIsTeacherOnly()) //if the current user is a teacher, search only their students
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchOwnStudents(gvStudent2Search, id, txtFirstName2.Text, txtLastName2.Text, student2Search, ((User)Session[Utility.userRole]).contactId))
                    {
                        showWarningMessage("The search did not return any results.");
                    }
                }
                else //if current user is a district admin search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudent2Search, id, txtFirstName2.Text, txtLastName2.Text, student2Search, getDistrictIdForPermissionLevel()))
                    {
                        showWarningMessage("The search did not return any results.");
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudent2Search);
                showWarningMessage("A Student Id must be numeric.");
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
            clearStudent1Search();
            clearStudent2Search();
            ddlStudent1.SelectedIndex = 0;
            ddlStudent2.SelectedIndex = 0;
            ddlReason.SelectedIndex = 0;
        }

        /*
         * Clear Student 1 Search section
         */
        protected void btnClearStudent1Search_Click(object sender, EventArgs e)
        {
            clearStudent1Search();
        }

        /*
         * Clear Student 2 Search section
         */
        protected void btnClearStudent2Search_Click(object sender, EventArgs e)
        {
            clearStudent2Search();
        }

        /*
         * Clear Student 1 Search section
         */
        private void clearStudent1Search()
        {
            txtStudent1Id.Text = "";
            txtFirstName1.Text = "";
            txtLastName1.Text = "";

            clearGridView(gvStudent1Search);
        }

        /*
         * Clear Student 2 Search section
         */
        private void clearStudent2Search()
        {
            txtStudent2Id.Text = "";
            txtFirstName2.Text = "";
            txtLastName2.Text = "";

            clearGridView(gvStudent2Search);
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
         * Post: Update the hidden student id based on the selected student
         */
        protected void ddlStudent1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStudent1Id.Text = ddlStudent1.SelectedValue;
        }
        protected void ddlStudent2_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStudent2Id.Text = ddlStudent2.SelectedValue;
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

        protected void ddlStudent2_SelectedIndexChanged1(object sender, EventArgs e)
        {
            lblStudent2Id.Text = ddlStudent2.SelectedValue;
        }

        protected void ddlStudent1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            lblStudent1Id.Text = ddlStudent1.SelectedValue;
        }
    }
}
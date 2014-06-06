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
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            bool success = true;

            clearErrors();

            if (dataIsValid())
            {
                string reason = ddlReason.SelectedValue;
                bool isDistrictAudition = ddlAuditionType.SelectedValue.Equals("District");

                //get student data
                Student student1 = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(lblStudent1Id.InnerText));
                Student student2 = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(lblStudent2Id.InnerText));

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
                displaySuccessMessageAndOptions();
            else
            {
                lblMainError.Text = "An error occurred while coordinating the students.";
                lblMainError.Visible = true;
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

            //make sure student 1 was chosen
            if (ddlStudent1.SelectedIndex <= 0)
            {
                lblStudent1Error.Visible = true;
                valid = false;
            }

            //make sure student 2 was chosen
            if (ddlStudent2.SelectedIndex <= 0)
            {
                lblStudent2Error.Visible = true;
                valid = false;
            }

            //make sure the students are different
            if (ddlStudent1.SelectedIndex == ddlStudent2.SelectedIndex)
            {
                lblMainError.Text = "Please make sure the two selected students are not the same";
                lblMainError.Visible = true;
                valid = false;
            }

            //make sure a reason was chosen
            if (ddlReason.SelectedIndex <= 0)
            {
                lblReasonError.Visible = true;
                valid = false;
            }

            //make sure an audition type was selected
            if (ddlAuditionType.SelectedIndex <= 0)
            {
                lblAudTypeError.Visible = true;
                valid = false;
            }

            if (!valid) lblErrorMsg.Visible = true;

            return valid;
        }

        //Displays a message telling the user that the operation was successful
        private void displaySuccessMessageAndOptions()
        {
            pnlFullPage.Visible = false;
            pnlSuccess.Visible = true;
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

            lblStudent1SearchError.Visible = false;

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
                lblStudent1Id.InnerText = id;

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

            lblStudent2SearchError.Visible = false;

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
                lblStudent2Id.InnerText = id;

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
                    cell.BackColor = Color.FromArgb(204, 204, 255);
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
                        lblStudent1SearchError.Visible = true;
                        lblStudent1SearchError.ForeColor = Color.DarkBlue;
                        lblStudent1SearchError.Text = "The search did not return any results";
                    }
                }
                else //if current user is a district admin search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudent1Search, id, txtFirstName1.Text, txtLastName1.Text, student1Search, getDistrictIdForPermissionLevel()))
                    {
                        lblStudent1SearchError.Visible = true;
                        lblStudent1SearchError.ForeColor = Color.DarkBlue;
                        lblStudent1SearchError.Text = "The search did not return any results";
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudent1Search);
                lblStudent1SearchError.Visible = true;
                lblStudent1SearchError.ForeColor = Color.Red;
                lblStudent1SearchError.Text = "A Student Id must be numeric";
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
                        lblStudent2SearchError.Visible = true;
                        lblStudent2SearchError.ForeColor = Color.DarkBlue;
                        lblStudent2SearchError.Text = "The search did not return any results";
                    }
                }
                else //if current user is a district admin search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudent2Search, id, txtFirstName2.Text, txtLastName2.Text, student2Search, getDistrictIdForPermissionLevel()))
                    {
                        lblStudent2SearchError.Visible = true;
                        lblStudent2SearchError.ForeColor = Color.DarkBlue;
                        lblStudent2SearchError.Text = "The search did not return any results";
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudent2Search);
                lblStudent2SearchError.Visible = true;
                lblStudent2SearchError.ForeColor = Color.Red;
                lblStudent2SearchError.Text = "A Student Id must be numeric";
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
                    if (gridView == gvStudent1Search)
                    {
                        lblStudent1SearchError.Text = "An error occurred during the search";
                        lblStudent1SearchError.Visible = true;
                    }
                    else
                    {
                        lblStudent2SearchError.Text = "An error occurred during the search";
                        lblStudent2SearchError.Visible = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (gridView == gvStudent1Search)
                {
                    lblStudent1SearchError.Text = "An error occurred during the search";
                    lblStudent1SearchError.Visible = true;
                }
                else
                {
                    lblStudent2SearchError.Text = "An error occurred during the search";
                    lblStudent2SearchError.Visible = true;
                }

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
                    if (gridView == gvStudent1Search)
                    {
                        lblStudent1SearchError.Text = "An error occurred during the search";
                        lblStudent1SearchError.Visible = true;
                    }
                    else
                    {
                        lblStudent2SearchError.Text = "An error occurred during the search";
                        lblStudent2SearchError.Visible = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (gridView == gvStudent1Search)
                {
                    lblStudent1SearchError.Text = "An error occurred during the search";
                    lblStudent1SearchError.Visible = true;
                }
                else
                {
                    lblStudent2SearchError.Text = "An error occurred during the search";
                    lblStudent2SearchError.Visible = true;
                }

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
         * Clear the page so the user can add a new coordinate
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            clearPage();
            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;
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
            clearErrors();
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
            lblStudent1SearchError.Visible = false;
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
            lblStudent2SearchError.Visible = false;
            txtStudent2Id.Text = "";
            txtFirstName2.Text = "";
            txtLastName2.Text = "";

            clearGridView(gvStudent2Search);
        }

        /*
         * Clear error messages on page
         */
        private void clearErrors()
        {
            lblMainError.Visible = false;
            lblErrorMsg.Visible = false;
            lblStudent1Error.Visible = false;
            lblStudent2Error.Visible = false;
            lblStudent1SearchError.Visible = false;
            lblStudent2SearchError.Visible = false;
            lblReasonError.Visible = false;
            lblAudTypeError.Visible = false;
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
         * Post: Bring the user back to the main screen
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
         * Pre:
         * Post: Update the hidden student id based on the selected student
         */
        protected void ddlStudent1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStudent1Id.InnerText = ddlStudent1.SelectedValue;
        }
        protected void ddlStudent2_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStudent2Id.InnerText = ddlStudent2.SelectedValue;
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

            //show error label
            lblMainError.Text = "An error occurred";
            lblMainError.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
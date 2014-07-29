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
    public partial class ManageStudents : System.Web.UI.Page
    {
        private Student student;
        private Utility.Action action;
        //Session variables
        private string studentSearch = "StudentData";
        private string studentVar = "Student";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
            initializePage();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[studentSearch] = null;
                Session[studentVar] = null;
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[studentVar] != null)
                student = (Student)Session[studentVar];
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
         * Post: Initialize the page for adding, editing, or deleting based on user selection
         */
        protected void initializePage()
        {
            //get requested action - default to adding
            string actionIndicator = Request.QueryString["action"];
            if (actionIndicator == null || actionIndicator.Equals(""))
            {
                action = Utility.Action.Add;
            }
            else
            {
                action = (Utility.Action)Convert.ToInt32(actionIndicator);
            }

            //initialize page based on action
            if (action == Utility.Action.Add)
            {
                pnlFullPage.Visible = true;
                pnlStudentSearch.Visible = false;
                lblLegacyPoints.Visible = true;
                txtLegacyPoints.Visible = true;
                enableControls();

                filterDistrictsAndTeachers();
            }
            else if (action == Utility.Action.Edit)
            {
                pnlStudentSearch.Visible = true;
                pnlFullPage.Visible = true;
                lblLegacyPoints.Visible = false;
                txtLegacyPoints.Visible = false;
                pnlButtons.Visible = false;
                enableControls();

                filterDistrictsAndTeachers();
            }
            else if (action == Utility.Action.Delete)
            {
                pnlStudentSearch.Visible = true;
                pnlFullPage.Visible = false;
                lblLegacyPoints.Visible = false;
                txtLegacyPoints.Visible = false;
                pnlButtons.Visible = false;
                disableControls();

                btnSubmit.Attributes.Add("onclick", "return confirm('Are you sure that you wish to permanently delete this student and all associated auditions and data?');");
            }
        }

        /*
         * Pre:
         * Post: If the user has not yet added a new student, clicking this 
         *       button will attempt to add a new student to the database
         *       using the supplied information.  If the user added a new
         *       student this button will clear the form and allow the user
         *       to add another new student.
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //add new student
            if (action == Utility.Action.Add)
            {
                if (AddNewStudent())
                {
                    clearData();
                }
            }
            else if (action == Utility.Action.Edit)
            {
                if (EditExistingStudent())
                {
                    clearData();
                    clearStudentSearch();
                    showSuccessMessage("The student information was successfully updated");
                }
            }
            else if (action == Utility.Action.Delete)
            {
                if (!txtFirstName.Text.Equals("") && !txtLastName.Text.Equals("") && !lblId.Text.Equals(""))
                    DeleteStudent();
            }
        }

        /*
         * Pre:
         * Post: If the user has entered valid information, the new student information is added
         *       to the database.  If no previous teacher id is entered, the value is set to "0".
         *       If the add is successful, the new student's id number will be displayed to the user,
         *       otherwise an error message will take its place.
         */
        private bool AddNewStudent()
        {
            bool result = true;

            //if the entered information is valid, add the new student
            if (verifyInformation())
            {
                int districtId, legacyPoints = 0, currTeacherId, prevTeacherId = 0;
                string firstName, middleInitial, lastName, grade;

                firstName = txtFirstName.Text;
                middleInitial = txtMiddleInitial.Text;
                lastName = txtLastName.Text;
                grade = txtGrade.Text;
                districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
                currTeacherId = Convert.ToInt32(cboCurrTeacher.SelectedValue);
                if (!cboPrevTeacher.SelectedValue.ToString().Equals("")) prevTeacherId = Convert.ToInt32(cboPrevTeacher.SelectedValue);
                if (!txtLegacyPoints.Text.Equals("")) legacyPoints = Convert.ToInt32(txtLegacyPoints.Text);

                //check for duplicate students
                DataTable duplicateStudentsTbl = DbInterfaceStudent.StudentExists(firstName, lastName);

                //call Javascript function to ask user if they want to add the duplicate student if any are found
                bool addDuplicate = true;
                if (duplicateStudentsTbl != null && duplicateStudentsTbl.Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ConfirmDuplicate", "confirmDuplicate()", true);

                    if (lblConfirmDuplicate.InnerText.Equals("false"))
                        addDuplicate = false;
                }

                //if there were no matching students or the user wants to add the duplicate, add
                if (addDuplicate || duplicateStudentsTbl == null || duplicateStudentsTbl.Rows.Count <= 0)
                {
                    Student newStudent = new Student(-1, firstName, middleInitial, lastName, grade, districtId,
                                             currTeacherId, prevTeacherId);
                    newStudent.legacyPoints = legacyPoints;

                    //add to database
                    newStudent.addToDatabase();

                    //get new id
                    if (newStudent.id != -1)
                        showSuccessMessage("The new student was successfully added and has an id of " + newStudent.id.ToString());
                    else
                    {
                        showErrorMessage("Error: There was an error adding the new student.");
                        result = false;
                    }
                }
                else 
                {
                    result = false;
                }
            }
            else
                result = false;

            return result;
        }

        /*
         * Pre:
         * Post: If the user has entered valid information, the  student information is edited
         *       in the database.  If no previous teacher id is entered, the value is set to "0".
         */
        private bool EditExistingStudent()
        {
            bool result = true;

            //if the entered information is valid, edit the student information
            if (verifyInformation())
            {
                int studentId, districtId, currTeacherId, prevTeacherId = 0;
                string firstName, middleInitial, lastName, grade;

                studentId = Convert.ToInt32(lblId.Text);
                firstName = txtFirstName.Text;
                middleInitial = txtMiddleInitial.Text;
                lastName = txtLastName.Text;
                grade = txtGrade.Text;
                districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
                currTeacherId = Convert.ToInt32(cboCurrTeacher.SelectedValue);
                if (!cboPrevTeacher.SelectedValue.ToString().Equals("")) prevTeacherId = Convert.ToInt32(cboPrevTeacher.SelectedValue);

                student = new Student(studentId, firstName, middleInitial, lastName, grade, districtId, currTeacherId, prevTeacherId);

                if (!student.editDatabaseInformation())
                {
                   showErrorMessage("Error: There was an error updating the student's information");
                }
            }
            else
                result = false;

            return result;
        }

        /*
         * Pre:  A student must have been selected previously
         * Post: The selected student and all associated data is deleted from the system
         */
        private void DeleteStudent()
        {
            int studentId, districtId, currTeacherId, prevTeacherId = 0;
            string firstName, middleInitial, lastName, grade;

            studentId = Convert.ToInt32(lblId.Text);
            firstName = txtFirstName.Text;
            middleInitial = txtMiddleInitial.Text;
            lastName = txtLastName.Text;
            grade = txtGrade.Text;
            districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
            currTeacherId = Convert.ToInt32(cboCurrTeacher.SelectedValue);
            if (!cboPrevTeacher.SelectedValue.ToString().Equals("")) prevTeacherId = Convert.ToInt32(cboPrevTeacher.SelectedValue);

            student = new Student(studentId, firstName, middleInitial, lastName, grade, districtId, currTeacherId, prevTeacherId);

            if (student != null && !student.deleteDatabaseInformation())
            {
                showErrorMessage("Error: There was an error deleting the student's information");
            }
            else
            {
                clearData();
                clearStudentSearch();
                pnlFullPage.Visible = false;
                pnlStudentSearch.Visible = true;
                pnlButtons.Visible = false;
                showSuccessMessage("The student information was successfully deleted.");
            }
        }

        /*
         * Pre:
         * Post: Verifies that the information entered by the user 
         *       is in the correct format
         */
        private bool verifyInformation()
        {
            bool valid = true, previousError = false;

            if (txtMiddleInitial.Text.Length > 2)
            {
                showWarningMessage("The Middle Initial may not be more than 2 characters in length.");
                valid = false;
                previousError = true;
            }

            //make sure the grade is 1-12, Kindergarten, or Adult
            bool isNum;
            int grade;
            isNum = Int32.TryParse(txtGrade.Text, out grade);
            if (isNum && (grade < 1 || grade > 12))
            {
                if (!previousError) 
                    showWarningMessage("Please enter a valid grade (use A for adult).");

                valid = false;
                previousError = true;
            }
            else if (!isNum && !(txtGrade.Text.Equals("K") || txtGrade.Text.Equals("A") ||
                       txtGrade.Text.Equals("Kindergarten") || txtGrade.Text.Equals("Adult")))
            {
                if (!previousError) 
                    showWarningMessage("Please enter a valid grade (use A for adult)");

                valid = false;
                previousError = true;
            }

            if (!txtLegacyPoints.Text.Equals("") && Convert.ToInt32(txtLegacyPoints.Text) < 0)
            {
                if (!previousError) 
                    showWarningMessage("Legacy Points must be greater than or equal to 0");

                valid = false;
                previousError = true;
            }

            return valid;
        }

        /*
         * Pre:
         * Post:  Clears entered data, error messages, and error
         *        highlighting on the page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
            clearStudentSearch();
        }

        /*
         * Pre:
         * Post:  Clears entered data, error messages, and error
         *        highlighting on the page
         */
        private void clearData()
        {
            //clear text and selections
            txtFirstName.Text = "";
            txtMiddleInitial.Text = "";
            txtLastName.Text = "";
            txtGrade.Text = "";
            ddlDistrict.SelectedIndex = -1;
            cboCurrTeacher.SelectedIndex = -1;
            cboPrevTeacher.SelectedIndex = -1;
            txtLegacyPoints.Text = "0";

            btnClear.Visible = true;

            Session[studentSearch] = null;
            Session[studentVar] = null;

            if (action != Utility.Action.Add)
            {
                pnlButtons.Visible = false;
                pnlFullPage.Visible = false;
                pnlStudentSearch.Visible = true;
            }
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students matching the search criteria are displayed (student id, first name, 
         *        and last name). The error message is also reset.
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtStudentId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                if (userIsTeacherOnly()) //if the current user is a teacher, search only their students
                {
                    if (!searchOwnStudents(gvStudentSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, studentSearch, ((User)Session[Utility.userRole]).contactId))
                    {
                        showInfoMessage("The search did not return any results.");
                    }
                }
                else //if the current user is a district admin, search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudentSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, studentSearch, getDistrictIdForPermissionLevel()))
                    {
                        showInfoMessage("The search did not return any results.");
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudentSearch);
                showWarningMessage("A Student Id must be numeric.");
            }
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
         * Pre:
         * Post: Determines whether the current user's highest permission level is 'Teacher'
         * @returns true if the user's highest permission level is 'Teacher' and false otherwise
         */
        private bool userIsTeacherOnly()
        {
            User user = (User)Session[Utility.userRole];

            return user.permissionLevel.Contains('T') && !user.permissionLevel.Contains('D') && !user.permissionLevel.Contains('S') && !user.permissionLevel.Contains('A');
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students.  Matching student 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param districtId is the district of students that should be searched
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
                    showErrorMessage("Error: An error occurred during the search");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search");

                Utility.LogError("Manage Students", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
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
                    showErrorMessage("Error: An error occurred during the search");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search");

                Utility.LogError("Manage Students", "searchOwnStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Pre:
         * Post: Clears data in the student search section
         */
        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            clearStudentSearch();
        }

        /*
         * Pre:   
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlFullPage.Visible = true;
            pnlButtons.Visible = true;
           
            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                txtStudentId.Text = gvStudentSearch.Rows[index].Cells[1].Text;

                Student student = loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));
                lblId.Text = student.id.ToString();

                Session[studentVar] = student;
            }
        }

        /*
         * Pre:  txtStudentId must contain a student id that exists in the system
         * Post: The student's information is loaded to the page
         */
        private Student loadStudentData(int id)
        {
            Student student = DbInterfaceStudent.LoadStudentData(id);

            //get general student information
            if (student != null)
            {
                //load rest of search fields
                txtFirstNameSearch.Text = student.firstName;
                txtLastNameSearch.Text = student.lastName;

                //bind dropdowns in case it hasn't been done yet
                ddlDistrict.DataBind();
                cboCurrTeacher.DataBind();
                cboPrevTeacher.DataBind();

                //load student data
                lblId.Text = id.ToString();
                txtFirstName.Text = student.firstName;
                txtMiddleInitial.Text = student.middleInitial;
                txtLastName.Text = student.lastName;
                ddlDistrict.SelectedIndex = ddlDistrict.Items.IndexOf(ddlDistrict.Items.FindByValue(student.districtId.ToString()));
                txtGrade.Text = student.grade;
                cboCurrTeacher.SelectedIndex = cboCurrTeacher.Items.IndexOf(cboCurrTeacher.Items.FindByValue(student.currTeacherId.ToString()));
                cboPrevTeacher.SelectedIndex = cboPrevTeacher.Items.IndexOf(cboPrevTeacher.Items.FindByValue(student.prevTeacherId.ToString()));
            }
            else
            {
                showErrorMessage("Error: The student's information could not be loaded");
            }

            return student;
        }

        /*
         * Pre:   
         * Post:  The page of gvStudentSearch is changed
         */
        protected void gvStudentSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudentSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        /*
         * Pre:   The student search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[studentSearch];
                gvStudentSearch.DataSource = data;
                gvStudentSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Manage Students", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
         * Pre:
         * Post: Clears the Student Search section
         */
        private void clearStudentSearch()
        {
            txtStudentId.Text = "";
            txtFirstNameSearch.Text = "";
            txtLastNameSearch.Text = "";
            gvStudentSearch.DataSource = null;
            gvStudentSearch.DataBind();
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
         * Post:  If the current user is a teacher they can only add/edit their own students.
         *        If the current user is a district administrator they can only add/edit students
         *        in their district
         */
        private void filterDistrictsAndTeachers()
        {
            //filter available districts and teachers based on user role
            User user = (User)Session[Utility.userRole];

            //if the user is a teacher, only allow students to be added to their district and assigned to themselves
            if (user.permissionLevel.Contains('T') && !(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')
                || user.permissionLevel.Contains('D')))
            {
                //only show current teacher's district
                ddlDistrict.DataBind();
                ListItem item = ddlDistrict.Items.FindByValue(user.districtId.ToString());
                ddlDistrict.Items.Clear();
                ddlDistrict.Items.Add(item);

                //only show current teacher in teacher dropdown
                cboCurrTeacher.DataBind();
                item = cboCurrTeacher.Items.FindByValue(user.contactId.ToString());
                cboCurrTeacher.Items.Clear();
                cboCurrTeacher.Items.Add(item);
            }
            //if the user is a district admin, allow them to add students to their district to and assign them to a teacher in their district
            else if (user.permissionLevel.Contains('D') && !(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')))
            {
                //only show current user's district
                ddlDistrict.DataBind();
                ListItem item = ddlDistrict.Items.FindByValue(user.districtId.ToString());
                ddlDistrict.Items.Clear();
                ddlDistrict.Items.Add(item);

                //only show teachers in the current user's district
                DataTable table = DbInterfaceContact.GetTeachersFromDistrict(user.districtId);
                cboCurrTeacher.DataSourceID = null;
                cboCurrTeacher.DataSource = table;
                cboCurrTeacher.DataBind();
            }
        }

        /*
         * Pre:
         * Post: All controls on the page are disabled
         */
        private void disableControls()
        {
            txtFirstName.Enabled = false;
            txtMiddleInitial.Enabled = false;
            txtLastName.Enabled = false;
            txtGrade.Enabled = false;
            ddlDistrict.Enabled = false;
            cboCurrTeacher.Enabled = false;
            cboPrevTeacher.Enabled = false;
        }

        /*
         * Pre:
         * Post: All controls on the page are enabled
         */
        private void enableControls()
        {
            txtFirstName.Enabled = true;
            txtMiddleInitial.Enabled = true;
            txtLastName.Enabled = true;
            txtGrade.Enabled = true;
            ddlDistrict.Enabled = true;
            cboCurrTeacher.Enabled = true;
            cboPrevTeacher.Enabled = true;
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Manage Students", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }

        /*
         * Pre:
         * Post: Displays the input error message in the top-left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showErrorMessage(string message)
        {
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowError", "showMainError(" + message + ")", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowMainError", "showMainError(" + message + ")", true);
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
    }
}
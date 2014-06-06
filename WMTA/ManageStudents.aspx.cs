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
    public partial class ManageStudents : System.Web.UI.Page
    {
        Student student;
        bool checkedForDuplicate = false;
        //Session variables
        private string creatingNew = "CreatingNew"; //tracks whether a student is being created or edited
        private string deleting = "Deleting";       //tracks whether a student is being deleted
        private string studentSearch = "StudentData";
        private string studentVar = "Student";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[creatingNew] = null;
                Session[deleting] = null;
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
            else
            {
                User user = (User)Session[Utility.userRole];

                //allow user to delete student and add duplicates if they are a system administrator
                if (user.permissionLevel.Contains("A") && ddlUserOptions.Items.Count <= 2)
                    ddlUserOptions.Items.Add(new ListItem("Delete Existing Student", "Delete Existing"));
                else if (user.permissionLevel.Contains("A"))
                    btnAddAnyways.Visible = true;
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
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            clearErrors();

            //add new student
            if (btnAdd.Text.Equals("Add"))
            {
                if (AddNewStudent())
                {
                    btnAdd.Text = "New";
                    btnClear.Visible = false;
                }
            }
            else if (btnAdd.Text.Equals("Submit") && !(bool)Session[creatingNew] && !(bool)Session[deleting])
            {
                EditExistingStudent();

                if (!lblError.Visible)
                {
                    clearData();
                    clearStudentSearch();
                    pnlFullPage.Visible = false;
                    pnlStudentSearch.Visible = false;
                    pnlButtons.Visible = false;
                    pnlSuccess.Visible = true;
                    lblSuccess.Text = "The student information was successfully updated";
                    lblSuccess.Visible = true;
                }
            }
            else if (btnAdd.Text.Equals("Submit") && (bool)Session[deleting])
            {
                if (!txtFirstName.Text.Equals("") && !txtLastName.Text.Equals("") && !lblId.Text.Equals(""))
                    DeleteStudent();
            }
            //clear data so user can enter another new student
            else if (btnAdd.Text.Equals("New"))
            {
                clearData();
                btnAdd.Text = "Add";
                btnClear.Visible = true;
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
                districtId = Convert.ToInt32(cboDistrict.SelectedValue);
                currTeacherId = Convert.ToInt32(cboCurrTeacher.SelectedValue);
                if (!cboPrevTeacher.SelectedValue.ToString().Equals("")) prevTeacherId = Convert.ToInt32(cboPrevTeacher.SelectedValue);
                if (!txtLegacyPoints.Text.Equals("")) legacyPoints = Convert.ToInt32(txtLegacyPoints.Text);

                //check for duplicate students
                DataTable duplicateStudentsTbl = DbInterfaceStudent.StudentExists(firstName, lastName);
                if (checkedForDuplicate || duplicateStudentsTbl == null || duplicateStudentsTbl.Rows.Count <= 0)
                {
                    Student newStudent = new Student(-1, firstName, middleInitial, lastName, grade, districtId,
                                             currTeacherId, prevTeacherId);
                    newStudent.legacyPoints = legacyPoints;

                    //add to database
                    newStudent.addToDatabase();

                    //get new id
                    if (newStudent.id != -1)
                        lblId.Text = newStudent.id.ToString();
                    else
                    {
                        lblError.Text = "There was an error adding the new student";
                        lblError.Visible = true;
                        lblId.Text = "Error Adding Student";
                        result = false;
                    }

                    checkedForDuplicate = false;
                }
                else if (!checkedForDuplicate)
                {
                    pnlDuplicateStudent.Visible = true;
                    pnlFullPage.Visible = false;
                    pnlButtons.Visible = false;
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
                districtId = Convert.ToInt32(cboDistrict.SelectedValue);
                currTeacherId = Convert.ToInt32(cboCurrTeacher.SelectedValue);
                if (!cboPrevTeacher.SelectedValue.ToString().Equals("")) prevTeacherId = Convert.ToInt32(cboPrevTeacher.SelectedValue);

                student = new Student(studentId, firstName, middleInitial, lastName, grade, districtId, currTeacherId, prevTeacherId);

                if (!student.editDatabaseInformation())
                {
                    lblError.Text = "There was an error updating the student's information";
                    lblError.Visible = true;
                }
            }
            else
                result = false;

            return result;
        }

        /*
         * Pre:
         * Post: If a student has been chosen the user is asked to confirm if they wish 
         *       to delete the student and all associated information
         */
        private void DeleteStudent()
        {
            pnlStudentSearch.Visible = false;
            pnlFullPage.Visible = false;
            pnlButtons.Visible = false;
            pnlAreYouSure.Visible = true;
            lblWarning.Text = "Are you sure that you want to permanently delete the data of " +
                              txtFirstName.Text + " " + txtLastName.Text + "?  All of the student's " +
                              "associated information will also be deleted including awards, coordinate " +
                              "student records, compositions, points, and auditions.  This action cannot " +
                              "be undone.";
        }

        /*
         * Pre:  A student must have been selected previously
         * Post: The selected student and all associated data is deleted from the system
         */
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int studentId, districtId, currTeacherId, prevTeacherId = 0;
            string firstName, middleInitial, lastName, grade;

            studentId = Convert.ToInt32(lblId.Text);
            firstName = txtFirstName.Text;
            middleInitial = txtMiddleInitial.Text;
            lastName = txtLastName.Text;
            grade = txtGrade.Text;
            districtId = Convert.ToInt32(cboDistrict.SelectedValue);
            currTeacherId = Convert.ToInt32(cboCurrTeacher.SelectedValue);
            if (!cboPrevTeacher.SelectedValue.ToString().Equals("")) prevTeacherId = Convert.ToInt32(cboPrevTeacher.SelectedValue);

            student = new Student(studentId, firstName, middleInitial, lastName, grade, districtId, currTeacherId, prevTeacherId);

            if (!student.deleteDatabaseInformation())
            {
                lblError.Text = "There was an error deleting the student's information";
                lblError.Visible = true;
            }
            else
            {
                if (!lblError.Visible)
                {
                    clearData();
                    clearStudentSearch();
                    pnlFullPage.Visible = false;
                    pnlStudentSearch.Visible = false;
                    pnlButtons.Visible = false;
                    pnlAreYouSure.Visible = false;
                    pnlSuccess.Visible = true;
                    lblSuccess.Text = "The student information was successfully deleted.";
                    lblSuccess.Visible = true;
                }
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
            clearErrors();

            if (txtFirstName.Text.Equals(""))
            {
                lblFirstNameError.Visible = true;
                valid = false;
                previousError = true;
            }

            if (txtMiddleInitial.Text.Length > 2)
            {
                lblMiddleNameError.Visible = true;
                valid = false;
                previousError = true;
            }

            if (txtLastName.Text.Equals(""))
            {
                lblLastNameError.Visible = true;
                valid = false;
                previousError = true;
            }

            //make sure the grade is 1-12, Kindergarten, or Adult
            bool isNum;
            int grade;
            isNum = Int32.TryParse(txtGrade.Text, out grade);
            if (isNum && (grade < 1 || grade > 12))
            {
                if (!previousError) lblError.Text = "Please enter a valid grade (use A for adult)";

                lblGradeError.Visible = true;
                valid = false;
                previousError = true;
            }
            else if (!isNum && !(txtGrade.Text.Equals("K") || txtGrade.Text.Equals("A") ||
                       txtGrade.Text.Equals("Kindergarten") || txtGrade.Text.Equals("Adult")))
            {
                if (!previousError) lblError.Text = "Please enter a valid grade (use A for adult)";

                lblGradeError.Visible = true;
                valid = false;
                previousError = true;
            }

            if (cboDistrict.SelectedValue.ToString().Equals(""))
            {
                lblDistrictError.Visible = true;
                valid = false;
                previousError = true;
            }

            if (cboCurrTeacher.SelectedValue.ToString().Equals(""))
            {
                lblCurrTeacherError.Visible = true;
                valid = false;
                previousError = true;
            }

            if (!txtLegacyPoints.Text.Equals("") && Convert.ToInt32(txtLegacyPoints.Text) < 0)
            {
                if (!previousError) lblError.Text = "Legacy Points must be greater than or equal to 0";

                txtLegacyPoints.BorderColor = Color.Red;
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
            cboDistrict.SelectedIndex = -1;
            cboCurrTeacher.SelectedIndex = -1;
            cboPrevTeacher.SelectedIndex = -1;
            txtLegacyPoints.Text = "0";
            lblId.Text = "";

            btnAdd.Text = "Add";
            btnClear.Visible = true;
            clearErrors();

            //if (Session[creatingNew] != null && !(bool)Session[creatingNew])
            //    pnlFullPage.Visible = false;

            //Session[creatingNew] = null;
            //Session[deleting] = null;
            Session[studentSearch] = null;
            Session[studentVar] = null;

            checkedForDuplicate = false;
        }

        /*
         * Pre:
         * Post: Clears error messages and highlighting on the page
         */
        private void clearErrors()
        {
            //clear error message
            lblError.Text = "";

            //clear error highlighting
            lblFirstNameError.Visible = false;
            lblMiddleNameError.Visible = false;
            lblLastNameError.Visible = false;
            lblGradeError.Visible = false;
            lblDistrictError.Visible = false;
            lblCurrTeacherError.Visible = false;
            txtLegacyPoints.BorderColor = Color.Empty;
        }

        /*
         * Pre:
         * Post: The user is redirected to the Welcome Screen
         */
        protected void btnMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students matching the search criteria are displayed (student id, first name, 
         *        and last name). The error message is also reset.
         */
        protected void btnStudentSearch_Click(object sender, EventArgs e)
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
                        lblStudentSearchError.Visible = true;
                        lblStudentSearchError.ForeColor = Color.DarkBlue;
                        lblStudentSearchError.Text = "The search did not return any results";
                    }
                }
                else //if the current user is a district admin, search only their district, otherwise search whole state
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudentSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, studentSearch, getDistrictIdForPermissionLevel()))
                    {
                        lblStudentSearchError.Visible = true;
                        lblStudentSearchError.ForeColor = Color.DarkBlue;
                        lblStudentSearchError.Text = "The search did not return any results";
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvStudentSearch);
                lblStudentSearchError.Visible = true;
                lblStudentSearchError.ForeColor = Color.Red;
                lblStudentSearchError.Text = "A Student Id must be numeric";
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
                    lblStudentSearchError.Text = "An error occurred during the search";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search";
                lblStudentSearchError.Visible = true;

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
                    lblStudentSearchError.Text = "An error occurred during the search";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search";
                lblStudentSearchError.Visible = true;

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
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
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
            lblStudentSearchError.Visible = false;
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

                //load student data
                lblId.Text = id.ToString();
                txtFirstName.Text = student.firstName;
                txtMiddleInitial.Text = student.middleInitial;
                txtLastName.Text = student.lastName;
                cboDistrict.SelectedIndex = cboDistrict.Items.IndexOf(cboDistrict.Items.FindByValue(student.districtId.ToString()));
                txtGrade.Text = student.grade;
                cboCurrTeacher.SelectedIndex = cboCurrTeacher.Items.IndexOf(cboCurrTeacher.Items.FindByValue(student.currTeacherId.ToString()));
                cboPrevTeacher.SelectedIndex = cboPrevTeacher.Items.IndexOf(cboPrevTeacher.Items.FindByValue(student.prevTeacherId.ToString()));
            }
            else
            {
                lblStudentSearchError.Text = "The student's information could not be loaded";
                lblStudentSearchError.Visible = true;
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
                    cell.BackColor = Color.FromArgb(204, 204, 255);
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
            lblStudentSearchError.Visible = false;
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
         * Post: No student information is changed and the user is brought back
         *       to the main screen
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: Initialize the page for adding, editing, or deleting
         *       based on user selection
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            pnlSuccess.Visible = false;
            pnlButtons.Visible = true;

            if (ddlUserOptions.SelectedValue.Equals("Create New"))
            {
                Session[creatingNew] = true;
                Session[deleting] = false;
                pnlFullPage.Visible = true;
                pnlStudentSearch.Visible = false;
                lblLegacyPoints.Visible = true;
                txtLegacyPoints.Visible = true;
                header.InnerText = "Add New Student";
                btnAdd.Text = "Add";
                enableControls();

                filterDistrictsAndTeachers();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Edit Existing"))
            {
                Session[creatingNew] = false;
                Session[deleting] = false;
                pnlStudentSearch.Visible = true;
                pnlFullPage.Visible = true;
                lblLegacyPoints.Visible = false;
                txtLegacyPoints.Visible = false;
                header.InnerText = "Edit Existing Student";
                btnAdd.Text = "Submit";
                enableControls();

                filterDistrictsAndTeachers();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Delete Existing"))
            {
                Session[creatingNew] = false;
                Session[deleting] = true;
                pnlStudentSearch.Visible = true;
                pnlFullPage.Visible = true;
                lblLegacyPoints.Visible = false;
                txtLegacyPoints.Visible = false;
                header.InnerText = "Delete Existing Student";
                btnAdd.Text = "Submit";
                disableControls();
            }
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
                cboDistrict.DataBind();
                ListItem item = cboDistrict.Items.FindByValue(user.districtId.ToString());
                cboDistrict.Items.Clear();
                cboDistrict.Items.Add(item);

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
                cboDistrict.DataBind();
                ListItem item = cboDistrict.Items.FindByValue(user.districtId.ToString());
                cboDistrict.Items.Clear();
                cboDistrict.Items.Add(item);

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
            cboDistrict.Enabled = false;
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
            cboDistrict.Enabled = true;
            cboCurrTeacher.Enabled = true;
            cboPrevTeacher.Enabled = true;
        }

        /*
         * Pre:
         * Post: No additional changes are made and the user is brought 
         *       back to the welcome/option screen
         */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /** clear errors **/
        protected void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            lblFirstNameError.Visible = false;
        }
        protected void txtMiddleInitial_TextChanged(object sender, EventArgs e)
        {
            lblMiddleNameError.Visible = false;
        }
        protected void txtLastName_TextChanged(object sender, EventArgs e)
        {
            lblLastNameError.Visible = false;
        }
        protected void txtGrade_TextChanged(object sender, EventArgs e)
        {
            lblGradeError.Visible = false;
        }
        protected void cboDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDistrictError.Visible = false;
        }
        protected void cboCurrTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCurrTeacherError.Visible = false;
        }

        /*
         * Pre:
         * Post: The student is not deleted and the confirmation panel is hidden
         */
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            pnlStudentSearch.Visible = true;
            pnlFullPage.Visible = true;
            pnlButtons.Visible = true;
            pnlAreYouSure.Visible = false;
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

            //show error label
            lblError.Text = "An error occurred";
            lblError.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }

        /*
         * Pre:  A duplicate name was detected and the user is being asked to
         *       confirm that they want to add the new student
         * Post: The new student is added
         */
        protected void btnAddAnyways_Click(object sender, EventArgs e)
        {
            checkedForDuplicate = true;

            if (AddNewStudent())
            {
                pnlFullPage.Visible = true;
                pnlButtons.Visible = true;
                pnlDuplicateStudent.Visible = false;

                btnAdd.Text = "New";
                btnClear.Visible = false;
            }
        }

        /*
         * Pre:  A duplicate name was detected and the user is being asked
         *       to confirm that they want to add the new student
         * Post: The new student is not added
         */
        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            checkedForDuplicate = false;

            pnlDuplicateStudent.Visible = false;
            pnlFullPage.Visible = true;
            pnlButtons.Visible = true;
        }
    }
}
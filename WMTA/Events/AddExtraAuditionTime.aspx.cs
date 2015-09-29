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
    public partial class AddExtraAuditionTime : System.Web.UI.Page
    {
        //session variables
        private string studentSearch = "StudentData";

        protected void Page_Load(object sender, EventArgs e)
        {
            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[studentSearch] = null;

                checkPermissions();
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

                // Give access to district, state, and system admins
                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("S") || user.permissionLevel.Contains("A")))
                    Response.Redirect("/Default.aspx");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (rblLength.SelectedIndex >= 0 && cboAudition.SelectedIndex > 0)
            {
                int auditionId = Convert.ToInt32(cboAudition.SelectedValue);

                if (DbInterfaceStudentAudition.UpdateAuditionLength(auditionId, Convert.ToInt32(rblLength.SelectedValue)))
                {
                    ClearPage();
                    showSuccessMessage("The audition length was successfully updated.");
                }
                else
                    showErrorMessage("The audition length could not be updated.");
            }
            else
                showInfoMessage("No changes were made to the audition length.");
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
                User user = (User)Session[Utility.userRole];
                int districtId = -1;

                //if district admin get their district because that is all the students they can register
                if (!chkMyStudentsOnly.Checked && (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')) && user.permissionLevel.Contains('D')))
                {
                    districtId = user.districtId;

                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, districtId))
                    {
                        displayStudentSearchError();
                    }
                }
                else if (chkMyStudentsOnly.Checked || (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S') ||
                                user.permissionLevel.Contains('D')) && user.permissionLevel.Contains('T')))
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchOwnStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, ((User)Session[Utility.userRole]).contactId))
                    {
                        displayStudentSearchError();
                    }
                }
                else if (!chkMyStudentsOnly.Checked && (user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')))
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, districtId))
                    {
                        displayStudentSearchError();
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
         * Post: Display message telling user that there were no search results
         */
        private void displayStudentSearchError()
        {
            showInfoMessage("The search did not return any results");
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students.  Matching student 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param session is the name of the session variable containing the student search table data
         * @param districtId is the id of the district in which to search students, -1 indicates all districts
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

                Utility.LogError("Add Extra Audition Time", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
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
                    showErrorMessage("Error: An error occurred during the search.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("Add Extra Audition Time", "searchOwnStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
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
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvStudentSearch.SelectedIndex;
            int year = DateTime.Today.Year;

            ClearAllExceptSearch();

            // Get audition year
            if (DateTime.Today.Month >= 6) year = year + 1;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                txtStudentId.Text = gvStudentSearch.Rows[index].Cells[1].Text;

                Student student = LoadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));

                if (student.grade.Equals("11") || student.grade.Equals("12"))
                {
                    upStudentSearch.Visible = false;
                    pnlButtons.Visible = true;
                    pnlInfo.Visible = true;
                }
            }
        }

        /*
         * Pre:  studentId must exist as a StudentId in the system
         * Post: The existing data for the student associated to the studentId 
         *       is loaded to the page.
         * @param studentId is the StudentId of the student being registered
         * @returns the student information 
         */
        private Student LoadStudentData(int studentId)
        {
            Student student = null;

            try
            {
                student = DbInterfaceStudent.LoadStudentData(studentId);

                //get general student information
                if (student != null)
                {
                    lblStudentId.Text = studentId.ToString();
                    txtFirstName.Text = student.firstName;
                    txtLastName.Text = student.lastName;
                    lblName.Text = student.lastName + ", " + student.firstName + " " + student.middleInitial;

                    // Get editable auditions
                    DataTable table = DbInterfaceStudentAudition.GetDistrictAuditionsForDropdown(student, false);
                    cboAudition.DataSource = null;
                    cboAudition.Items.Clear();
                    cboAudition.DataSourceID = "";

                    if (table.Rows.Count > 0 && (student.grade.Equals("11") || student.grade.Equals("12")))
                    {
                        cboAudition.DataSource = table;
                        cboAudition.Items.Add(new ListItem(""));
                        cboAudition.DataBind();
                    }
                    else if (!(student.grade.Equals("11") || student.grade.Equals("12")))
                        showWarningMessage("The selected student is not in 11/12th grade, the audition length cannot be adjusted.");
                    else
                        showWarningMessage("This student has no editable auditions. Add a new registration for this student with the 'Add Registration' option");
                }
                else
                {
                    showErrorMessage("An error occurred loading the student data");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("An error occurred loading the student data");

                Utility.LogError("Add Extra Audition Time", "LoadStudentData", "studentId: " + studentId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return student;
        }

        /*
         * Pre:
         * Post: The information associated with the selected audition is loaded to the page
         */
        protected void cboAudition_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblLength.SelectedIndex = -1;
            lblCurrentLength.Text = "";

            //load associated audition information
            if (!cboAudition.SelectedValue.ToString().Equals(""))
            {
                if (!txtStudentId.Text.Equals(""))
                {
                    Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(txtStudentId.Text));

                    if (student != null)
                    {
                        DistrictAudition districtAudition = DbInterfaceStudentAudition.GetStudentDistrictAudition(
                                                        Convert.ToInt32(cboAudition.SelectedValue), student);

                        if (districtAudition != null)
                        {
                            lblCurrentLength.Text = districtAudition.auditionLength.ToString();
                        }
                        else
                        {
                            showErrorMessage("Error: An error occurred while loading the audition information.");
                        }
                    }
                    else
                    {
                        showWarningMessage("Please reselect the student");
                    }
                }
            }
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
        * Pre:
        * Post:  The color of the header row is set
        */
        protected void gvStudentSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvStudentSearch, e);
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
                Utility.LogError("Add Extra Audition Time", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearPage();
        }

        /*
         * Pre: 
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
        {
            ClearStudentSearch();
        }

        private void ClearPage()
        {
            ClearStudentSearch();
            ClearAllExceptSearch();

            upStudentSearch.Visible = true;
            pnlButtons.Visible = false;
            pnlInfo.Visible = false;
        }

        /*
         * Pre:
         * Post: Clears the Student Search section
         */
        private void ClearStudentSearch()
        {
            txtStudentId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            gvStudentSearch.DataSource = null;
            gvStudentSearch.DataBind();
        }

        private void ClearAllExceptSearch()
        {
            rblLength.SelectedIndex = -1;
            lblStudentId.Text = "";
            lblName.Text = "";
            lblCurrentLength.Text = "";
        }

        #region Messages
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
        #endregion Messages

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("DistrictRegistration", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            showErrorMessage("Error: An error occurred.");

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
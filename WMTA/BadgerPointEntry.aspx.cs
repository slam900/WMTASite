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
    public partial class BadgerPointEntry : System.Web.UI.Page
    {
        StateAudition audition;
        //session variables
        private string auditionVar = "Audition";
        private string studentSearch = "StudentData";
        private string userRole = "UserRole";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[auditionVar] = null;
                Session[studentSearch] = null;
                loadYearDropdown();
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[auditionVar] != null)
            {
                audition = (StateAudition)Session[auditionVar];
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

                if (!(user.permissionLevel.Contains("S") || user.permissionLevel.Contains("A")))
                    Response.Redirect("/WelcomeScreen.aspx");
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
         * Pre:   The selected index must be a positive number less than the number of rows
         *        in the gridView
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAuditionError.Visible = false;
            lblStudentError.Visible = false;
            clearAllExceptSearch();

            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                string id = gvStudentSearch.Rows[index].Cells[1].Text;

                txtStudentId.Text = id;
                lblStudId.InnerText = id;

                loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));
            }
        }

        /*
         * Pre: 
         * Post: If a student is selected, the auditions are retrieve for that student
         *       for the selected year
         */
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadStudentData(Convert.ToInt32(lblStudId.InnerText));
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
                    cell.BackColor = Color.FromArgb(204, 204, 255);
            }
        }

        /*
         * Pre:  studentId must exist as a StudentId in the system
         * Post: The existing data for the student associated to the studentId 
         *       is loaded to the page.
         * @param studentId is the StudentId of the student being registered
         */
        private Student loadStudentData(int studentId)
        {
            Student student = null;

            try
            {
                student = DbInterfaceStudent.LoadStudentData(studentId);

                //get eligible auditions
                if (student != null)
                {
                    DataTable table = DbInterfaceStudentAudition.GetStateAuditionsForDropdownByYear(student, Convert.ToInt32(ddlYear.SelectedValue));
                    cboAudition.DataSource = null;
                    cboAudition.Items.Clear();
                    cboAudition.DataSourceID = "";

                    //load student name
                    txtFirstName.Text = student.firstName;
                    txtLastName.Text = student.lastName;
                    lblStudent.InnerText = student.firstName + " " + student.lastName;

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
                        lblAuditionError.InnerText = "The student has no eligible auditions";
                        lblAuditionError.Visible = true;
                    }
                }
                else
                {
                    lblErrorMsg.Text = "An error occurred while loading the student data.";
                    lblErrorMsg.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "An error occurred while loading the student data.";
                lblErrorMsg.Visible = true;

                Utility.LogError("Badger Point Entry", "loadStudentData", "studentId: " + studentId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return student;
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
                Utility.LogError("BadgerPointEntry", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students are displayed that match the search criteria (student id, first name, and last name).
         *        The error message is also reset.
         */
        protected void btnStudentSearch_Click(object sender, EventArgs e)
        {
            string id = txtStudentId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            lblStudentSearchError.Text = "";

            if (isNum || txtStudentId.Text.Equals(""))
                searchStudents(gvStudentSearch, txtStudentId.Text, txtFirstName.Text, txtLastName.Text, studentSearch);
            else
            {
                clearGridView(gvStudentSearch);
                lblStudentSearchError.ForeColor = Color.Red;
                lblStudentSearchError.Text = "A Student Id must be numeric";
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
         * @returns true if results were found and false otherwise
         */
        private bool searchStudents(GridView gridView, string id, string firstName, string lastName, string session)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceStudent.GetStudentSearchResults(id, firstName, lastName, -1);

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
                    lblStudentSearchError.Text = "An error occurred during the search.";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search.";
                lblStudentSearchError.Visible = true;

                Utility.LogError("Badger Point Entry", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Pre:
         * Post: Clears the screen to allow a user to add additional points
         */
        protected void btnNew_Click(object sender, EventArgs e)
        {
            clearPage();
            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;
            btnSubmit.Text = "Submit";
        }

        /*
         * Pre:
         * Post: The new point total is calculated based on whether or not the student
         *       attended the audition and whether or not they received a room award
         */
        protected void rblAttendance_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculatePoints();

            if (rblAttendance.SelectedIndex == 0)
                ddlRoomAward.Enabled = true;
            else
                ddlRoomAward.Enabled = false;
        }

        /*
         * Pre:
         * Post: The new point total is calculated based on whether or not the student
         *       attended the audition and whether or not they received a room award
         */
        protected void ddlRoomAward_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculatePoints();
        }

        /*
         * Pre:
         * Post: The new point total is calculated based on whether or not the student
         *       attended the audition and whether or not they received a room award.
         *       If the student attended the audition they receive 10 points and receive
         *       0 otherwise.  If they receive an Honorable Mention they get 3 points,
         *       Runner Up gets 4 points, and Room Winner gets 5 points.
         */
        private void calculatePoints()
        {
            int pointTotal = 0;

            if (rblAttendance.SelectedIndex == 0)
                pointTotal = 10 + Convert.ToInt32(ddlRoomAward.SelectedValue);

            lblPoints.InnerText = "  " + pointTotal.ToString();
        }

        /*
         * Pre:
         * Post: No new changes are submitted and the user is redirected
         *       to the welcome screen
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: Redirect the user to the welcome screen
         */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: Clears data on the page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: Clears data on the page
         */
        private void clearPage()
        {
            clearErrorMessages();
            clearStudentSearch();
            ddlYear.SelectedIndex = 0;

            lblStudent.InnerText = "";
            lblStudId.InnerText = "";
            lblDuetPartnerInfo.Visible = false;

            if (cboAudition.Items.Count > 0)
                cboAudition.SelectedIndex = 0;

            rblAttendance.SelectedIndex = 0;
            ddlRoomAward.SelectedIndex = 0;
            lblPoints.InnerText = "10";
        }

        /*
         * Pre:
         * Post: Clears all data except student search
         */
        private void clearAllExceptSearch()
        {
            clearErrorMessages();
            ddlYear.SelectedIndex = 0;

            lblStudent.InnerText = "";
            lblStudId.InnerText = "";
            lblDuetPartnerInfo.Visible = false;

            if (cboAudition.Items.Count > 0)
                cboAudition.SelectedIndex = 0;

            rblAttendance.SelectedIndex = 0;
            ddlRoomAward.SelectedIndex = 0;
            lblPoints.InnerText = "10";
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
            lblStudentSearchError.Text = "";
        }

        /*
         * Pre:
         * Post: Clears the error messages on the page
         */
        private void clearErrorMessages()
        {
            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
            lblAttendanceError.Visible = false;
            lblStudentSearchError.Visible = false;
            lblChooseAudition.Visible = false;
            lblStudentError.Visible = false;
            lblAuditionError.Visible = false;
        }

        /*
         * Pre:
         * Post: If the entered data is valid, the points are added
         *       or edited in the datbase
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            clearErrorMessages();

            if (dataIsValid())
            {
                if (audition == null) resetAuditionVar();

                audition.points = Convert.ToInt32(lblPoints.InnerText);

                if (audition.submitPoints())
                    displaySuccessMessageAndOptions();
                else
                {
                    lblErrorMsg.Text = "An error occurred. Please refresh the page to ensure the points were added.";
                    lblErrorMsg.Visible = true;
                }
            }
        }

        /*
         * Pre:
         * Post: Indicates whether or not the data on the page is
         *       valid and complete
         * @returns true if the data is complete and valid and false otherwise
         */
        private bool dataIsValid()
        {
            bool isValid = true;

            //make sure student is chosen
            if (lblStudId.InnerText.Equals(""))
            {
                isValid = false;
                lblStudentError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //make sure audition is chosen
            if (cboAudition.SelectedIndex == 0)
            {
                isValid = false;
                lblChooseAudition.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //make sure if the student was a no show that they don't have a room award
            if (rblAttendance.SelectedIndex == 1 && ddlRoomAward.SelectedIndex != 0)
            {
                isValid = false;
                lblAttendanceError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            return isValid;
        }

        /*
         * Pre:
         * Post: All controls are hidden, the user is told that the points were entered,
         *       and is given the options to add additional points or go back to
         *       the menu/welcome page
         */
        private void displaySuccessMessageAndOptions()
        {
            pnlSuccess.Visible = true;
            pnlFullPage.Visible = false;

            if (audition.auditionType.ToUpper().Equals("DUET"))
                lblSuccess.Text = "The points of the student and their duet partner were successfully entered";
            else
                lblSuccess.Text = "The student's points were successfully entered";
        }

        /*
         * Pre:
         * Post: Load the audition information associated with the selected audition
         */
        protected void cboAudition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboAudition.SelectedValue.ToString().Equals(""))
                resetAuditionVar();
        }

        /*
         * Pre:
         * Post: Loads the information of the selected audition and saves it to a session variable
         */
        private void resetAuditionVar()
        {
            try
            {
                int auditionId = Convert.ToInt32(cboAudition.SelectedValue);
                int studentId = Convert.ToInt32(Convert.ToInt32(txtStudentId.Text));
                Student student = DbInterfaceStudent.LoadStudentData(studentId);

                //get all audition info associated with audition id and save as session variable
                if (student != null)
                {
                    audition = new StateAudition(auditionId, student, false);
                    audition = DbInterfaceStudentAudition.GetStudentStateAudition(audition.districtAudition, auditionId);

                    if (audition != null)
                    {
                        Session[auditionVar] = audition;

                        //if the audition was a duet, show label to inform user that the points for the
                        //partner will also be updated
                        if (audition.auditionType.ToUpper().Equals("DUET"))
                            lblDuetPartnerInfo.Visible = true;
                        else
                            lblDuetPartnerInfo.Visible = false;

                        setPoints();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LogError("Badger Point Entry", "resetAuditionVar", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Sets the points and point control values for the selected audition
         */
        private void setPoints()
        {
            //if there have not been points previously entered, clear point data
            if (audition.points == -1)
            {
                rblAttendance.SelectedIndex = 0;
                ddlRoomAward.SelectedIndex = 0;
                lblPoints.InnerText = "10";
                btnSubmit.Text = "Submit";
            }
            //if points have already been entered, load point data
            else
            {
                int points = audition.points;
                lblPoints.InnerText = points.ToString();
                btnSubmit.Text = "Update";

                if (points >= 10)
                {
                    rblAttendance.SelectedIndex = 0;

                    if (points == 13)
                        ddlRoomAward.SelectedIndex = 1;
                    else if (points == 14)
                        ddlRoomAward.SelectedIndex = 2;
                    else if (points == 15)
                        ddlRoomAward.SelectedIndex = 3;
                    else
                        ddlRoomAward.SelectedIndex = 0;
                }
                else
                {
                    rblAttendance.SelectedIndex = 1;
                    ddlRoomAward.SelectedIndex = 0;
                }
            }
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Badger Point Entry", "On Error", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            lblErrorMsg.Text = "An error occurred";
            lblErrorMsg.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
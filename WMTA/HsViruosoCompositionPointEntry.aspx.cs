using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class HsViruosoCompositionPointEntry : System.Web.UI.Page
    {
        private HsVirtuosoCompositionAudition audition;
        //session variables
        private string auditionVar = "Audition";
        private string studentSearch = "StudentData";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            if (!Page.IsPostBack)
            {
                Session[auditionVar] = null;
                Session[studentSearch] = null;
                loadYearDropdown();
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[auditionVar] != null)
                audition = (HsVirtuosoCompositionAudition)Session[auditionVar];
        }

        /*
         * Pre:
         * Post: If the user is not logged in they will be redirected to the welcome screen
         *       If the user is not a system admin or a district admin they will be redirected
         *       to the welcome screen
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("A")))
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
            {
                User user = (User)Session[Utility.userRole];
                int districtId = -1;

                if (!user.permissionLevel.Contains('A'))
                    districtId = user.districtId;

                searchStudents(gvStudentSearch, txtStudentId.Text, txtFirstName.Text, txtLastName.Text, studentSearch, districtId);
            }
            else
            {
                clearGridView(gvStudentSearch);
                lblStudentSearchError.ForeColor = Color.Red;
                lblStudentSearchError.Text = "A Student Id must be numeric.";
            }
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students.  Matching student 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param session is the session variable that stores the student search table data
         * @param districtId is the district that should be searched, -1 indicates all districts
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
                    lblStudentSearchError.Text = "An error occurred during the search.";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search.";
                lblStudentSearchError.Visible = true;

                Utility.LogError("Hs Virtuoso Composition Point Entry", "searchStudents", "gridView: " + gridView.ID +
                                 ", id: " + id + ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
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

                //if the audition doesn't have an id assigned it is a new audition that needs to be added
                //otherwise the points just need to be updated
                if (audition.auditionId == -1)
                {
                    if (audition.addToDatabase())
                        displaySuccessMessageAndOptions();
                    else
                    {
                        lblErrorMsg.Text = "An error occurred while adding the points.  Please reload this audition to ensure they were added.";
                        lblErrorMsg.Visible = true;
                    }
                }
                else
                {
                    if (audition.updateInDatabase())
                        displaySuccessMessageAndOptions();
                    else
                    {
                        lblErrorMsg.Text = "An error occurred while updating the points.  Please reload this audition to ensure they were updated.";
                        lblErrorMsg.Visible = true;
                    }
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

            //make sure audition type is chosen
            if (ddlAuditionType.SelectedIndex == 0)
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
        }

        /*
     * Pre:
     * Post: Loads the information of the selected audition and saves it to a session variable
     */
        private void resetAuditionVar()
        {
            int studentId;

            try
            {
                if (Int32.TryParse(txtStudentId.Text, out studentId))
                {
                    Student student = DbInterfaceStudent.LoadStudentData(studentId);
                    int year = Convert.ToInt32(ddlYear.SelectedValue);
                    string auditionType = ddlAuditionType.SelectedValue;

                    //get all audition info associated with audition id and save as session variable
                    if (student != null)
                    {
                        audition = new HsVirtuosoCompositionAudition(student, year, auditionType);
                        Session[auditionVar] = audition;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LogError("HS Virtuoso Composition Point Entry", "resetAuditionVar", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
         * Pre:   The selected index must be a positive number less than the number of rows
         *        in the gridView
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStudentError.Visible = false;
            clearAllExceptSearch();

            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                string id = gvStudentSearch.Rows[index].Cells[1].Text;
                string firstName = gvStudentSearch.Rows[index].Cells[2].Text;
                string lastName = gvStudentSearch.Rows[index].Cells[3].Text;

                //load student data to avoid the bug where ' shows up as &#39; if the data is just taken from the gridview
                Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                if (student != null)
                {
                    firstName = student.firstName;
                    lastName = student.lastName;
                }
                
                txtStudentId.Text = id;
                txtFirstName.Text = firstName;
                txtLastName.Text = lastName;
                lblStudent.InnerText = firstName + " " + lastName;
                lblStudId.InnerText = id;
            }
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
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
        {
            clearStudentSearch();
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

            ddlAuditionType.SelectedIndex = 0;
            rblAttendance.SelectedIndex = 0;
            ddlRoomAward.SelectedIndex = 0;
            lblPoints.InnerText = "10";
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
            lblAttendanceError.Visible = false;
            lblStudentSearchError.Visible = false;
            lblAuditionYearError.Visible = false;
            lblChooseAudition.Visible = false;
            lblStudentError.Visible = false;
            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
            lblChooseAudition.Visible = false;
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

            ddlAuditionType.SelectedIndex = 0;
            rblAttendance.SelectedIndex = 0;
            ddlRoomAward.SelectedIndex = 0;
            lblPoints.InnerText = "10";
        }

        /*
         * Pre:
         * Post: Redirect the user to the welcome screen
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
         * Pre:   gvStudentSearch must contain more than one page
         * Post:  The page of gvStudentSearch is changed
         */
        protected void gvStudentSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudentSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
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
                Utility.LogError("Hs Virtuoso Composition Point Entry", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
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
         * Pre:
         * Post: If an audition type is selected see if the student has already
         *       had points entered.  If there are existing points, load them
         */
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAuditionYearError.Visible = false;

            if (!ddlYear.SelectedValue.ToString().Equals("") && !ddlAuditionType.SelectedValue.ToString().Equals("") && !lblStudId.InnerText.Equals(""))
                loadAudition(Convert.ToInt32(lblStudId.InnerText),
                             Convert.ToInt32(ddlYear.SelectedValue), ddlAuditionType.SelectedValue);
        }

        /*
         * Pre:
         * Post: If an audition year is selected see if the student has already
         *       had points entered.  If there are existing points, load them
         */
        protected void ddlAuditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblChooseAudition.Visible = false;

            if (!ddlYear.SelectedValue.ToString().Equals("") && !ddlAuditionType.SelectedValue.ToString().Equals("") && !lblStudId.InnerText.Equals(""))
                loadAudition(Convert.ToInt32(lblStudId.InnerText),
                             Convert.ToInt32(ddlYear.SelectedValue), ddlAuditionType.SelectedValue);
        }

        /*
         * Pre:
         * Post: Get audition information for the input year, audition type, and student
         *       if a matching audition exists
         * @param studentId is the unique id of the student
         * @year is the year of the audition
         * @auditionType signifies whether the points are being entered for HS Virtuoso
         *               or Composition
         */
        private void loadAudition(int studentId, int year, string auditionType)
        {
            //get student
            Student student = DbInterfaceStudent.LoadStudentData(studentId);

            //get audition
            if (student != null)
            {
                audition = DbInterfaceStudentAudition.GetStudentHsOrCompositionAudition(student, year, auditionType);

                Session[auditionVar] = audition;

                //set points on screen
                if (audition != null)
                    setPoints();
                else
                {
                    rblAttendance.SelectedIndex = 1;
                    ddlRoomAward.SelectedIndex = 0;
                    lblPoints.InnerText = "0";
                    btnSubmit.Text = "Submit";
                    ddlRoomAward.Enabled = true;
                }
            }
            else
            {
                lblErrorMsg.Text = "There was an error loading the student data";
                lblErrorMsg.Visible = true;
            }
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
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("HS Virtuoso Composition Point Entry", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            lblErrorMsg.Text = "An error occurred";
            lblErrorMsg.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
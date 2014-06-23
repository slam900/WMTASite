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
    public partial class DistrictRegistration : System.Web.UI.Page
    {
        private Utility.Action action = Utility.Action.Add;
        //session variables
        private string studentSearch = "StudentData";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //coordinatesToRemove = new List<StudentCoordinateSimple>(); uncomment everything in here

            //clear session variables
            if (!Page.IsPostBack)
            {
                //Session[compositionTable] = null;
                Session[studentSearch] = null;
                //Session[coordinateSearch] = null;
                //Session[partnerSearch] = null;
                //Session[coordinateTable] = null;
                //Session[coordsToRemove] = null;
                //Session[preferredTime] = null;
                //Session[auditionVar] = null;
                //Session[completed] = null;

                //get requested action - default to adding
                string test = Request.QueryString["action"];

                if (test == null)
                {
                    action = Utility.Action.Add;
                }
                else
                {
                    action = (Utility.Action)Convert.ToInt32(action);
                }
            }

            ////if there were compositions selected before the postback, add them 
            ////back to the table
            //else if (Page.IsPostBack && Session[compositionTable] != null)
            //{
            //    TableRow[] rowArray = (TableRow[])Session[compositionTable];

            //    for (int i = 1; i < rowArray.Length; i++)
            //        tblCompositions.Rows.Add(rowArray[i]);
            //}

            ////if there were coordinating students selected before the postback,
            ////add them back to the table
            //if (Page.IsPostBack && Session[coordinateTable] != null)
            //{
            //    TableRow[] rowArray = (TableRow[])Session[coordinateTable];

            //    for (int i = 1; i < rowArray.Length; i++)
            //        tblCoordinates.Rows.Add(rowArray[i]);
            //}

            ////if there were coordinating students to remove from the audition before 
            ////the postback, add them back to the list
            //if (Page.IsPostBack && Session[coordsToRemove] != null)
            //{
            //    List<StudentCoordinateSimple> coords = (List<StudentCoordinateSimple>)Session[coordsToRemove];

            //    for (int i = 0; i < coords.Count; i++)
            //        coordinatesToRemove.Add(coords.ElementAt(i));
            //}

            ////if an audition object has been instantiated, reload
            //if (Page.IsPostBack && Session[auditionVar] != null)
            //    audition = (DistrictAudition)Session[auditionVar];

            //if (Page.IsPostBack && Session[completed] != null)
            //{
            //    pnlSuccess.Visible = true;
            //    //pnlFullPage.Visible = false;
            //    Session[completed] = null;
            //}
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

                //allow user to view only their own students if they are a teacher as well as a higher permission level
                if (user.permissionLevel.Contains("T") && (user.permissionLevel.Contains("D") || user.permissionLevel.Contains("S") || user.permissionLevel.Contains("A")))
                    pnlMyStudents.Visible = true;
            }
        }

        /*** Student Search Code ***/

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
                phStudentSearchError.Visible = true;
                lblStudentSearchError.Text = "A Student Id must be numeric.";
            }
        }

        /*
         * Pre:
         * Post: Display message telling user that there were no search results
         */
        private void displayStudentSearchError()
        {
            lblStudentSearchError.Text = "The search did not return any results";
            phStudentSearchError.Visible = true;
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
                    lblStudentSearchError.Text = "An error occurred during the search";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search";
                lblStudentSearchError.Visible = true;

                Utility.LogError("District Registration", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
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

                Utility.LogError("District Registration", "searchOwnStudents", "gridView: " + gridView.ID + ", id: " + id +
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
            lblStudentSearchError.Visible = false;
            //lblAuditionError.Visible = false; uncomment this
            int index = gvStudentSearch.SelectedIndex;
            int year = DateTime.Today.Year;

            clearAllExceptSearch();

            //get audition year
            if (DateTime.Today.Month >= 6) year = year + 1;
            year = DateTime.Today.Year; //delete this

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                upStudentSearch.Visible = false;
                pnlInfo.Visible = true;

                txtStudentId.Text = gvStudentSearch.Rows[index].Cells[1].Text;

                Student student = loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));

                //ddlSite.SelectedIndex = ddlSite.Items.IndexOf(ddlSite.Items.FindByValue(student.districtId.ToString()));
                //getAuditionDate(Convert.ToInt32(ddlSite.SelectedValue), year);  uncomment all of this
                //setTheoryLevel(student.theoryLevel);

                ////create DistrictAudition object and save to session variable
                //audition = new DistrictAudition(student);
                //Session[auditionVar] = audition;
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
                Utility.LogError("District Registration", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
        {
            clearStudentSearch();
        }

        /*** End Student Search Code ***/

        /*** Student Information Function ***/


        /*
         * Pre:  studentId must exist as a StudentId in the system
         * Post: The existing data for the student associated to the studentId 
         *       is loaded to the page.
         * @param studentId is the StudentId of the student being registered
         * @returns the student information 
         */
        private Student loadStudentData(int studentId)
        {
            Student student = null;

            try
            {
                student = DbInterfaceStudent.LoadStudentData(studentId);
                resetTheoryLevels();

                //get general student information
                if (student != null)
                {
                    lblStudentId.Text = studentId.ToString();
                    txtFirstName.Text = student.firstName;
                    txtLastName.Text = student.lastName;
                    lblName.Text = student.lastName + ", " + student.firstName + " " + student.middleInitial;
                    txtGrade.Text = student.grade;
                    lblDistrict.Text = student.getDistrict();
                    lblTeacher.Text = student.getCurrTeacher();

                    //load the student's theory level
                    setTheoryLevel(student.theoryLevel);

                    //get auditions for upcoming district audition if editing or deleting
                    if (action != Utility.Action.Add)
                    {
                        //DataTable table = DbInterfaceStudentAudition.GetDistrictAuditionsForDropdown(student);
                        //cboAudition.DataSource = null;   uncomment
                        //cboAudition.Items.Clear();
                        //cboAudition.DataSourceID = "";

                        //if (table.Rows.Count > 0)
                        //{
                        //    cboAudition.DataSource = table;
                        //    cboAudition.Items.Add(new ListItem(""));
                        //    cboAudition.DataBind();
                        //}
                        //else
                        //    lblAuditionError.Visible = true;
                    }
                }
                else
                {
                    //lblErrorMsg.Text = "An error occurred loading the student data"; uncomment and make error popup thing
                    //lblErrorMsg.Visible = true;
                }
            }
            catch (Exception e)
            {
                //lblErrorMsg.Text = "An error occurred loading the student data"; uncomment
                //lblErrorMsg.Visible = true;

                Utility.LogError("District Registration", "loadStudentData", "studentId: " + studentId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return student;
        }

        /*
        * Pre:
        * Post: The student's Theory Test Level will be updated based
        *       on the instrument and grade.
        */
        private void setTheoryLevel()
        {
            //int grade = getEnteredGrade();  uncomment all of this
            //string currTheoryLevel = ddlTheoryLevel.SelectedValue;
            //DataTable table;

            ////if the grade is valid and an instrument has been selected, get the valid theory test levels
            //if (grade != -1 && ddlInstrument.SelectedIndex != 0)
            //{
            //    ddlTheoryLevel.Items.Clear();
            //    ddlTheoryLevel.Items.Add(new ListItem("", ""));

            //    table = DbInterfaceStudentAudition.GetTheoryTestLevel(ddlInstrument.Text, txtGrade.Text, ddlAuditionTrack.SelectedValue);

            //    if (table != null)
            //    {
            //        ddlTheoryLevel.DataSource = table;
            //        ddlTheoryLevel.DataTextField = "TheoryTest";
            //        ddlTheoryLevel.DataValueField = "TheoryTest";
            //        ddlTheoryLevel.DataBind();
            //    }
            //    else
            //    {
            //        lblErrorMsg.Text = "An error occurred while updating the valid theory levels";
            //        lblErrorMsg.Visible = true;
            //    }
            //}

            //setTheoryLevel(currTheoryLevel);
        }

        /*
         * Pre:
         * Post: Set the student's theory level . If EA or EB, show the theory level type box and fill it in if
         *       the student has a theory type (Ex: EA-Bass)
         */
        private void setTheoryLevel(string theoryLevel)
        {
            string level = theoryLevel;
            string theoryLevelType = "";

            //ddlTheoryLevelType.Visible = false; uncomment all of this

            //if (level.Length > 2)  
            //    level = level.Substring(0, 2);

            //ListItem selectedItem = ddlTheoryLevel.Items.FindByValue(level);
            //if (selectedItem != null)
            //    ddlTheoryLevel.SelectedIndex = ddlTheoryLevel.Items.IndexOf(selectedItem);

            //if (level.Equals("EA") || level.Equals("EB"))
            //{
            //    ddlTheoryLevelType.Visible = true;

            //    if (theoryLevel.Length > 2)
            //        theoryLevelType = theoryLevel.Substring(3);

            //    selectedItem = ddlTheoryLevelType.Items.FindByValue(theoryLevelType);
            //    if (selectedItem != null)
            //        ddlTheoryLevelType.SelectedIndex = ddlTheoryLevelType.Items.IndexOf(selectedItem);
            //}
        }

        /*
         * Pre:
         * Post: Loads all of the available theory test levels to the dropdown
         */
        private void resetTheoryLevels()
        {
            DataTable table;

            //ddlTheoryLevel.Items.Clear();  uncomment this
            //ddlTheoryLevel.Items.Add(new ListItem("", ""));

            //table = DbInterfaceStudentAudition.GetTheoryTestLevel("Piano", "12", "District");

            //if (table != null)
            //{
            //    ddlTheoryLevel.DataSource = table;
            //    ddlTheoryLevel.DataTextField = "TheoryTest";
            //    ddlTheoryLevel.DataValueField = "TheoryTest";
            //    ddlTheoryLevel.DataBind();
            //}
            //else
            //{
            //    lblErrorMsg.Text = "An error occurred while updating the theory test levels";
            //    lblErrorMsg.Visible = true;
            //}
        }

        /*** End Student Information Functions

        /*** Clear Functions ***/

        /*
         * Pre:
         * Post: Clears the Student Search section
         */
        private void clearStudentSearch()
        {
            txtStudentId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            gvStudentSearch.DataSource = null;
            gvStudentSearch.DataBind();
            lblStudentSearchError.Visible = false;
        }

        /*
         * Pre:
         * Post: Clears all data except the student search section
         */
        private void clearAllExceptSearch()
        {
            //clearStudentInformation();  uncomment all of this
            //clearAuditionInformation();
            //clearCompositionsToPerform();
            //clearTimeConstraints();
            //ddlSite.SelectedIndex = 0;
            //lblAuditionDate.Text = "";
            //lblSiteError.Visible = false;
            //clearDuetPartner();
            //pnlDuetPartner.Visible = false;
            //lblErrorMsg.Visible = false;
            ////clear student auditions
            //cboAudition.Items.Clear();
            //cboAudition.Items.Add(new ListItem("", ""));
        }

        /*** End Clear Functions ***/
    }
}
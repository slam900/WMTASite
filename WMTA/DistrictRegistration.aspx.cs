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
    public partial class DistrictRegistration : System.Web.UI.Page
    {
        private DistrictAudition audition;
        private List<StudentCoordinateSimple> coordinatesToRemove; //keeps track of coordinates that need to be removed from the audition
        /* session variables */
        private string compositionTable = "CompositionTable";
        private string studentSearch = "StudentData";
        private string coordinateSearch = "CoordinateData";
        private string partnerSearch = "PartnerData";
        private string coordinateTable = "CoordinateTable";
        private string coordsToRemove = "CoordinatesToRemove";
        private string preferredTime = "PreferredTime";
        private string auditionVar = "Audition";
        private string completed = "Completed";
        private Utility.Action action = Utility.Action.Add;

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            coordinatesToRemove = new List<StudentCoordinateSimple>();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[compositionTable] = null;
                Session[studentSearch] = null;
                Session[coordinateSearch] = null;
                Session[partnerSearch] = null;
                Session[coordinateTable] = null;
                Session[coordsToRemove] = null;
                Session[preferredTime] = null;
                Session[auditionVar] = null;
                Session[completed] = null;

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

            //if there were compositions selected before the postback, add them 
            //back to the table
            else if (Page.IsPostBack && Session[compositionTable] != null)
            {
                TableRow[] rowArray = (TableRow[])Session[compositionTable];

                for (int i = 1; i < rowArray.Length; i++)
                    tblCompositions.Rows.Add(rowArray[i]);
            }

            //if there were coordinating students selected before the postback,
            //add them back to the table
            if (Page.IsPostBack && Session[coordinateTable] != null)
            {
                TableRow[] rowArray = (TableRow[])Session[coordinateTable];

                for (int i = 1; i < rowArray.Length; i++)
                    tblCoordinates.Rows.Add(rowArray[i]);
            }

            //if there were coordinating students to remove from the audition before 
            //the postback, add them back to the list
            if (Page.IsPostBack && Session[coordsToRemove] != null)
            {
                List<StudentCoordinateSimple> coords = (List<StudentCoordinateSimple>)Session[coordsToRemove];

                for (int i = 0; i < coords.Count; i++)
                    coordinatesToRemove.Add(coords.ElementAt(i));
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[auditionVar] != null)
                audition = (DistrictAudition)Session[auditionVar];

            if (Page.IsPostBack && Session[completed] != null)
            {
                pnlSuccess.Visible = true;
                //pnlFullPage.Visible = false;
                Session[completed] = null;
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

                //allow user to view only their own students if they are a teacher as well as a higher permission level
                if (user.permissionLevel.Contains("T") && (user.permissionLevel.Contains("D") || user.permissionLevel.Contains("S") || user.permissionLevel.Contains("A")))
                    pnlMyStudents.Visible = true;
            }
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
                        lblStudentSearchError.Visible = true;
                        lblStudentSearchError.ForeColor = Color.DarkBlue;
                        lblStudentSearchError.Text = "The search did not return any results";
                    }
                }
                else if (chkMyStudentsOnly.Checked || (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S') ||
                                user.permissionLevel.Contains('D')) && user.permissionLevel.Contains('T')))
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchOwnStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, ((User)Session[Utility.userRole]).contactId))
                    {
                        lblStudentSearchError.Visible = true;
                        lblStudentSearchError.ForeColor = Color.DarkBlue;
                        lblStudentSearchError.Text = "The search did not return any results";
                    }
                }
                else if (!chkMyStudentsOnly.Checked && (user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')))
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, districtId))
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
                lblStudentSearchError.Text = "A Student Id must be numeric.";
            }
        }

        /*
         * Pre:  The StudentId field must be empty or contain an integer
         * Post: Students matching the search criteria are displayed and the error
         *       message is reset.
         */
        protected void btnPartnerSearch_Click(object sender, EventArgs e)
        {
            string id = txtPartnerId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                User user = (User)Session[Utility.userRole];
                int districtId = -1;

                //if district admin get their district because that is all the students they can register
                if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')) && user.permissionLevel.Contains('D'))
                {
                    districtId = user.districtId;

                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvDuetPartner, id, txtPartnerFirstName.Text, txtPartnerLastName.Text, partnerSearch, districtId))
                    {
                        lblPartnerError.Visible = true;
                        lblPartnerError.ForeColor = Color.DarkBlue;
                        lblPartnerError.Text = "The search did not return any results";
                    }
                }
                else if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S') ||
                                user.permissionLevel.Contains('D')) && user.permissionLevel.Contains('T'))
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchOwnStudents(gvDuetPartner, id, txtPartnerFirstName.Text, txtPartnerLastName.Text, partnerSearch, ((User)Session[Utility.userRole]).contactId))
                    {
                        lblPartnerError.Visible = true;
                        lblPartnerError.ForeColor = Color.DarkBlue;
                        lblPartnerError.Text = "The search did not return any results";
                    }
                }
                else if (user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S'))
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchStudents(gvDuetPartner, id, txtPartnerFirstName.Text, txtPartnerLastName.Text, partnerSearch, -1))
                    {
                        lblPartnerError.Visible = true;
                        lblPartnerError.ForeColor = Color.DarkBlue;
                        lblPartnerError.Text = "The search did not return any results";
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvDuetPartner);
                lblPartnerError.ForeColor = Color.Red;
                lblPartnerError.Text = "A Student Id must be numeric";
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
         * Pre:   
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            //pnlInfo.Visible = true;
            lblStudentSearchError.Visible = false;
            lblAuditionError.Visible = false;
            int index = gvStudentSearch.SelectedIndex;
            int year = DateTime.Today.Year;

            clearAllExceptSearch();

            //get audition year
            if (DateTime.Today.Month >= 6) year = year + 1;
            year = DateTime.Today.Year; //delete this

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                txtStudentId.Text = gvStudentSearch.Rows[index].Cells[1].Text;

                Student student = loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));

                ddlSite.SelectedIndex = ddlSite.Items.IndexOf(ddlSite.Items.FindByValue(student.districtId.ToString()));
                getAuditionDate(Convert.ToInt32(ddlSite.SelectedValue), year);
                setTheoryLevel(student.theoryLevel);            

                //create DistrictAudition object and save to session variable
                audition = new DistrictAudition(student);
                Session[auditionVar] = audition;
            }
        }

        /*
         * Pre:   
         * Post:  The name and id of the selected student is shown under the audition type
         *        drop down list and a new entry is made in the coordinate table
         */
        protected void gvDuetPartner_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblPartnerError.Text = "";
            int index = gvDuetPartner.SelectedIndex;

            if (index >= 0 && index < gvDuetPartner.Rows.Count)
            {
                string id = gvDuetPartner.Rows[index].Cells[1].Text;
                string firstName = gvDuetPartner.Rows[index].Cells[2].Text;
                string lastName = gvDuetPartner.Rows[index].Cells[3].Text;

                //retrieve from database in case there is an apostrophe in the student's name (which would other wise display as &#39;
                Student partner = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                if (partner != null)
                {
                    firstName = partner.firstName;
                    lastName = partner.lastName;
                }

                //show label under audition track dropdownlist
                lblDuetPartner.InnerText = "Partner: " + firstName + " " + lastName + " (" + id + ")";
                lblDuetPartner.Visible = true;
                lnkChangePartner.Visible = true;

                //add entries to coordinate table
                addCoordinate(id, firstName, lastName, "Duet");

                //create carpool if the partner has other auditions
                if (DbInterfaceStudentAudition.StudentHasDistrictAudition(DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id))))
                    addCoordinate(id, firstName, lastName, "Carpool");

                //clear and hide the partner search panel
                txtPartnerId.Text = "";
                txtPartnerFirstName.Text = "";
                txtPartnerLastName.Text = "";
                gvDuetPartner.DataSource = null;
                gvDuetPartner.DataBind();
                lblPartnerError.Visible = false;
                pnlDuetPartner.Visible = false;
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
         * Post:  The page of gvDuetPartner is changed
         */
        protected void gvDuetPartner_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDuetPartner.PageIndex = e.NewPageIndex;
            BindPartnerSessionData();
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
        * Pre:
        * Post:  The color of the header row is set
        */
        protected void gvDuetPartner_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvDuetPartner, e);
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
                        DataTable table = DbInterfaceStudentAudition.GetDistrictAuditionsForDropdown(student);
                        cboAudition.DataSource = null;
                        cboAudition.Items.Clear();
                        cboAudition.DataSourceID = "";

                        if (table.Rows.Count > 0)
                        {
                            cboAudition.DataSource = table;
                            cboAudition.Items.Add(new ListItem(""));
                            cboAudition.DataBind();
                        }
                        else
                            lblAuditionError.Visible = true;
                    }
                }
                else
                {
                    lblErrorMsg.Text = "An error occurred loading the student data";
                    lblErrorMsg.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "An error occurred loading the student data";
                lblErrorMsg.Visible = true;

                Utility.LogError("District Registration", "loadStudentData", "studentId: " + studentId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return student;
        }

        /*
         * Pre:
         * Post: Loads all of the available theory test levels to the dropdown
         */
        private void resetTheoryLevels()
        {
            DataTable table;

            ddlTheoryLevel.Items.Clear();
            ddlTheoryLevel.Items.Add(new ListItem("", ""));

            table = DbInterfaceStudentAudition.GetTheoryTestLevel("Piano", "12", "District");

            if (table != null)
            {
                ddlTheoryLevel.DataSource = table;
                ddlTheoryLevel.DataTextField = "TheoryTest";
                ddlTheoryLevel.DataValueField = "TheoryTest";
                ddlTheoryLevel.DataBind();
            }
            else
            {
                lblErrorMsg.Text = "An error occurred while updating the theory test levels";
                lblErrorMsg.Visible = true;
            }
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
         * Pre:  The duet partner search session variable must have been previously defined
         * Post: The stored data is bound to the gridView
         */
        protected void BindPartnerSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[partnerSearch];
                gvDuetPartner.DataSource = data;
                gvDuetPartner.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("District Registration", "BindPartnerSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
         * Pre:
         * Post:  If an instrument is selected and a grade has been entered,
         *        the student's Theory Test Level will be updated based on their
         *        Grade and Instrument.
         *        If the student is playing organ or piano, the accompanist text box
         *        will be emptied and not enabled since an accompanist is not needed.
         */
        protected void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblInstrumentError.Visible = false;
            setTheoryLevel();

            if (ddlInstrument.Text.Equals("Organ") || ddlInstrument.Text.Equals("Piano"))
            {
                txtAccompanist.Text = "";
                txtAccompanist.Enabled = false;
                txtAccompanist.BackColor = Color.LightGray;
            }
            else
            {
                txtAccompanist.Enabled = true;
                txtAccompanist.BackColor = Color.White;
            }
        }

        /*
         * Pre:
         * Post:  If the student's grade is changed the student's Theory Test Level,
         *        will be updated based on their Grade and Instrument
         */
        protected void txtGrade_TextChanged(object sender, EventArgs e)
        {
            lblGradeError.Visible = false;

            audition.student.grade = txtGrade.Text;

            //set theory level and available audition tracks based on the entered grade
            setTheoryLevel();
            setValidAuditionTracks();
        }

        /*
         * Pre:
         * Post: When the audition type changes, the available audition tracks in the dropdown
         *       are updated based on the selected audition type and entered grade.
         */
        protected void ddlAuditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAuditionTypeError.Visible = false;
            setValidAuditionTracks();

            if (ddlAuditionType.SelectedValue.Equals("Duet"))
            {
                pnlDuetPartner.Visible = true;
            }
            else if (!lblDuetPartner.InnerText.Equals(""))
            {
                removeDuetPartner();

                //hide the panel
                pnlDuetPartner.Visible = false;
            }
            else
            {
                //clear the partner controls
                lblDuetPartner.InnerText = "";
                lblDuetPartner.Visible = false;
                lnkChangePartner.Visible = false;
                txtPartnerId.Text = "";
                txtPartnerFirstName.Text = "";
                txtPartnerLastName.Text = "";
                gvDuetPartner.DataSource = null;
                gvDuetPartner.DataBind();

                //hide the panel
                pnlDuetPartner.Visible = false;
            }
        }

        /*
         * Pre:
         * Post: The current duet partner is removed and the controls for choosing
         *       a duet partner are shown
         */
        protected void lnkChangePartner_Click(object sender, EventArgs e)
        {
            removeDuetPartner();
            pnlDuetPartner.Visible = true;
        }

        /*
         * Pre:  A duet partner must have previously been chosen
         * Post: The duet partner is removed from the coordinate table and their
         *       data is removed from the duet partner label
         */
        private void removeDuetPartner()
        {
            //clear the partner from the coordinate table
            if (lblDuetPartner.Visible)
            {
                string id = lblDuetPartner.InnerText.Split('(')[1]; //get the id out of the label
                id = id.Substring(0, id.Length - 1);

                //find in coordinate table and delete row
                bool removed = false;
                int i = 1;
                while (i < tblCoordinates.Rows.Count && !removed)
                {
                    if (tblCoordinates.Rows[i].Cells[0].Text.Equals(id))
                    {
                        tblCoordinates.Rows.Remove(tblCoordinates.Rows[i]);
                        removed = true;
                    }

                    i++;
                }

                //if the duet partner was the only coordinate, hide the coordinate section
                if (tblCoordinates.Rows.Count == 1)
                    pnlCoordinateParticipants.Visible = false;

                saveTableToSession(tblCoordinates, coordinateTable);

                //clear the partner controls
                lblDuetPartner.InnerText = "";
                lblDuetPartner.Visible = false;
                lnkChangePartner.Visible = false;
                txtPartnerId.Text = "";
                txtPartnerFirstName.Text = "";
                txtPartnerLastName.Text = "";
                gvDuetPartner.DataSource = null;
                gvDuetPartner.DataBind();
            }
        }

        /*
         * Pre:
         * Post: The student's Theory Test Level will be updated based
         *       on the instrument and grade.
         */
        private void setTheoryLevel()
        {
            int grade = getEnteredGrade();
            string currTheoryLevel = ddlTheoryLevel.SelectedValue;
            DataTable table;

            //if the grade is valid and an instrument has been selected, get the valid theory test levels
            if (grade != -1 && ddlInstrument.SelectedIndex != 0)
            {
                ddlTheoryLevel.Items.Clear();
                ddlTheoryLevel.Items.Add(new ListItem("", ""));

                table = DbInterfaceStudentAudition.GetTheoryTestLevel(ddlInstrument.Text, txtGrade.Text, ddlAuditionTrack.SelectedValue);

                if (table != null)
                {
                    ddlTheoryLevel.DataSource = table;
                    ddlTheoryLevel.DataTextField = "TheoryTest";
                    ddlTheoryLevel.DataValueField = "TheoryTest";
                    ddlTheoryLevel.DataBind();
                }
                else
                {
                    lblErrorMsg.Text = "An error occurred while updating the valid theory levels";
                    lblErrorMsg.Visible = true;
                }
            }

            setTheoryLevel(currTheoryLevel);
        }

        /*
         * Pre:
         * Post: The possible auditoin tracks are updated based on the entered
         *       grade and selected audition type
         */
        private void setValidAuditionTracks()
        {
            int grade = getEnteredGrade();

            if (grade != -1 || txtGrade.Text.Equals(""))
            {
                try
                {
                    DataTable tableTracks = DbInterfaceStudentAudition.GetValidAuditionTracks(txtGrade.Text, ddlAuditionType.Text);

                    if (tableTracks != null)
                    {
                        //clear current contents
                        ddlAuditionTrack.DataSource = null;
                        ddlAuditionTrack.Items.Clear();
                        ddlAuditionTrack.DataSourceID = "";

                        //update tables
                        ddlAuditionTrack.DataSource = tableTracks;
                        ddlAuditionTrack.DataTextField = "AuditionTrack";
                        ddlAuditionTrack.DataValueField = "AuditionTrack";

                        //add blank item
                        ddlAuditionTrack.Items.Add(new ListItem(""));

                        //bind new data
                        ddlAuditionTrack.DataBind();
                    }
                    else
                    {
                        lblErrorMsg.Text = "An error occurred while loading audition tracks.";
                        lblErrorMsg.Visible = true;
                    }
                }
                catch (Exception e)
                {
                    lblErrorMsg.Text = "An error occurred while loading audition tracks.";
                    lblErrorMsg.Visible = true;

                    Utility.LogError("District Registration", "setValidAuditionTracks", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
                }
            }
            else
            {
                lblGradeError.Visible = true;
                lblErrorMsg.Visible = true;
            }
        }

        /*
         * Pre:
         * Post:  If the entered grade is valid, return it.  Otherwise
         *        return -1
         */
        private int getEnteredGrade()
        {
            int grade;
            bool gradeIsInt = int.TryParse(txtGrade.Text, out grade);

            //a grade must be an integer between 1 and 12, K, or A
            if (!gradeIsInt && txtGrade.Text.Equals("K"))
                grade = 1;
            else if (!gradeIsInt && (txtGrade.Text.Equals("A") || txtGrade.Text.Equals("Adult")))
                grade = 12;
            else if (!gradeIsInt || (gradeIsInt && (grade < 1 || grade > 12)))
                grade = -1;
            //TODO display invalid grade error

            return grade;
        }

        /*
         * Pre:
         * Post: The options in the "Composer" and "Composition" dropdowns
         *       will be filtered based on the selected Style and Level.
         *       Compositions will also be filtered based on the selected
         *       composer.
         */
        protected void cboStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyle.Text, ddlCompLevel.Text, ddlComposer.Text);
            searchComposers(ddlStyle.Text, ddlCompLevel.Text);
        }

        /*
         * Pre:
         * Post: The options in the "Composer" and "Composition" dropdowns
         *       will be filtered based on the selected Style and Level.
         *       Compositions will also be filtered based on the selected
         *       composer.
         */
        protected void cboCompLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyle.Text, ddlCompLevel.Text, ddlComposer.Text);
            searchComposers(ddlStyle.Text, ddlCompLevel.Text);
        }

        /*
         * Pre:
         * Post: The options in the "Composition" dropdown will be filtered based
         *       on the selected Style, Level, and Composer
         */
        protected void cboComposer_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyle.Text, ddlCompLevel.Text, ddlComposer.Text);
        }

        /*
         * Pre:
         * Post: The data associated with the selected composition is retrieved from the
         *       database and displayed in the Compositions to Perform section
         */
        protected void ddlComposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            double length, seconds;
            int minutes;

            if (!ddlComposition.SelectedValue.ToString().Equals(""))
            {
                int selectedCompId = Convert.ToInt32(ddlComposition.SelectedValue);
                Composition composition = DbInterfaceComposition.GetComposition(selectedCompId);

                if (composition != null)
                {
                    ddlStyle.Text = composition.style;
                    ddlCompLevel.SelectedValue = composition.compLevel;

                    ListItem item = ddlComposer.Items.FindByValue(composition.composer);
                    if (item != null) 
                        ddlComposer.SelectedValue = composition.composer;

                    //get minutes and seconds
                    length = composition.playingTime;
                    minutes = (int)length;
                    seconds = length - (double)minutes;
                    txtMinutes.Text = minutes.ToString();
                    ddlSeconds.SelectedValue = seconds.ToString();
                }
                else
                {
                    lblErrorMsg.Text = "The composition could not be loaded";
                    lblErrorMsg.Visible = true;
                }
            }
        }

        /*
         * Pre:   The input style and competition level must exist in the system
         * Post:  The input parameters are used to search for existing compositions.  
         *        Matching compositions are loaded to the corresponding drop downs
         * @param style is the style of compositions being loaded
         * @param compLevel is the competition level of compositions being loaded
         */
        private void searchCompositions(string style, string compLevelId, string composer)
        {
            try
            {
                DataTable tableComposition = DbInterfaceComposition.GetCompositionSearchResults(style, compLevelId, composer);

                if (tableComposition != null)
                {
                    //clear current contents
                    ddlComposition.DataSource = null;
                    ddlComposition.Items.Clear();
                    ddlComposition.DataSourceID = "";

                    //update tables
                    ddlComposition.DataSource = tableComposition;
                    ddlComposition.DataTextField = "CompositionName";
                    ddlComposition.DataValueField = "CompositionId";

                    //add blank item
                    ddlComposition.Items.Add(new ListItem(""));

                    //bind new data
                    ddlComposition.DataBind();
                }
                else
                {
                    lblCompositionError.Text = "An error occurred during the search";
                    lblCompositionError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblCompositionError.Text = "An error occurred during the search";
                lblCompositionError.Visible = true;

                Utility.LogError("District Registration", "searchCompositions", "style: " + style + ", compLevelId: " +
                                compLevelId + ", composer: " + composer, "Message: " + e.Message + "   Stack Trace: " +
                                e.StackTrace, -1);
            }
        }

        /*
         * Pre:   The input style and competition level must exist in the system
         * Post:  The input parameters are used to search for existing composers.  
         *        Matching composers are loaded to the Composer dropdown
         * @param style is the style of compositions by composers being loaded
         * @param compLevel is the competition level of compositions by composers being loaded
         */
        private void searchComposers(string style, string compLevelId)
        {
            try
            {
                DataTable tableComposer = DbInterfaceComposition.GetComposerSearchResults(style, compLevelId);

                if (tableComposer != null)
                {
                    //Load the search results in the dropdowns. 
                    ddlComposer.DataSource = null;

                    //clear current contents
                    ddlComposer.Items.Clear();
                    ddlComposer.DataSourceID = "";

                    //update tables
                    ddlComposer.DataSource = tableComposer;
                    ddlComposer.DataTextField = "Composer";
                    ddlComposer.DataValueField = "Composer";

                    //add blank item
                    ddlComposer.Items.Add(new ListItem(""));

                    //bind new data
                    ddlComposer.DataBind();
                }
                else
                {
                    lblCompositionError.Text = "An error occurred during the search";
                    lblCompositionError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblCompositionError.Text = "An error occurred during the search";
                lblCompositionError.Visible = true;

                Utility.LogError("District Registration", "searchComposers", "style: " + style + ", compLevelId: " +
                                compLevelId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post:  The selected composition and its associated data are added
         *        to the table showing the compositions the student plans
         *        to perform.
         */
        protected void btnAddComposition_Click(object sender, EventArgs e)
        {
            if (compositionDataValid())
            {
                Composition composition;
                string composer;
                double length;

                //if a new composer is being entered, get the name
                if (chkNewComposer.Checked)
                {
                    composer = txtComposerLast.Text;

                    //get first and middle initials, if entered
                    if (!txtComposerFI.Text.Equals(""))
                    {
                        composer = composer + ", " + txtComposerFI.Text + ".";

                        if (!txtComposerMI.Text.Equals(""))
                            composer = composer + txtComposerMI.Text + ".";
                    }
                    else if (!txtComposerMI.Text.Equals(""))
                        composer = composer + ", " + txtComposerMI.Text + ".";
                }
                //use existing composer
                else
                    composer = ddlComposer.Text;

                //if the user is adding a new composition, add to database and get data
                if (chkNewTitle.Checked)
                {
                    length = Convert.ToDouble(txtMinutes.Text) + Convert.ToDouble(ddlSeconds.SelectedValue);

                    composition = new Composition(txtComposition.Text, composer, ddlStyle.Text,
                                                 ddlCompLevel.Text, length);

                    if (composition.compositionId == -1)
                    {
                        lblErrorMsg.Text = "An error occurred while adding the composition";
                        lblErrorMsg.Visible = true;
                    }
                }
                else
                {
                    //get the composition title from the composition id
                    int compositionId = Convert.ToInt32(ddlComposition.Text);
                    composition = DbInterfaceComposition.GetComposition(compositionId);
                }

                if (composition != null)
                    addComposition(composition);
            }
        }

        /*
         * Pre:
         * Post: The data of the input composition is added to the
         *       composition table for the current audition
         * @param composition is the composition to be entered into the table
         */
        private void addComposition(Composition composition)
        {
            TableRow row = new TableRow();
            TableCell chkBoxCell = new TableCell();
            TableCell compId = new TableCell();
            TableCell comp = new TableCell();
            TableCell composer = new TableCell();
            TableCell style = new TableCell();
            TableCell level = new TableCell();
            TableCell time = new TableCell();
            CheckBox chkBox = new CheckBox();

            chkBoxCell.Controls.Add(chkBox);
            //save the id in an invisible cell for later access
            compId.Text = composition.compositionId.ToString();
            compId.Visible = false;

            //set cell text
            comp.Text = composition.title;
            composer.Text = composition.composer;
            style.Text = composition.style;
            level.Text = composition.compLevel;
            time.Text = composition.playingTime.ToString();

            //add cells to new row
            row.Cells.Add(chkBoxCell);
            row.Cells.Add(compId);
            row.Cells.Add(comp);
            row.Cells.Add(composer);
            row.Cells.Add(style);
            row.Cells.Add(level);
            row.Cells.Add(time);

            //add new row to table
            tblCompositions.Rows.Add(row);

            //save table to session variable as an array
            saveTableToSession(tblCompositions, compositionTable);
        }

        /*
         * Pre:
         * Post: Adds the entered coordinate data to the coordinate table.
         * @param id is the student id of the coordinating student
         * @param firstName is the first name
         * @param lastName is the last name
         * @param reason is the reason that coordination is needed between the studnets
         */
        private void addCoordinate(string id, string firstName, string lastName, string reason)
        {
            if (reason.ToUpper().Equals("DUET") || !coordinateExists(id))
            {
                TableRow row = new TableRow();
                //TableCell chkBoxCell = new TableCell();
                TableCell studIdCell = new TableCell();
                TableCell firstNameCell = new TableCell();
                TableCell lastNameCell = new TableCell();
                TableCell reasonCell = new TableCell();
                CheckBox chkBox = new CheckBox();

                //set cell values
               // chkBoxCell.Controls.Add(chkBox);
                studIdCell.Text = id;
                firstNameCell.Text = firstName;
                lastNameCell.Text = lastName;
                reasonCell.Text = reason;

                //add cells to new row
                //row.Cells.Add(chkBoxCell);
                row.Cells.Add(studIdCell);
                row.Cells.Add(firstNameCell);
                row.Cells.Add(lastNameCell);
                row.Cells.Add(reasonCell);

                //add new row to table
                tblCoordinates.Rows.Add(row);

                //save table to session variable as an array
                saveTableToSession(tblCoordinates, coordinateTable);
            }
        }

        /*
         * Pre:
         * Post: Determines whether the input student is already in the list of coordinates
         * param id is the id of the student being searched for
         */
        private bool coordinateExists(string id)
        {
            bool exists = false;
            int i = 1;

            //search for matching id
            while (i < tblCoordinates.Rows.Count && !exists)
            {
                if (tblCoordinates.Rows[i].Cells[0].Text.Equals(id) && !tblCoordinates.Rows[i].Cells[3].Text.Equals("DUET"))
                    exists = true;

                i++;
            }

            return exists;
        }

        /*
         * Pre:  
         * Post:  If no composition is selected, an error will be displayed.  Otherwise
         *        the selected composition(s) will be removed from the composition table
         *        for the audition
         */
        protected void btnRemoveComposition_Click(object sender, EventArgs e)
        {
            bool compositionSelected = false;

            //look at each row in the table. If the associated checkbox is checked, remove current row
            for (int i = 1; i < tblCompositions.Rows.Count; i++)
            {
                if (((CheckBox)tblCompositions.Rows[i].Cells[0].Controls[0]).Checked)
                {
                    tblCompositions.Rows.Remove(tblCompositions.Rows[i]);
                    lblRemoveError.Visible = false;
                    compositionSelected = true;
                    i--;
                }
            }
            //if no composition was selected, display error message
            if (!compositionSelected)
                lblRemoveError.Visible = true;
            //if a change was made, save the table in a session variable
            else
                saveTableToSession(tblCompositions, compositionTable);
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
         * Post:  Checks whether all required composition data is entered before
         *        adding to the table of compositions for the student
         * @returns true if all required information is entered and false otherwise
         */
        private bool compositionDataValid()
        {
            //check fields common between using an existing composition or entering a new one
            bool commonDataEntered = !ddlStyle.SelectedValue.ToString().Equals("") && !ddlCompLevel.SelectedValue.ToString().Equals("")
                                     && !(txtMinutes.Text.Equals("") && ddlSeconds.SelectedIndex == 0);

            //if a new composition is being entered, make sure all required fields are filled in
            if (chkNewTitle.Checked && (!commonDataEntered || txtComposition.Text.Equals("") ||
                (ddlComposer.SelectedIndex < 1 && txtComposerLast.Text.Equals(""))))
            {
                lblCompositionError.Text = "Please select a style and level, enter a composition and " +
                            "composition time, and make sure you have either selected an existing composer " +
                            "or entered a new one.  If you do not wish to add a new composition, uncheck " +
                            "the 'New Composition' box.";
                lblCompositionError.Visible = true;
            }
            else if (!chkNewTitle.Checked && (!commonDataEntered || ddlComposer.SelectedIndex < 1 ||
                     ddlComposition.SelectedIndex < 1))
            {
                lblCompositionError.Text = "Please select a style, level, composer, and composition";
                lblCompositionError.Visible = true;
            }
            else
            {
                lblCompositionError.Visible = false;
            }

            //make sure minutes are entered and greater than 0
            if (!txtMinutes.Text.Equals(""))
            {
                int num;
                bool isNum = Int32.TryParse(txtMinutes.Text, out num);

                if (!isNum || (isNum && num < 0))
                {
                    lblMinutesErrorMsg.Visible = true;
                    lblMinutesError.Visible = true;
                }
            }
            else
            {
                lblMinutesError.Visible = true;
            }

            return !lblCompositionError.Visible && !lblMinutesErrorMsg.Visible;
        }

        /*
         * Pre:
         * Post: If the "No Time Preference" checkbox is selected, disable the time fields. 
         *       If the "Time Preference" checkbox is selected, enable the time fields.
         */
        protected void rblTimePreference_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTimePrefError.Visible = false;

            //index 0 is "No Preference"
            if (rblTimePreference.SelectedIndex == 0)
            {
                pnlPreferredTime.Visible = false;
                rblTimeOptions.SelectedIndex = -1;
            }
            //index 1 is "Preference"
            else
                pnlPreferredTime.Visible = true;
        }

        /*
         * Pre:
         * Post:  First, the entered data is verified by ensuring that all required fields are filled
         *        in, the number and styles of compositions are sufficient for the selected Audition
         *        Track, that a duet partner is selected if the audition is a Duet, and that a preferred
         *        time is specified if the radio button is selected.
         *        If all information is valid, the audition is entered into the database.  If the audition
         *        is a duet, the duet partner also has an audition entered
         */
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            clearErrors();

            try
            {
                //verify all entered information and create/edit audition
                if (action != Utility.Action.Delete && verifyRequiredDataEntered() && verifyCompositionsToPerform() && verifyDuetPartner()
                    && verifyTimePreference() && auditionCreated() && noDuplicateAudition() && validTheoryLevel() && !checkFreezeDate())
                {
                    if (audition == null) resetAuditionVar();

                    string theoryLevel = ddlTheoryLevel.Text;
                    if (ddlTheoryLevelType.Visible)
                        theoryLevel = theoryLevel + "-" + ddlTheoryLevelType.SelectedValue.ToString();

                    audition.setAuditionInfo(ddlInstrument.SelectedValue, txtAccompanist.Text,
                        ddlAuditionType.SelectedValue, ddlAuditionTrack.SelectedValue,
                        Convert.ToInt32(ddlSite.SelectedValue), theoryLevel,
                        DbInterfaceStudentAudition.GetAuditionOrgId(Convert.ToInt32(ddlSite.SelectedValue), DateTime.Today.Year));

                    if (cboAudition.Visible)
                        audition.auditionId = Convert.ToInt32(cboAudition.SelectedValue);

                    addAuditionCompositions();
                    setAuditionTimeConstraints();
                    addAuditionCoordinates();

                    //make sure the audition doesn't already exist, add or update if it doesn't
                    if (action == Utility.Action.Add && DbInterfaceStudentAudition.AuditionExists(-1, audition.auditionOrgId,
                                  audition.yearId, audition.auditionTrack,
                                  audition.instrument, audition.auditionType))
                    {
                        lblErrorMsg.Text = "The audition already exists";
                    }
                    //add audition to database if it is being newly created
                    else if (action == Utility.Action.Add && audition.addToDatabase())
                        displaySuccessMessageAndOptions();
                    //update in database if the audition was edited
                    else if (action != Utility.Action.Add && action != Utility.Action.Delete && audition.updateInDatabase(coordinatesToRemove))
                    {
                        Session[coordsToRemove] = null;
                        coordinatesToRemove.Clear();
                        displaySuccessMessageAndOptions();
                    }
                    else
                    {
                        lblErrorMsg.Text = "An error occurred while updating the audition.  Please reload the audition to make sure everything was saved successfully.";
                        lblErrorMsg.Visible = true;
                    }
                }
                else if (action == Utility.Action.Delete) //delete audition
                {
                    pnlSuccess.Visible = false;
                    pnlFullPage.Visible = false;
                    pnlAreYouSure.Visible = true;
                }
                else
                {
                    lblErrorMsg.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMsg.Text = "An error occurred during the registration.";
                lblErrorMsg.Visible = true;

                Utility.LogError("District Registration", "btnRegister_Click", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
            }
        }

        /*
         * Pre:  An audition must have been selected
         * Post: The selected audition is deleted
         */
        private void deleteAudition()
        {
            if (audition == null) resetAuditionVar();

            audition.auditionId = Convert.ToInt32(cboAudition.SelectedValue);
            audition.auditionType = ddlAuditionType.SelectedValue;

            if (audition.deleteFromDatabase())
                displaySuccessMessageAndOptions();
            else
            {
                lblErrorMsg.Text = "An error occurred while deleting the audition.  Please attempt to reload the audition to make sure it was deleted.";
                lblErrorMsg.Visible = true;
            }
        }

        /*
        * Pre:
        * Post: Determines whether or not an audition has been created for the chosen district
        * @returns true if an audition has been created and false otherwise
        */
        private bool auditionCreated()
        {
            bool created = true;
            int year = DateTime.Today.Year;

            //get the year of the next audition
            if (DateTime.Today.Month >= 6)
                year = year + 1;

            year = DateTime.Today.Year; //delete this

            //see if the audition has been created
            if (DbInterfaceStudentAudition.GetAuditionOrgId(Convert.ToInt32(ddlSite.SelectedValue), year) == -1)
            {
                lblAuditionSiteError.Visible = true;
                lblErrorMsg.Text = "Errors on page";
                lblErrorMsg.Visible = true;
                created = false;
            }

            return created;
        }

        /*
         * Pre:
         * Post: Determines whether or not the database already contains an equivalent audition
         * @param returns true if creating this audition would create a duplicate and false otherwise
         */
        private bool noDuplicateAudition()
        {
            bool result = true;
            int auditionId = -1, auditionOrgId, yearId;

            if (Int32.TryParse(ddlSite.SelectedValue, out auditionOrgId) && Int32.TryParse(lblStudentId.Text, out yearId))
            {
                if (cboAudition.Visible) Int32.TryParse(cboAudition.SelectedValue, out auditionId);

                auditionOrgId = DbInterfaceStudentAudition.GetAuditionOrgId(Convert.ToInt32(ddlSite.SelectedValue),
                                                                     DateTime.Today.Year);
                yearId = DbInterfaceStudent.GetStudentYearId(Convert.ToInt32(lblStudentId.Text));

                result = !DbInterfaceStudentAudition.AuditionExists(auditionId, auditionOrgId, yearId, ddlAuditionTrack.SelectedValue,
                                                      ddlInstrument.SelectedValue, ddlAuditionType.SelectedValue);

                if (!result)
                {
                    lblErrorMsg.Text = "An audition with the same student, audition track, audition type, and instrument " +
                                       "already exists for the current year.";
                    lblErrorMsg.Visible = true;
                    result = false;
                }
            }
            else
            {
                lblErrorMsg.Text = "Please select an audition site and student.";
                lblErrorMsg.Visible = true;
                result = false;
            }

            return result;
        }

        /*
         * Pre:
         * Post: Determines whether or not the student is allowed to take the chosen theory level.
         *       If the student has gotten a 5 on the entered theory level or any higher theory level,
         *       they must go up at least one level
         * @returns true if the student is allowed to take the chosen theory level and false otherwise
         */
        private bool validTheoryLevel()
        {
            int studentId;
            bool result = false;

            if (Int32.TryParse(lblStudentId.Text, out studentId))
            {
                string theoryTest = ddlTheoryLevel.SelectedValue;
                result = DbInterfaceStudentAudition.CountTheoryLevel5s(studentId, theoryTest) < 2;

                if (!result)
                    lblTheoryLvl.Visible = true;
                else
                {
                    if ((!theoryTest.Equals("EA") && !theoryTest.Equals("EB")) || 
                        ((theoryTest.Equals("EA") || theoryTest.Equals("EB")) && ddlTheoryLevelType.SelectedIndex >= 1))
                    {
                        lblTheoryLevelError.Visible = false;
                    }
                    else
                    {
                        lblTheoryLevelError.Visible = true;
                        result = false;
                    }
                }
            }
            else
            {
                lblErrorMsg.Text = "Please select a student";
                lblErrorMsg.Visible = true;
            }

            return result;
        }

        /*
        * Pre:
        * Post: Recreates the audition in case the session variable is lost
        */
        private void resetAuditionVar()
        {
            int studentId;

            if (Int32.TryParse(lblStudentId.Text, out studentId))
            {
                Student student = loadStudentData(studentId);
                student.grade = txtGrade.Text;

                //create DistrictAudition object and save to session variable
                audition = new DistrictAudition(student);

                Session[auditionVar] = audition;
            }
            else
            {
                lblErrorMsg.Text = "Please select a student";
                lblErrorMsg.Visible = true;
            }
        }

        /*
         * Pre:
         * Post: All controls are hidden, the user is told that the audition was created,
         *       and are given the options to create another new audition or go back to
         *       the menu/welcome page
         */
        private void displaySuccessMessageAndOptions()
        {
            clearAll();
            Session[completed] = true;

            lblSuccess.Visible = true;
            pnlSuccess.Visible = true;
            pnlFullPage.Visible = false;
            pnlAreYouSure.Visible = false;

            if (audition != null && audition.auditionType != null && action == Utility.Action.Add && action != Utility.Action.Delete)
            {
                if (audition.auditionType.ToUpper().Equals("DUET"))
                    lblSuccess.Text = "The auditions for the student and their duet "
                                           + "partner were successfully created";
                else
                    lblSuccess.Text = "The audition was successfully created";
            }
            else if (audition != null && audition.auditionType != null && action != Utility.Action.Add && action != Utility.Action.Delete)
            {
                if (audition.auditionType.ToUpper().Equals("DUET"))
                    lblSuccess.Text = "The auditions for the student and their duet "
                                           + "partner were successfully updated";
                else
                    lblSuccess.Text = "The audition was successfully updated";
            }
            else if (audition != null && audition.auditionType != null && action == Utility.Action.Delete)
            {

                if (audition.auditionType.ToUpper().Equals("DUET"))
                    lblSuccess.Text = "The auditions for the student and their duet "
                                           + "partner were successfully deleted";
                else
                    lblSuccess.Text = "The audition was successfully deleted";
            }
        }

        /*
         * Pre:
         * Post: Adds the selected compositions to the audition if they have not
         *       already been added
         */
        private void addAuditionCompositions()
        {
            for (int i = 1; i < tblCompositions.Rows.Count; i++)
            {
                int id = Convert.ToInt32(tblCompositions.Rows[i].Cells[1].Text);
                string composition = tblCompositions.Rows[i].Cells[2].Text;
                string composer = tblCompositions.Rows[i].Cells[3].Text;
                string style = tblCompositions.Rows[i].Cells[4].Text;
                string level = tblCompositions.Rows[i].Cells[5].Text;
                double time = Convert.ToDouble(tblCompositions.Rows[i].Cells[6].Text);

                Composition comp = new Composition(id, composition, composer, style, level, time);
                AuditionCompositions audComp = new AuditionCompositions(comp, 0);

                //do not add duplicates
                if (!audition.compositions.Contains(audComp))
                    audition.addComposition(audComp);
            }
        }

        /*
         * Pre:
         * Post: Adds the chosen time constraints to the audition if they are specified.
         *       If no from or to times are specified, they are set to the minimum
         *       DateTime value.
         */
        private void setAuditionTimeConstraints()
        {
            bool am = rblTimeOptions.SelectedIndex == 0;
            bool pm = rblTimeOptions.SelectedIndex == 1;
            bool earliest = rblTimeOptions.SelectedIndex == 2;
            bool latest = rblTimeOptions.SelectedIndex == 3;

            audition.setTimeConstraints(am, pm, earliest, latest);
        }

        /*
         * Pre:
         * Post: Adds any selected coordinating students to the audition
         */
        private void addAuditionCoordinates()
        {
            audition.coordinates.Clear();

            for (int i = 1; i < tblCoordinates.Rows.Count; i++)
            {
                int studentId = Convert.ToInt32(tblCoordinates.Rows[i].Cells[0].Text);
                Student student = DbInterfaceStudent.LoadStudentData(studentId);
                string reason = tblCoordinates.Rows[i].Cells[3].Text;

                if (student != null)
                {
                    StudentCoordinate coordinate = new StudentCoordinate(student, reason, true, true);

                    //only add unique coordinates
                    if (!audition.coordinates.Contains(coordinate))
                        audition.addStudentCoordinate(coordinate);
                }
                else
                {
                    lblErrorMsg.Text = "An error occurred while adding the coordinate students";
                    lblErrorMsg.Visible = true;
                }
            }
        }

        /*
         * Pre:
         * Post: Indicates whether the selected styles and number of different styles are 
         *       sufficient for the student's grade and selected audition track
         * @returns true if the compositions selections are sufficient
         */
        private bool verifyCompositionsToPerform()
        {
            int grade = getEnteredGrade();
            string audTrack = ddlAuditionTrack.Text;
            List<string> selectedStyles = new List<string>();
            bool hasRequiredStyle = false;
            string requiredStyles = "";

            AudTrackRequirements requirements = new AudTrackRequirements(audTrack, grade);

            lblCompositionError.Visible = false;

            //make sure there are not extra compositions
            if (requirements.requiredNumStyles < tblCompositions.Rows.Count - 1)
            {
                lblCompositionError.Text = "There may only be " + requirements.requiredNumStyles +
                                           " compositions for the selected audition track";
                lblCompositionError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //add each unique style to a list
            for (int i = 1; i < tblCompositions.Rows.Count; i++)
            {
                if (!selectedStyles.Contains(tblCompositions.Rows[i].Cells[4].Text.ToString()))
                    selectedStyles.Add(tblCompositions.Rows[i].Cells[4].Text.ToString());
            }

            //make sure the number of styles is equal to the requirement
            if (!audTrack.Equals("D2") && requirements.requiredNumStyles > selectedStyles.Count)
            {
                lblCompositionError.Text = "There must be " + requirements.requiredNumStyles.ToString() +
                                           " unique styles for the selected audition track";
                lblCompositionError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //if there are no required styles, return the current result
            if (requirements.requiredStyles == null) return !lblErrorMsg.Visible;

            //make sure a composition of each required style has been chosen
            foreach (string style in requirements.requiredStyles)
            {
                requiredStyles = requiredStyles + style + ", ";

                if (selectedStyles.Contains(style))
                    hasRequiredStyle = true;
            }

            //if any required styles were not chosen, display an error message with the required styles
            if (!hasRequiredStyle)
            {
                requiredStyles = requiredStyles.Substring(0, requiredStyles.Length - 2);

                lblCompositionError.Text = "The selected audition track requires that there be compositions " +
                                            "of at least one of the following styles: " + requiredStyles;
                lblCompositionError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            return !lblErrorMsg.Visible;
        }

        /*
         * Pre:
         * Post: If the Audition Type is a duet, this method insures that a duet
         *       partner has been chosen and entered into the coordinates table
         */
        private bool verifyDuetPartner()
        {
            bool result = true;

            if (ddlAuditionType.SelectedValue.Equals("Duet"))
            {
                bool foundPartner = false;
                int i = 1;

                //search for a duet partner in the coordinates table
                while (i < tblCoordinates.Rows.Count)
                {
                    if (tblCoordinates.Rows[i].Cells[3].Text.ToUpper().Equals("DUET"))
                        foundPartner = true;

                    i++;
                }

                if (!foundPartner)
                {
                    pnlDuetPartner.Visible = true;
                    lblPartnerError.Text = "You must choose a duet partner.";
                    lblPartnerError.Visible = true;
                    lblErrorMsg.Visible = true;
                    result = false;
                }
            }

            return result;
        }

        /*
         * Pre:
         * Post: Verifies that if the user specifies that the student has a time preference
         *       that the time preference is entered/chosen.
         */
        private bool verifyTimePreference()
        {
            bool result = true;

            lblTimePrefError.Visible = false;

            //if the user signified that there is a time preference, look for one
            if (rblTimePreference.SelectedIndex == 1)
            {
                //if no preferred time option was selected, return false
                if (rblTimeOptions.SelectedIndex == -1)
                {
                    lblTimePrefError.Visible = true;
                    lblErrorMsg.Visible = true;
                    result = false;
                }
            }

            return result;
        }

        /*
         * Pre:
         * Post: If all required data is entered, returns true.  Otherwise 
         *       specific error messages are shown and returns false.
         */
        private bool verifyRequiredDataEntered()
        {
            lblErrorMsg.Visible = false;

            //an audition site must be selected
            if (ddlSite.SelectedIndex == 0)
            {
                lblSiteError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //a student must be selected
            if (lblStudentId.Text.Equals(""))
            {
                lblStudentSearchError.Visible = true;
                lblStudentSearchError.Text = "Please select a student";
                lblErrorMsg.Visible = true;
            }

            //a grade must be entered
            if (txtGrade.Text.Equals(""))
            {
                lblGradeError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //an instrument must be selected
            if (ddlInstrument.SelectedIndex == 0)
            {
                lblInstrumentError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //an audition type must be selected
            if (ddlAuditionType.SelectedIndex == 0)
            {
                lblAuditionTypeError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //an audition track must be selected
            if (ddlAuditionTrack.SelectedIndex == 0)
            {
                lblAuditionTrackError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //a theory level must be selected
            if (ddlTheoryLevel.SelectedIndex == 0)
            {
                lblTheoryLevelError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //an audition must be selected if the user is editing
            if (action != Utility.Action.Add && cboAudition.SelectedIndex == 0)
            {
                lblAuditionError2.Visible = true;
                lblErrorMsg.Visible = true;
            }

            return !lblErrorMsg.Visible;
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
         * Post:  Clears the Compositions to Perform section (minus any data
         *        in the gridview
         */
        protected void btnClearCompSearch_Click(object sender, EventArgs e)
        {
            clearCompositionSearch();
        }

        /*
         * Pre:
         * Post: Clears the Duet Partner Search section
         */
        protected void btnPartnerClear_Click(object sender, EventArgs e)
        {
            clearDuetPartner();
        }

        /*
         * Pre:
         * Post: All data on the page is cleared
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        /*
         * Pre:
         * Post: Clears all data on page
         */
        private void clearAll()
        {
            clearStudentSearch();
            clearStudentInformation();
            clearAuditionInformation();
            clearCompositionsToPerform();
            clearTimeConstraints();
            ddlSite.SelectedIndex = 0;
            lblAuditionDate.Text = "";
            lblSiteError.Visible = false;
            clearDuetPartner();
            //pnlInfo.Visible = false;
            pnlDuetPartner.Visible = false;
            lblErrorMsg.Visible = false;
            //clear student auditions
            cboAudition.Items.Clear();
            cboAudition.Items.Add(new ListItem("", ""));
        }

        /*
         * Pre:
         * Post: Clears all data except the student search section
         */
        private void clearAllExceptSearch()
        {
            clearStudentInformation();
            clearAuditionInformation();
            clearCompositionsToPerform();
            clearTimeConstraints();
            ddlSite.SelectedIndex = 0;
            lblAuditionDate.Text = "";
            lblSiteError.Visible = false;
            clearDuetPartner();
            pnlDuetPartner.Visible = false;
            lblErrorMsg.Visible = false;
            //clear student auditions
            cboAudition.Items.Clear();
            cboAudition.Items.Add(new ListItem("", ""));
        }
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
         * Post: Clears the Student Information section
         */
        private void clearStudentInformation()
        {
            lblStudentId.Text = "";
            lblName.Text = "";
            txtGrade.Text = "";
            lblGradeError.Visible = false;
            lblDistrict.Text = "";
            lblTeacher.Text = "";
        }

        /*
         * Pre:
         * Post: Clears the Audition Information section
         */
        private void clearAuditionInformation()
        {
            ddlInstrument.SelectedIndex = 0;
            lblInstrumentError.Visible = false;
            txtAccompanist.Text = "";
            txtAccompanist.Enabled = true;
            txtAccompanist.BackColor = Color.White;
            ddlAuditionType.SelectedIndex = 0;
            lblAuditionTypeError.Visible = false;
            lblDuetPartner.InnerText = "";
            lblDuetPartner.Visible = false;
            lnkChangePartner.Visible = false;
            ddlAuditionTrack.SelectedIndex = 0;
            lblAuditionTrackError.Visible = false;
            ddlTheoryLevel.SelectedIndex = 0;
            ddlTheoryLevelType.SelectedIndex = 0;
            ddlTheoryLevelType.Visible = false;
            lblTheoryLevelError.Visible = false;
            setValidAuditionTracks();
        }

        /*
         * Pre:
         * Post: Clears the Compositions to Perform section
         */
        private void clearCompositionsToPerform()
        {
            clearCompositionSearch();

            //clear the compositions saved in the table
            while (tblCompositions.Rows.Count > 1)
                tblCompositions.Rows.Remove(tblCompositions.Rows[tblCompositions.Rows.Count - 1]);


            Session[compositionTable] = null;
        }

        /*
         * Pre:
         * Post: Clears the composition search section
         */
        private void clearCompositionSearch()
        {
            ddlStyle.SelectedIndex = -1;
            ddlCompLevel.SelectedIndex = -1;
            ddlComposer.SelectedIndex = -1;
            ddlComposition.SelectedIndex = -1;
            ddlComposer.Visible = true;
            ddlComposition.Visible = true;
            txtComposition.Visible = false;
            lblCompositionInstruction.Visible = false;
            txtMinutes.Text = "";
            ddlSeconds.SelectedIndex = 0;
            txtComposerLast.Text = "";
            txtComposerFI.Text = "";
            txtComposerMI.Text = "";
            chkNewTitle.Checked = false;
            chkNewComposer.Checked = false;
            pnlComposer.Visible = false;
            lblCompositionError.Visible = false;
            lblRemoveError.Visible = false;
            ddlComposer.Visible = true;
            ddlComposition.Visible = true;
            txtComposition.Visible = false;
            lblMinutesError.Visible = false;
            lblMinutesErrorMsg.Visible = false;

            //reset the dropdowns with all data
            searchComposers("", "");
            searchCompositions("", "", "");
        }

        /*
         * Pre:
         * Post: Clears the Time Constraints section
         */
        private void clearTimeConstraints()
        {
            rblTimeOptions.SelectedIndex = -1;
            rblTimePreference.SelectedIndex = 0;
            lblTimePrefError.Visible = false;
            pnlPreferredTime.Visible = false;

            while (tblCoordinates.Rows.Count > 1)
                tblCoordinates.Rows.Remove(tblCoordinates.Rows[tblCoordinates.Rows.Count - 1]);

            Session[coordinateTable] = null;
        }

        /*
         * Pre:
         * Post: Clears the Duet Partner Search section
         */
        private void clearDuetPartner()
        {
            txtPartnerId.Text = "";
            txtPartnerFirstName.Text = "";
            txtPartnerLastName.Text = "";
            gvDuetPartner.DataSource = null;
            Session[partnerSearch] = null;
        }

        /*
         * Pre:
         * Post: Hides all error messages on the page
         */
        private void clearErrors()
        {
            lblSiteError.Visible = false;
            lblAuditionSiteError.Visible = false;
            lblStudentSearchError.Visible = false;
            lblGradeError.Visible = false;
            lblInstrumentError.Visible = false;
            lblAuditionTypeError.Visible = false;
            lblPartnerError.Visible = false;
            lblAuditionError.Visible = false;
            lblAuditionError2.Visible = false;
            lblAuditionTrackError.Visible = false;
            lblTheoryLevelError.Visible = false;
            lblTheoryLvl.Visible = false;
            lblCompositionError.Visible = false;
            lblRemoveError.Visible = false;
            lblTimePrefError.Visible = false;
            lblErrorMsg.Visible = false;
            lblFreezeDatePassed.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
        }

        /*
         * Pre:
         * Post: Get the audition date
         */
        protected void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            setAuditionDate();
            checkFreezeDate();
        }

        /*
         * Pre:
         * Post: Get the audition date for the current district
         */
        private void setAuditionDate()
        {
            int year = DateTime.Today.Year;

            if (DateTime.Today.Month >= 6) year = year + 1;
            year = DateTime.Today.Year; //delete this

            if (!ddlSite.SelectedValue.ToString().Equals(""))
                getAuditionDate(Convert.ToInt32(ddlSite.SelectedValue), year);
            else
                lblAuditionDate.Text = "";

            lblSiteError.Visible = false;
        }

        /*
        * Pre:
        * Post: Determine whether the freeze date has already passed for the selected audition
        */
        private bool checkFreezeDate()
        {
            bool freezeDatePassed = false;
            int year = DateTime.Today.Year;

            if (DateTime.Today.Month >= 6) year = year + 1;
            year = DateTime.Today.Year; //delete this

            if (!ddlSite.SelectedIndex.ToString().Equals(""))
            {
                DateTime freezeDate;
                int districtId = Convert.ToInt32(ddlSite.SelectedValue);
                string freezeDateStr = DbInterfaceAudition.GetAuditionFreezeDate(districtId, year);

                if (DateTime.TryParse(freezeDateStr, out freezeDate))
                {
                    if (DateTime.Today > freezeDate)
                    {
                        freezeDatePassed = true;
                        lblFreezeDatePassed.Visible = true;
                    }
                    else
                        lblFreezeDatePassed.Visible = false;
                }
            }

            return freezeDatePassed;
        }

        /*
         * Pre:
         * Post: Get the audition date
         */
        private void getAuditionDate(int auditionSiteId, int year)
        {
            lblAuditionDate.Text = DbInterfaceAudition.GetAuditionDate(auditionSiteId, year);
        }

        /*
         * Pre:
         * Post: The user is brought back to the Welcome Screen without saving 
         *       the current audition registration
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        protected void ddlAuditionTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAuditionTrackError.Visible = false;
            setTheoryLevel();
        }

        /*
         * Pre:
         * Post: The information associated with the selected audition is loaded to the page
         */
        protected void cboAudition_SelectedIndexChanged(object sender, EventArgs e)
        {
            //clear existing info on page
            clearAuditionInformation();
            clearCompositionsToPerform();
            clearTimeConstraints();
            clearDuetPartner();
            pnlDuetPartner.Visible = false;
            lblAuditionError2.Visible = false;
            lblErrorMsg.Visible = false;

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
                            ddlInstrument.SelectedIndex =
                                ddlInstrument.Items.IndexOf(ddlInstrument.Items.FindByText(districtAudition.instrument));
                            txtAccompanist.Text = districtAudition.accompanist;
                            ddlAuditionType.SelectedIndex =
                                ddlAuditionType.Items.IndexOf(ddlAuditionType.Items.FindByText(districtAudition.auditionType));
                            ddlAuditionTrack.SelectedIndex =
                                ddlAuditionTrack.Items.IndexOf(ddlAuditionTrack.Items.FindByText(districtAudition.auditionTrack));
                            ddlSite.SelectedValue = districtAudition.districtId.ToString();
                            setAuditionDate();

                            //load valid theory levels
                            setTheoryLevel();

                            //get previously selected theory level
                            setTheoryLevel(student.theoryLevel);

                            //load compositions
                            foreach (AuditionCompositions comp in districtAudition.compositions)
                                addComposition(comp.composition);

                            //load time constraints
                            if (districtAudition.am)
                            {
                                rblTimePreference.SelectedIndex = 1;
                                pnlPreferredTime.Visible = true;
                                rblTimeOptions.SelectedIndex = 0;
                            }
                            else if (districtAudition.pm)
                            {
                                rblTimePreference.SelectedIndex = 1;
                                pnlPreferredTime.Visible = true;
                                rblTimeOptions.SelectedIndex = 1;
                            }
                            else if (districtAudition.earliest)
                            {
                                rblTimePreference.SelectedIndex = 1;
                                rblTimeOptions.SelectedIndex = 2;
                                pnlPreferredTime.Visible = true;
                            }
                            else if (districtAudition.latest)
                            {
                                rblTimePreference.SelectedIndex = 1;
                                rblTimeOptions.SelectedIndex = 3;
                                pnlPreferredTime.Visible = true;
                            }

                            //If there are coordinates, make the coordinate section visible
                            if (districtAudition.coordinates.Count > 0)
                                pnlCoordinateParticipants.Visible = true;

                            //load coordinates - if duet partner, put name by audition type dropdown
                            foreach (StudentCoordinate coord in districtAudition.coordinates)
                            {
                                addCoordinate(coord.student.id.ToString(), coord.student.firstName,
                                              coord.student.lastName, coord.reason);

                                if (coord.reason.ToUpper().Equals("DUET"))
                                {
                                    lblDuetPartner.InnerText = "Partner: " + coord.student.firstName +
                                                               " " + coord.student.lastName + " (" +
                                                               coord.student.id + ")";
                                    txtPartnerId.Text = coord.student.id.ToString();
                                    txtPartnerFirstName.Text = coord.student.firstName;
                                    txtPartnerLastName.Text = coord.student.lastName;
                                    lblDuetPartner.Visible = true;
                                    lnkChangePartner.Visible = true;
                                }
                            }
                        }
                        else
                        {
                            lblAuditionError.InnerText = "An error occurred while loading the audition information";
                            lblAuditionError.Visible = true;
                        }
                    }
                    else
                    {
                        lblStudentSearchError.Text = "Please reselect the student";
                        lblStudentSearchError.Visible = true;
                    }
                }
            }

            if (ddlInstrument.Text.Equals("Organ") || ddlInstrument.Text.Equals("Piano"))
            {
                txtAccompanist.Text = "";
                txtAccompanist.Enabled = false;
                txtAccompanist.BackColor = Color.LightGray;
            }
            else
            {
                txtAccompanist.Enabled = true;
                txtAccompanist.BackColor = Color.White;
            }
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

            ddlTheoryLevelType.Visible = false;

            if (level.Length > 2) 
                level = level.Substring(0, 2);            

            ListItem selectedItem = ddlTheoryLevel.Items.FindByValue(level);
            if (selectedItem != null)
                ddlTheoryLevel.SelectedIndex = ddlTheoryLevel.Items.IndexOf(selectedItem);

            if (level.Equals("EA") || level.Equals("EB"))
            {
                ddlTheoryLevelType.Visible = true;

                if (theoryLevel.Length > 2)
                    theoryLevelType = theoryLevel.Substring(3);

                selectedItem = ddlTheoryLevelType.Items.FindByValue(theoryLevelType);
                if (selectedItem != null)
                    ddlTheoryLevelType.SelectedIndex = ddlTheoryLevelType.Items.IndexOf(selectedItem);
            }
        }

        /*
         * Pre:
         * Post: Initialize the page for adding or editing based on user selection
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;

            if (ddlUserOptions.SelectedValue.Equals("Create New"))
            {
                action = Utility.Action.Add;
                pnlChooseAudition.Visible = false;
                btnRegister.Text = "Register";
                enableControls();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Edit Existing"))
            {
                action = Utility.Action.Edit;
                pnlChooseAudition.Visible = true;
                btnRegister.Text = "Submit";
                enableControls();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Delete Existing"))
            {
                action = Utility.Action.Delete;
                pnlChooseAudition.Visible = true;
                btnRegister.Text = "Delete";
                disableControls();
            }
        }

        /*
         * Pre:
         * Post: All controls that are disabled when deleting an audition
         *       are enabled
         */
        private void enableControls()
        {
            txtGrade.Enabled = true;
            ddlSite.Enabled = true;
            ddlInstrument.Enabled = true;
            txtAccompanist.Enabled = true;
            ddlAuditionType.Enabled = true;
            lnkChangePartner.Enabled = true;
            ddlAuditionTrack.Enabled = true;
            ddlTheoryLevel.Enabled = true;
            ddlStyle.Enabled = true;
            ddlCompLevel.Enabled = true;
            ddlComposer.Enabled = true;
            chkNewComposer.Enabled = true;
            ddlComposition.Enabled = true;
            chkNewTitle.Enabled = true;
            btnAddComposition.Enabled = true;
            btnRemoveComposition.Enabled = true;
            rblTimePreference.Enabled = true;
            rblTimeOptions.Enabled = true;
        }

        /*
         * Pre:
         * Post: All controls that should not be edited when deleting
         *       an audition are disabled
         */
        private void disableControls()
        {
            txtGrade.Enabled = false;
            ddlSite.Enabled = false;
            ddlInstrument.Enabled = false;
            txtAccompanist.Enabled = false;
            ddlAuditionType.Enabled = false;
            lnkChangePartner.Enabled = false;
            ddlAuditionTrack.Enabled = false;
            ddlTheoryLevel.Enabled = false;
            ddlStyle.Enabled = false;
            ddlCompLevel.Enabled = false;
            ddlComposer.Enabled = false;
            chkNewComposer.Enabled = false;
            ddlComposition.Enabled = false;
            chkNewTitle.Enabled = false;
            btnAddComposition.Enabled = false;
            btnRemoveComposition.Enabled = false;
            rblTimePreference.Enabled = false;
            rblTimeOptions.Enabled = false;
        }

        /*
       * Pre:
       * Post: The user is redirected to the Welcome Screen
       */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        protected void ddlTheoryLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTheoryLevelError.Visible = false;

            if (ddlTheoryLevel.Text.Equals("EA") || ddlTheoryLevel.Text.Equals("EB"))
                ddlTheoryLevelType.Visible = true;
            else
                ddlTheoryLevelType.Visible = false;
        }

        /*
         * Pre:
         * Post:  Show a textbox if the user selects to enter a new composer and a dropdown otherwise
         */
        protected void chkNewComposer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNewComposer.Checked)
            {
                ddlComposer.Visible = false;
                pnlComposer.Visible = true;

                //if a new composer is being entered, the composition must be new
                chkNewTitle.Checked = true;
                ddlComposition.Visible = false;
                txtComposition.Visible = true;
                lblCompositionInstruction.Visible = true;
            }
            else
            {
                pnlComposer.Visible = false;
                ddlComposer.Visible = true;
            }
        }

        /*
         * Pre:
         * Post:  Show a textbox if the user selects to enter a new composition and a dropdown otherwise
         */
        protected void chkNewTitle_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNewTitle.Checked)
            {
                ddlComposition.Visible = false;
                txtComposition.Visible = true;
                lblCompositionInstruction.Visible = true;

                //show all composers
                DataTable table = DbInterfaceComposition.GetComposerSearchResults("", "");

                if (table != null)
                {
                    ddlComposer.DataSource = null;
                    ddlComposer.Items.Clear();
                    ddlComposer.Items.Add(new ListItem("", ""));
                    ddlComposer.DataSource = table;
                    ddlComposer.DataBind();
                }
                else
                {
                    lblCompositionError.Text = "An error occurred while refreshing the composers.";
                    lblCompositionError.Visible = true;
                }
            }
            else
            {
                //don't allow user to use existing title if they have chosen to enter a new composer
                if (!chkNewComposer.Checked)
                {
                    txtComposition.Visible = false;
                    ddlComposition.Visible = true;
                    lblCompositionInstruction.Visible = false;

                    //filter composers
                    DataTable table = DbInterfaceComposition.GetComposerSearchResults(ddlStyle.SelectedValue, ddlCompLevel.SelectedValue);

                    if (table != null)
                    {
                        ddlComposer.DataSource = null;
                        ddlComposer.Items.Clear();
                        ddlComposer.Items.Add(new ListItem("", ""));
                        ddlComposer.DataSource = table;
                        ddlComposer.DataBind();
                    }
                    else
                    {
                        lblCompositionError.Text = "An error occurred while filtering the composers.";
                        lblCompositionError.Visible = true;
                    }
                }
                else
                    chkNewTitle.Checked = true;
            }
        }

        /*
         * Pre:
         * Post: The selected audition is deleted
         */
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            deleteAudition();
        }

        /*
         * Pre:
         * Post: The audition is not deleted and all data is shown
         */
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            pnlFullPage.Visible = true;
            pnlSuccess.Visible = false;
            pnlAreYouSure.Visible = false;
        }
    }
}
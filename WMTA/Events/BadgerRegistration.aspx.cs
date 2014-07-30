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
    public partial class BadgerRegistration : System.Web.UI.Page
    {
        private StateAudition audition;
        private Utility.Action action;
        private List<StudentCoordinateSimple> coordinatesToRemove; //keeps track of coordinates that need to be removed from the audition
        //session variables
        private string compositionTable = "CompositionTable";
        private string coordinateTable = "CoordinateTable";
        private string coordinateSearch = "CoordinateData";
        private string coordsToRemove = "CoordinatesToRemove";
        private string studentSearch = "StudentData";
        private string auditionVar = "Audition";

        protected void Page_Load(object sender, EventArgs e)
        {
            initializeAction();
            coordinatesToRemove = new List<StudentCoordinateSimple>();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[compositionTable] = null;
                Session[coordinateTable] = null;
                Session[coordinateSearch] = null;
                Session[coordsToRemove] = null;
                Session[studentSearch] = null;
                Session[auditionVar] = null;

                checkPermissions();
                initializePage();
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
                audition = (StateAudition)Session[auditionVar];
        }

        /*
         * Pre:
         * Post: The action being performed is initialized.  Defaulted to Add
         */
        private void initializeAction()
        {
            string actionIndicator = Request.QueryString["action"];
            if (actionIndicator == null || actionIndicator.Equals(""))
            {
                action = Utility.Action.Add;
            }
            else
            {
                action = (Utility.Action)Convert.ToInt32(actionIndicator);
            }
        }

        /*
         * Pre:
         * Post: Initialize the page for adding, editing, or deleting based on user selection
         */
        protected void initializePage()
        {
            //initialize page based on action
            if (action == Utility.Action.Add)
            {
                upStudentSearch.Visible = false;
            }
            else if (action == Utility.Action.Edit)
            {
                legend.InnerText = "Edit Badger Registration";
            }
            else if (action == Utility.Action.Delete)
            {
                legend.InnerText = "Delete Badger Registration";
                disableControls();

                btnSubmit.Attributes.Add("onclick", "return confirm('Are you sure that you wish to permanently delete this audition and all associated data?');");
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
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students are displayed that match the search criteria (student id, first name, and last name).
         *        The error message is also reset.
         */
        protected void btnStudentSearch_Click(object sender, EventArgs e)
        {
            int num;
            string id = txtStudentId.Text;
            bool isNum = int.TryParse(id, out num);

            if (isNum || txtStudentId.Text.Equals(""))
            {
                User user = (User)Session[Utility.userRole];
                int districtId = -1;

                //if district admin, get their district because that is all the students they can register
                if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S')) && user.permissionLevel.Contains('D'))
                {
                    districtId = user.districtId;
                    searchStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, districtId);
                }
                else if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S') ||
                           user.permissionLevel.Contains('D')) && user.permissionLevel.Contains('T'))
                {
                    searchOwnStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, ((User)Session[Utility.userRole]).contactId);
                }
                else if (user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S'))
                {
                    searchStudents(gvStudentSearch, id, txtFirstName.Text, txtLastName.Text, studentSearch, districtId);
                }
            }
            else
            {
                clearGridView(gvStudentSearch);
                showWarningMessage("A Student Id must be numeric.");
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
         * @param district is the district to search students in, -1 indicates that all districts should be searched
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

                Utility.LogError("Badger Registration", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
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

                Utility.LogError("Badger Registration", "searchOwnStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
         * Pre:   The selected index must be a positive number less than the number of rows
         *        in the gridView
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlInfo.Visible = true;
            clearAllExceptSearch();

            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                txtStudentId.Text = gvStudentSearch.Rows[index].Cells[1].Text;
                loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));
            }
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
                {
                    cell.BackColor = Color.Black;
                    cell.ForeColor = Color.White;
                }
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

                //get general student information
                if (student != null)
                {
                    lblStudentId.Text = studentId.ToString();
                    txtFirstName.Text = student.firstName;
                    txtLastName.Text = student.lastName;
                    lblName.Text = student.lastName + ", " + student.firstName + " " + student.middleInitial;
                    lblGrade.Text = student.grade;
                    lblDistrict.Text = student.getDistrict();
                    lblTeacher.Text = student.getCurrTeacher();

                    //make sure the student is in at least 4th grade
                    verifyAge();

                    //get eligible auditions
                    if (action == Utility.Action.Edit)
                    {
                        DataTable table = DbInterfaceStudentAudition.GetStateAuditionsForDropdown(student);
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
                        {
                            showWarningMessage("This student has no state auditions to edit.");
                        }
                    }
                    else
                    {
                        cboAudition.DataBind();
                    }
                }
                else
                {
                    showErrorMessage("Error: There was an error loading the student data.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: There was an error loading the student data.");

                Utility.LogError("Badger Registration", "loadStudentData", "studentId: " + studentId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
                DataTable data = (DataTable)Session[studentSearch];
                gvStudentSearch.DataSource = data;
                gvStudentSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Badger Registration", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
         * Post:  The audition information associated with the selected district
         *        audition is loaded to the page
         */
        protected void cboAudition_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!cboAudition.SelectedValue.ToString().Equals(""))
                {
                    clearCompositions();
                    clearCompetitionInfo();

                    int auditionId = Convert.ToInt32(cboAudition.SelectedValue);
                    int studentId = Convert.ToInt32(Convert.ToInt32(txtStudentId.Text));
                    Student student = DbInterfaceStudent.LoadStudentData(studentId);

                    //get all audition info associated with audition id
                    if (student != null)
                    {
                        DistrictAudition districtAudition = DbInterfaceStudentAudition.GetStudentDistrictAudition(auditionId, student);

                        if (districtAudition != null)
                        {
                            loadInfoToPage(districtAudition);

                            //create StateAudition object and save to session variable
                            audition = new StateAudition(districtAudition);
                            Session[auditionVar] = audition;

                            //load regional audition options based on geo id and instrument
                            DataTable table = DbInterfaceStudentAudition.GetStateSites(audition.instrument);
                            cboSite.DataSource = null;
                            cboSite.Items.Clear();
                            cboSite.DataSourceID = "";


                            if (table.Rows.Count > 0)
                            {
                                cboSite.DataSource = table;
                                cboSite.DataValueField = "AuditionOrgId";
                                cboSite.DataTextField = "GeoName";
                                cboSite.Items.Add(new ListItem(""));
                                cboSite.DataBind();
                            }
                            else
                            {
                                showWarningMessage("No audition sites have been created.");
                            }
                        }
                        else
                        {
                            showErrorMessage("An error occurred while loading the audition.");
                        }

                        //if an audition is being edited load regional site, drive time, time constraints, and coordinates
                        if (action == Utility.Action.Edit)
                        {
                            audition = DbInterfaceStudentAudition.GetStudentStateAudition(districtAudition,
                                                                   Convert.ToInt32(cboAudition.SelectedValue));

                            if (audition != null)
                            {
                                Session[auditionVar] = audition;

                                cboSite.SelectedIndex =
                                        cboSite.Items.IndexOf(cboSite.Items.FindByValue(audition.auditionOrgId.ToString()));
                                txtDriveTime.Text = audition.driveTime.ToString();

                                //load time constraints
                                setTimePreference(audition.am, audition.pm, audition.earliest, audition.latest, false);

                                //If there are coordinates, make the coordinate section visible
                                if (audition.coordinates.Count > 0)
                                    pnlCoordinateParticipants.Visible = true;

                                //load coordinates - if duet partner, put name by audition type dropdown
                                foreach (StudentCoordinate coord in audition.coordinates)
                                {
                                    addCoordinate(coord.student.id.ToString(), coord.student.firstName,
                                                  coord.student.lastName, coord.reason);
                                }
                            }
                            else
                            {
                                showErrorMessage("Error: The audition information could not be loaded.");
                            }
                        }
                    }
                    else
                    {
                        showErrorMessage("Error: An error occurred while updating the page.");
                    }
                }
            }
            catch (Exception ex)
            {
                showErrorMessage("Error: An error occurred while updating the page.");

                Utility.LogError("Badger Registration", "cboAudition_SelectedIndexChanged", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Set the preferred audition time if there is one
         * @param am indicates whether a morning time is preferred
         * @param pm indicates whether an afternoon time is preferred
         * @param early indicates whether the earliest possible time is preffered
         * @param late indicates whether the latest possible time is preferred
         */
        private void setTimePreference(bool am, bool pm, bool early, bool late, bool hasPreference)
        {
            //determine whether or no there is a preference
            if (am || pm || early || late || hasPreference)
            {
                pnlPreferredTime.Visible = true;

                rblTimePreference.SelectedIndex = 1;
            }
            else if (!hasPreference)
            {
                pnlPreferredTime.Visible = false;

                rblTimePreference.SelectedIndex = 0;
            }

            //set time preference
            opAM.Checked = am;
            opPM.Checked = pm;
            opEarly.Checked = early;
            opLate.Checked = late;
        }

        /*
         * Pre:
         * Post: The district audition information is loaded to the page
         */
        private void loadInfoToPage(DistrictAudition districtAudition)
        {
            //load student info
            lblStudentId.Text = districtAudition.student.id.ToString();
            lblName.Text = districtAudition.student.lastName + ", " + districtAudition.student.firstName +
                           " " + districtAudition.student.middleInitial;
            lblGrade.Text = districtAudition.student.grade;
            lblDistrict.Text = districtAudition.student.getDistrict();
            lblTeacher.Text = districtAudition.student.getCurrTeacher();

            //load audition info
            lblInstrument.Text = districtAudition.instrument;
            lblAccompanist.Text = districtAudition.accompanist;
            lblAuditionType.Text = districtAudition.auditionType;

            //load duet partner if type is duet
            if (districtAudition.auditionType.ToUpper().Equals("DUET"))
            {
                pnlCoordinateParticipants.Visible = true;

                List<StudentCoordinate> coords = districtAudition.coordinates;
                Student partner = coords.ElementAt(0).student;
                int id = partner.id;
                string firstName = partner.firstName;
                string lastName = partner.lastName;

                //add duet partner as duet partner and carpool
                addCoordinate(id.ToString(), firstName, lastName, "Duet");
                addCoordinate(id.ToString(), firstName, lastName, "Carpool");
            }

            //load compositions
            foreach (AuditionCompositions comp in districtAudition.compositions)
                addComposition(comp.composition);

            //TODO load regional audition options based on geo id and instrument
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
            TableRow row = new TableRow();
            TableCell studIdCell = new TableCell();
            TableCell firstNameCell = new TableCell();
            TableCell lastNameCell = new TableCell();
            TableCell reasonCell = new TableCell();
            CheckBox chkBox = new CheckBox();

            //set cell values
            studIdCell.Text = id;
            firstNameCell.Text = firstName;
            lastNameCell.Text = lastName;
            reasonCell.Text = reason;

            //add cells to new row
            row.Cells.Add(studIdCell);
            row.Cells.Add(firstNameCell);
            row.Cells.Add(lastNameCell);
            row.Cells.Add(reasonCell);

            //add new row to table
            tblCoordinates.Rows.Add(row);

            //save table to session variable as an array
            saveTableToSession(tblCoordinates, coordinateTable);
        }

        /*
         * Pre:
         * Post: The input composition information is added to the table 
         *       of compositions
         * @param composition holds the composition information
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: All data on page is cleared
         */
        private void clearPage()
        {
            cboAudition.Items.Clear();
            cboAudition.Items.Add(new ListItem("", ""));
            lblAuditionDate.Text = "";
            clearStudentSearch();
            clearStudentInfo();
            clearCompetitionInfo();
            clearCompositions();
            clearTimeConstraints();
            chkAdditionalInfo.Checked = false;
            pnlAdditionalInfo.Visible = false;

            if (action != Utility.Action.Add)
            {
                upStudentSearch.Visible = true;
                pnlFullPage.Visible = false;
            }
        }

        /*
         * Pre:
         * Post: All data on page except student search is cleared
         */
        private void clearAllExceptSearch()
        {
            cboAudition.Items.Clear();
            cboAudition.Items.Add(new ListItem("", ""));
            lblAuditionDate.Text = "";
            clearStudentInfo();
            clearCompetitionInfo();
            clearCompositions();
            clearTimeConstraints();
            chkAdditionalInfo.Checked = false;
            pnlAdditionalInfo.Visible = false;
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
        }

        /*
         * Pre:
         * Post: Clears all information in the student information section
         */
        private void clearStudentInfo()
        {
            lblStudentId.Text = "";
            lblName.Text = "";
            lblGrade.Text = "";
            lblDistrict.Text = "";
            lblTeacher.Text = "";
        }

        /*
         * Pre:
         * Post: Clears all information in the Competition Information section 
         *       except for the selected audition
         */
        private void clearCompetitionInfo()
        {
            cboSite.SelectedIndex = 0;
            txtDriveTime.Text = "";
            lblInstrument.Text = "";
            lblAccompanist.Text = "";
            lblAuditionType.Text = "";

            //clear the students saved in the table
            for (int i = 1; i < tblCoordinates.Rows.Count; i++)
                tblCompositions.Rows.Remove(tblCoordinates.Rows[i]);

            Session[coordinateTable] = null;
        }

        /*
         * Pre:
         * Post: Clears the compositions in the composition table as well
         *       as the session variable
         */
        private void clearCompositions()
        {
            //clear the compositions saved in the table
            while (tblCompositions.Rows.Count > 1)
                tblCompositions.Rows.Remove(tblCompositions.Rows[tblCompositions.Rows.Count - 1]);


            Session[compositionTable] = null;
        }

        /*
         * Pre:
         * Post: Clears the Time Constraints section
         */
        private void clearTimeConstraints()
        {
            setTimePreference(false, false, false, false, false);

            while (tblCoordinates.Rows.Count > 1)
                tblCoordinates.Rows.Remove(tblCoordinates.Rows[tblCoordinates.Rows.Count - 1]);

            Session[coordinateTable] = null;
        }

        /*
         * Pre:
         * Post: If the "No Time Preference" checkbox is selected, disable the time fields. 
         *       If the "Time Preference" checkbox is selected, enable the time fields.
         */
        protected void rblTimePreference_SelectedIndexChanged(object sender, EventArgs e)
        {
            //index 0 is "No Preference"
            if (rblTimePreference.SelectedIndex == 0)
            {
                setTimePreference(false, false, false, false, false);
            }
            //index 1 is "Preference"
            else
            {
                setTimePreference(false, false, false, false, true);
            }
        }

        /*
         * Pre:
         * Post: If the user chooses to view additional information, the Audition
         *       Information and Compositions to Performs sections will be shown
         */
        protected void chkAdditionalInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAdditionalInfo.Checked)
                pnlAdditionalInfo.Visible = true;
            else
                pnlAdditionalInfo.Visible = false;
        }

        /*
         * Pre:
         * Post:  First, the entered data is verified by ensuring that all required fields are filled
         *        in.  If all information is valid, the audition is entered into the database.  If the 
         *        audition is a duet, the duet partner also has an audition entered
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //verify all entered information and create audition
                if (action != Utility.Action.Delete && verifyRequiredDataEntered() && verifyTimePreference() && verifyAge() && duetsAllowed())
                {
                    if (audition == null) resetAuditionVar();

                    audition.siteId = Convert.ToInt32(cboSite.SelectedValue);
                    audition.driveTime = Convert.ToInt32(txtDriveTime.Text);

                    setAuditionTimeConstraints();
                    addAuditionCoordinates();

                    //make sure the audition doesn't already exist, add or update if it doesn't
                    if (action == Utility.Action.Add && DbInterfaceStudentAudition.AuditionExists(-1, audition.auditionOrgId,
                                  audition.yearId, audition.districtAudition.auditionTrack,
                                  audition.districtAudition.instrument, audition.districtAudition.auditionType))
                    {
                        showWarningMessage("The audition already exists.");
                    }
                    //add audition to database if it is being newly created
                    else if (action == Utility.Action.Add && audition.addToDatabase())
                        displaySuccessMessage();
                    //update in database if the audition was edited
                    else if (action == Utility.Action.Edit && audition.updateInDatabase(coordinatesToRemove))
                    {
                        Session[coordsToRemove] = null;
                        coordinatesToRemove.Clear();
                        displaySuccessMessage();
                    }
                    else
                    {
                        showErrorMessage("Error: An error occurred.");
                    }
                }
                else if (action == Utility.Action.Delete) //delete audition
                {
                    deleteAudition();
                }
            }
            catch (Exception ex)
            {
                showErrorMessage("Error: An error occurred during the registration.");

                Utility.LogError("Badger Registration", "btnSubmit_Click", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
            }
        }

        /*
        * Pre:
        * Post: Recreates the audition in case the session variable is lost
        */
        private void resetAuditionVar()
        {
            if (!cboAudition.SelectedValue.ToString().Equals(""))
            {
                Student student = loadStudentData(Convert.ToInt32(lblStudentId.Text));

                //create DistrictAudition object and save to session variable
                audition = new StateAudition(Convert.ToInt32(cboAudition.SelectedValue), student, true);

                Session[auditionVar] = audition;
            }
        }

        /*
         * Pre:
         * Post: If all required data is entered, returns true.  Otherwise 
         *       specific error messages are shown and returns false.
         */
        private bool verifyRequiredDataEntered()
        {
            bool valid = true;

            //a student must be selected
            if (lblStudentId.Text.Equals(""))
            {
                showWarningMessage("Please select a student.");
                valid = false;
            }

            else if (Convert.ToInt32(txtDriveTime.Text) < 0)
            {
                showWarningMessage("The drive time must be a positive integer.");
                valid = false;
            }

            return valid;
        }

        /*
         * Pre:
         * Post: Verifies that if the user specifies that the student has a time preference
         *       that the time preference is entered/chosen.
         */
        private bool verifyTimePreference()
        {
            bool result = true;

            //if the user signified that there is a time preference, look for one
            if (rblTimePreference.SelectedIndex == 1)
            {
                //if no preferred time option was selected, return false
                if (!opAM.Checked && !opPM.Checked && !opEarly.Checked && !opLate.Checked)
                {
                    showWarningMessage("Please choose the preferred time or select 'No Preference'.");

                    result = false;
                }
            }

            return result;
        }

        /*
         * Pre:
         * Post: Verifies that the student is old enough to participate in the state competition (at least 4th grade)
         * @returns true if the student is old enough and false otherwise
         */
        private bool verifyAge()
        {
            string grade = lblGrade.Text;
            bool result = true;

            if (!grade.Equals("") && (grade.Equals("1") || grade.Equals("2") || grade.Equals("3") || grade.Substring(0, 1).Equals("K")))
            {
                showWarningMessage("Students must be in at least 4th grade to register.");
                result = false;
            }

            return result;
        }

        /*
         * Pre:
         * Post:  Verifies that duets are allowed at the chosen audition site
         * @returns true if the audition is a solo or if duets are enabled for the chosen audition site
         */
        private bool duetsAllowed()
        {
            bool allowed = true;

            //if the audition is a duet, make sure duets are enabled at the chosen site
            if (lblAuditionType.Text.ToUpper().Equals("DUET"))
            {
                if (!DbInterfaceAudition.StateSiteAllowsDuets(DateTime.Today.Year, Convert.ToInt32(cboSite.SelectedValue)))
                {
                    showWarningMessage("The chosen audition site does not allow duets.");
                    allowed = false;
                }
            }

            return allowed;
        }

        /*
         * Pre:
         * Post: Adds the chosen time constraints to the audition if they are specified.
         *       If no from or to times are specified, they are set to the minimum
         *       DateTime value.
         */
        private void setAuditionTimeConstraints()
        {
            bool am = opAM.Checked;
            bool pm = opPM.Checked;
            bool earliest = opEarly.Checked;
            bool latest = opLate.Checked;

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
                    StudentCoordinate coordinate = new StudentCoordinate(student, reason, true, false);

                    //only add unique coordinates
                    if (!audition.coordinates.Contains(coordinate))
                        audition.addStudentCoordinate(coordinate);
                }
                else
                {
                    showErrorMessage("Error: An error occurred while adding the coordinates");
                }
            }
        }

        /*
         * Pre:
         * Post: The user is shown a success message and the page is cleared
         */
        private void displaySuccessMessage()
        {
            clearPage();

            if (audition != null && audition.districtAudition != null && audition.districtAudition.auditionType != null
                && action == Utility.Action.Add)
            {
                if (audition.districtAudition.auditionType.ToUpper().Equals("DUET"))
                    showSuccessMessage("The student and their duet partner were successfully registered.");
                else
                    showSuccessMessage("The student was successfully registered.");

                clearPage();
            }
            else if (audition != null && audition.districtAudition != null && audition.districtAudition.auditionType != null
                     && action == Utility.Action.Edit)
            {
                if (audition.districtAudition.auditionType.ToUpper().Equals("DUET"))
                    showSuccessMessage("The auditions for the student and their duet partner were successfully updated.");
                else
                    showSuccessMessage("The audition was successfully updated.");

                clearPage();
            }
            else if (audition != null && audition.auditionType != null && action == Utility.Action.Delete)
            {
                if (audition.auditionType.ToUpper().Equals("DUET"))
                    showSuccessMessage("The auditions for the student and their duet partner were successfully deleted.");
                else
                    showSuccessMessage("The audition was successfully deleted.");
            }
        }
        /*
         * Pre:
         * Post: All controls that should not be edited when deleting
         *       an audition are disabled
         */
        private void disableControls()
        {
            cboSite.Enabled = false;
            txtDriveTime.Enabled = false;
            opAM.Disabled = true;
            opPM.Disabled = true;
            opEarly.Disabled = true;
            opLate.Disabled = true;
            rblTimePreference.Enabled = false;
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
         * Post: Get the audition date
         */
        protected void cboSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboSite.SelectedValue.ToString().Equals(""))
            {
                lblAuditionDate.Text = DbInterfaceAudition.GetAuditionDate(Convert.ToInt32(cboSite.SelectedValue));
                audition.auditionOrgId = Convert.ToInt32(cboSite.SelectedValue);
            }
            else
                lblAuditionDate.Text = "";
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Badger Registration", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }

        /*
         * Pre:  An audition must have been selected
         * Post: The selected audition is deleted
         */
        private void deleteAudition()
        {
            if (audition == null) resetAuditionVar();

            audition.auditionId = Convert.ToInt32(cboAudition.SelectedValue);
            audition.districtAudition.auditionType = lblAuditionType.Text;

            if (audition.deleteFromDatabase())
                displaySuccessMessage();
            else
            {
                showErrorMessage("Error: An error occurred while deleting the audition.  Please try to reload the audition to make sure it was deleted.");
            }
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
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
    public partial class BadgerRegistration : System.Web.UI.Page
    {
        StateAudition audition;
        private List<StudentCoordinateSimple> coordinatesToRemove; //keeps track of coordinates that need to be removed from the audition
        //session variables
        private string compositionTable = "CompositionTable";
        private string coordinateTable = "CoordinateTable";
        private string coordinateSearch = "CoordinateData";
        private string coordsToRemove = "CoordinatesToRemove";
        private string studentSearch = "StudentData";
        private string completed = "Completed";
        private string auditionVar = "Audition";
        private string creatingNew = "CreatingNew"; //tracks whether an audition is being created or edited
        private string deleting = "Deleting";       //tracks whether an audition is being created, edited, or deleted

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            coordinatesToRemove = new List<StudentCoordinateSimple>();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[compositionTable] = null;
                Session[coordinateTable] = null;
                Session[coordinateSearch] = null;
                Session[coordsToRemove] = null;
                Session[studentSearch] = null;
                Session[completed] = null;
                Session[auditionVar] = null;
                Session[creatingNew] = null;
                Session[deleting] = null;
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

            if (Page.IsPostBack && Session[completed] != null && (bool)Session[completed])
            {
                pnlSuccess.Visible = true;
                pnlFullPage.Visible = false;
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[auditionVar] != null)
                audition = (StateAudition)Session[auditionVar];
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
            string id = txtStudentId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            lblStudentSearchError.Text = "";

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
                lblStudentSearchError.ForeColor = Color.Red;
                lblStudentSearchError.Text = "A Student Id must be numeric";
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
                    lblStudentSearchError.Text = "An error occurred during the search";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search";
                lblStudentSearchError.Visible = true;

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
                    lblStudentSearchError.Text = "An error occurred during the search";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search";
                lblStudentSearchError.Visible = true;

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
            lblAuditionError.Visible = false;
            lblSiteError.Visible = false;
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
                    if (!((bool)Session[creatingNew]))
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
                            lblAuditionError.InnerText = "This student has no state auditions to edit";
                            lblAuditionError.Visible = true;
                        }
                    }
                    else
                    {
                        cboAudition.DataBind();
                    }
                }
                else
                {
                    lblErrorMsg.Text = "There was an error loading the student data.";
                    lblErrorMsg.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "There was an error loading the student data.";
                lblErrorMsg.Visible = true;

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
            lblAuditionSelectError.Visible = false;

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
                                lblSiteError.InnerText = "No audition sites have been created";
                                lblSiteError.Visible = true;
                            }
                        }
                        else
                        {
                            lblSiteError.InnerText = "An error occurred while loading the audition.";
                            lblSiteError.Visible = true;
                        }

                        if (Session[creatingNew] == null) Session[creatingNew] = !cboAudition.Visible;

                        //if an audition is being edited load regional site, drive time, time constraints, and coordinates
                        if (!((bool)Session[creatingNew]))
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
                                if (audition.coordinates.Count > 0)
                                    pnlCoordinateParticipants.Visible = true;

                                //load coordinates - if duet partner, put name by audition type dropdown
                                foreach (StudentCoordinate coord in audition.coordinates)
                                {
                                    addCoordinate(coord.student.id.ToString(), coord.student.firstName,
                                                  coord.student.lastName, coord.reason);

                                    /*if (coord.reason.ToUpper().Equals("DUET"))
                                    {
                                        lblDuetPartner.InnerText = "Partner: " + coord.student.firstName +
                                                                   " " + coord.student.lastName + " (" +
                                                                   coord.student.id + ")";
                                        lblDuetPartner.Visible = true;
                                        lnkChangePartner.Visible = true;
                                    }*/
                                }
                            }
                            else
                            {
                                lblAuditionError.InnerText = "The audition information could not be loaded";
                                lblAuditionError.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        lblErrorMsg.Text = "An error occurred while updating the page.";
                        lblErrorMsg.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMsg.Text = "An error occurred while updating the page.";
                lblErrorMsg.Visible = true;

                Utility.LogError("Badger Registration", "cboAudition_SelectedIndexChanged", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
            }
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
            clearErrors();

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
            //pnlInfo.Visible = false;
        }

        /*
         * Pre:
         * Post: All data on page except student search is cleared
         */
        private void clearAllExceptSearch()
        {
            clearErrors();

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
            lblStudentSearchError.Text = "";
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
            rblTimeOptions.SelectedIndex = -1;
            rblTimePreference.SelectedIndex = 0;
            lblTimePrefError.Visible = false;
            pnlPreferredTime.Visible = false;

            //clear the students saved in the table
            for (int i = 1; i < tblCoordinates.Rows.Count; i++)
                tblCompositions.Rows.Remove(tblCoordinates.Rows[i]);

            Session[coordinateTable] = null;
        }

        /*
         * Pre:
         * Post: Hides all error messages on the page
         */
        private void clearErrors()
        {
            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
            lblSiteError.Visible = false;
            lblSiteError2.Visible = false;
            lblStudentSearchError.Visible = false;
            lblTimePrefError.Visible = false;
            lblDriveTimeError.Visible = false;
            lblAuditionError.Visible = false;
            lblAuditionSelectError.Visible = false;
            lblStudentTooYoung.Visible = false;
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
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            clearErrors();

            try
            {
                //verify all entered information and create audition
                if (!(bool)Session[deleting] && verifyRequiredDataEntered() && verifyTimePreference() && verifyAge() && duetsAllowed())
                {
                    if (audition == null) resetAuditionVar();

                    audition.siteId = Convert.ToInt32(cboSite.SelectedValue);
                    audition.driveTime = Convert.ToInt32(txtDriveTime.Text);

                    setAuditionTimeConstraints();
                    addAuditionCoordinates();

                    //make sure the audition doesn't already exist, add or update if it doesn't
                    if ((bool)Session[creatingNew] && DbInterfaceStudentAudition.AuditionExists(-1, audition.auditionOrgId,
                                  audition.yearId, audition.districtAudition.auditionTrack,
                                  audition.districtAudition.instrument, audition.districtAudition.auditionType))
                    {
                        lblErrorMsg.Text = "The audition already exists";
                        lblErrorMsg.Visible = true;
                    }
                    //add audition to database if it is being newly created
                    else if ((bool)Session[creatingNew] && audition.addToDatabase())
                        displaySuccessMessageAndOptions();
                    //update in database if the audition was edited
                    else if (!((bool)Session[creatingNew]) && !((bool)Session[deleting]) && audition.updateInDatabase(coordinatesToRemove))
                    {
                        Session[coordsToRemove] = null;
                        coordinatesToRemove.Clear();
                        displaySuccessMessageAndOptions();
                    }
                    else
                    {
                        lblErrorMsg.Text = "An error occurred";
                        lblErrorMsg.Visible = true;
                    }
                }
                else if ((bool)Session[deleting]) //delete audition
                {
                    pnlSuccess.Visible = false;
                    pnlFullPage.Visible = false;
                    pnlAreYouSure.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMsg.Text = "An error occurred during the registration.";
                lblErrorMsg.Visible = true;

                Utility.LogError("Badger Registration", "btnRegister_Click", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
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
            lblErrorMsg.Visible = false;

            //a student must be selected
            if (lblStudentId.Text.Equals(""))
            {
                lblStudentSearchError.Visible = true;
                lblStudentSearchError.Text = "Please select a student";
                lblErrorMsg.Visible = true;
            }

            //an audition must be selected
            if (cboAudition.SelectedIndex == 0)
            {
                lblAuditionSelectError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //an audition site must be selected
            if (cboSite.SelectedIndex == 0)
            {
                lblSiteError2.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //a drive time must be entered and greater than 0
            if (txtDriveTime.Text.Equals(""))
            {
                lblDriveTimeError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            else if (Convert.ToInt32(txtDriveTime.Text) < 0)
            {
                txtDriveTime.Text = "The drive time must be a positive number";
                txtDriveTime.Visible = true;
                lblErrorMsg.Visible = true;
            }

            return !lblErrorMsg.Visible;
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
         * Post: Verifies that the student is old enough to participate in the state competition (at least 4th grade)
         * @returns true if the student is old enough and false otherwise
         */
        private bool verifyAge()
        {
            string grade = lblGrade.Text;
            bool result = true;

            if (!grade.Equals("") && (grade.Equals("1") || grade.Equals("2") || grade.Equals("3") || grade.Substring(0, 1).Equals("K")))
            {
                lblStudentTooYoung.Visible = true;
                lblErrorMsg.Visible = true;
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
                    lblErrorMsg.Text = "The chosen audition site does not allow duets";
                    lblErrorMsg.Visible = true;
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
                    StudentCoordinate coordinate = new StudentCoordinate(student, reason, true, false);

                    //only add unique coordinates
                    if (!audition.coordinates.Contains(coordinate))
                        audition.addStudentCoordinate(coordinate);
                }
                else
                {
                    lblErrorMsg.Text = "An error occurred while adding the coordinates";
                    lblErrorMsg.Visible = true;
                }
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
            clearPage();
            Session[completed] = true;

            pnlSuccess.Visible = true;
            pnlFullPage.Visible = false;
            lblSuccess.Visible = true;

            if (audition != null && audition.districtAudition != null && audition.districtAudition.auditionType != null
                && (bool)Session[creatingNew] && !(bool)Session[deleting])
            {
                if (audition.districtAudition.auditionType.ToUpper().Equals("DUET"))
                    lblSuccess.Text = "The student and their duet partner were successfully registered";
                else
                    lblSuccess.Text = "The student was successfully registered";
            }
            else if (audition != null && audition.districtAudition != null && audition.districtAudition.auditionType != null
                     && !((bool)Session[creatingNew]) && !((bool)Session[deleting]))
            {
                if (audition.districtAudition.auditionType.ToUpper().Equals("DUET"))
                    lblSuccess.Text = "The auditions for the student and their duet "
                                           + "partner were successfully updated";
                else
                    lblSuccess.Text = "The audition was successfully updated";
            }
            else if (audition != null && audition.auditionType != null && (bool)Session[deleting])
            {
                if (audition.auditionType.ToUpper().Equals("DUET"))
                    lblSuccess.Text = "The auditions for the student and their duet "
                                           + "partner were successfully deleted";
                else
                    lblSuccess.Text = "The audition was successfully deleted";

                pnlAreYouSure.Visible = false;
            }
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
         * Pre:
         * Post: Initialize the page for adding or editing, based
         *       off of the user's choice
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            clearPage();

            if (ddlUserOptions.SelectedValue.Equals("Create New"))
            {
                Session[creatingNew] = true;
                Session[deleting] = false;
                btnRegister.Text = "Register";
                enableControls();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Edit Existing"))
            {
                Session[creatingNew] = false;
                Session[deleting] = false;
                btnRegister.Text = "Submit";
                enableControls();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Delete Existing"))
            {
                Session[creatingNew] = false;
                Session[deleting] = true;
                btnRegister.Text = "Delete";
                disableControls();
            }

            Session[completed] = false;
            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;
            pnlInfo.Visible = true;
        }

        /*
         * Pre:
         * Post: All controls that are disabled when deleting an audition
         *       are enabled
         */
        private void enableControls()
        {
            cboSite.Enabled = true;
            txtDriveTime.Enabled = true;
            rblTimeOptions.Enabled = true;
            rblTimePreference.Enabled = true;
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
            rblTimeOptions.Enabled = false;
            rblTimePreference.Enabled = false;
        }

        /*
         * Pre:
         * Post: The user is redirected to the Welcome Screen
         */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
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

            lblSiteError2.Visible = false;
        }

        protected void txtDriveTime_TextChanged(object sender, EventArgs e)
        {
            lblDriveTimeError.Visible = false;
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

            //show error label
            lblErrorMsg.Text = "An error occurred";
            lblErrorMsg.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
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
         * Pre:  An audition must have been selected
         * Post: The selected audition is deleted
         */
        private void deleteAudition()
        {
            if (audition == null) resetAuditionVar();

            audition.auditionId = Convert.ToInt32(cboAudition.SelectedValue);
            audition.districtAudition.auditionType = lblAuditionType.Text;

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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Reporting
{
    public partial class FullDataDump : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                checkPermissions();

                loadYearDropdown();
                loadDistrictDropdown();
                loadTeacherDropdown();
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
         * Pre:
         * Post: Loads the appropriate years in the dropdown
         */
        private void loadYearDropdown()
        {
            int firstYear = DbInterfaceStudentAudition.GetFirstAuditionYear();

            for (int i = DateTime.Now.Year + 1; i >= firstYear; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        /*
         * Pre:
         * Post:  If the current user is not an administrator, the district
         *        dropdowns are filtered to containing only the current
         *        user's district
         */
        private void loadDistrictDropdown()
        {
            User user = (User)Session[Utility.userRole];

            if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S'))) //if the user is a district admin, add only their district
            {
                //get own district dropdown info
                string districtName = DbInterfaceStudent.GetStudentDistrict(user.districtId);

                //add new item to dropdown and select it
                ddlDistrictSearch.Items.Add(new ListItem(districtName, user.districtId.ToString()));
                ddlDistrictSearch.SelectedIndex = 1;
            }
            else //if the user is an administrator, add all districts
            {
                ddlDistrictSearch.DataSource = DbInterfaceAudition.GetDistricts();

                ddlDistrictSearch.DataTextField = "GeoName";
                ddlDistrictSearch.DataValueField = "GeoId";

                ddlDistrictSearch.DataBind();
            }
        }

        /*
         * Pre:
         * Post: If the current user is a teacher, the teacher dropdown
         *       should only show the current user
         */
        private void loadTeacherDropdown()
        {
            if (HighestPermissionTeacher())
            {
                User user = (User)Session[Utility.userRole];
                Contact contact = DbInterfaceContact.GetContact(user.contactId);

                if (contact != null)
                {
                    ddlTeacher.Items.Add(new ListItem(contact.lastName + ", " + contact.firstName, user.contactId.ToString()));
                }
            }
        }

        /*
         * Pre:
         * Post: Determines whether or not the current user's highest permission level is Teacher
         * @returns true if the current user's highest permission level is Teacher and false otherwise
         */
        private bool HighestPermissionTeacher()
        {
            User user = (User)Session[Utility.userRole];
            bool teacherOnly = false;

            if (user.permissionLevel.Contains('T') && !(user.permissionLevel.Contains('D') || user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
            {
                teacherOnly = true;
            }

            return teacherOnly;
        }

        /*
         * Pre:
         * Post: Load the auditions for the selected event.  Filter on teacher if a teacher was selected
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            int districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);
            int auditionOrgId = DbInterfaceAudition.GetAuditionOrgId(districtId, year);

            if (auditionOrgId != -1)
            {
                int teacherId = 0;
                if (ddlTeacher.SelectedIndex >= 0 && !ddlTeacher.SelectedValue.Equals(""))
                    teacherId = Convert.ToInt32(ddlTeacher.SelectedValue);

                // Update gridview
                Dictionary<int, StateAudition> auditions = DbInterfaceAudition.GetFullDataDump(auditionOrgId, teacherId, year, districtId);
                DataTable table = PopulateTable(auditions);
                gvAuditions.DataSource = table;
                gvAuditions.DataBind();
            }
            else
            {
                showWarningMessage("No audition exists matching the input criteria.");
            }
        }

        /*
         * Populate the ridiculous huge table with audition data
         */
        private DataTable PopulateTable(Dictionary<int, StateAudition> auditions)
        {
            DataTable table = CreateTable();

            foreach (int auditionId in auditions.Keys)
            {
                StateAudition stateAudition = auditions[auditionId];
                DistrictAudition audition = auditions[auditionId].districtAudition;

                // Assign composition and coordinate variables since there is a variable number of each
                int compositionPoints = 0;
                string comp1 = "", composer1 = "", period1 = "", comp2 = "", composer2 = "", period2 = "", comp3 = "", composer3 = "", period3 = "", 
                    coordName1 = "", coordReason1 = "",  coordName2 = "", coordReason2 = "", coordName3 = "", coordReason3 = "", coordName4 = "", 
                    coordReason4 = "", coordName5 = "", coordReason5 = "";
 
                AssignCompositionVariables(audition.compositions, 0, ref comp1, ref composer1, ref period1, ref compositionPoints);
                AssignCompositionVariables(audition.compositions, 1, ref comp2, ref composer2, ref period2, ref compositionPoints);
                AssignCompositionVariables(audition.compositions, 2, ref comp3, ref composer3, ref period3, ref compositionPoints);
                AssignCoordinateVariables(audition.simpleCoordinates, 0, ref coordName1, ref coordReason1);
                AssignCoordinateVariables(audition.simpleCoordinates, 1, ref coordName2, ref coordReason2);
                AssignCoordinateVariables(audition.simpleCoordinates, 2, ref coordName3, ref coordReason3);
                AssignCoordinateVariables(audition.simpleCoordinates, 3, ref coordName4, ref coordReason4);
                AssignCoordinateVariables(audition.simpleCoordinates, 4, ref coordName5, ref coordReason5);
                bool badgerEligible = compositionPoints >= 14 && audition.theoryPoints >= 4 && verifyAge(audition.student.grade); 

                // Get the time preference
                string timePref = "";
                if (audition.am)
                    timePref = "AM";
                else if (audition.pm)
                    timePref = "PM";
                else if (audition.earliest)
                    timePref = "Earliest";
                else if (audition.latest)
                    timePref = "Latest";

                AddRow(table, audition.student.id, audition.student.lastName, audition.student.firstName, audition.student.grade, audition.instrument,
                    audition.auditionType, audition.auditionTrack, audition.theoryLevel, audition.student.teacherName, timePref, comp1, composer1,
                    period1, comp2, composer2, period2, comp3, composer3, period3, audition.student.totalPoints - compositionPoints - audition.theoryPoints - stateAudition.points,
                    audition.startTime, audition.room, compositionPoints, audition.theoryPoints, compositionPoints + audition.theoryPoints, audition.awards, badgerEligible, 
                    stateAudition.points, compositionPoints + audition.theoryPoints + stateAudition.points, audition.student.totalPoints, stateAudition.awards, 
                    coordName1, coordReason1, coordName2, coordReason2, coordName3, coordReason3, coordName4, coordReason4, coordName5, coordReason5);
            }

            return table;
        }

        /*
         * Pre:
         * Post: Verifies that the student is old enough to participate in the state competition (at least 4th grade)
         * @returns true if the student is old enough and false otherwise
         */
        private bool verifyAge(string grade)
        {
            bool result = true;

            if (!grade.Equals("") && (grade.Equals("1") || grade.Equals("2") || grade.Equals("3") || grade.Substring(0, 1).Equals("K")))
            {
                result = false;
            }

            return result;
        }

        private void AssignCompositionVariables(List<AuditionCompositions> compositions, int indexToCheck, ref string composition, ref string composer, ref string period, ref int points)
        {
            if (compositions.Count() > indexToCheck)
            {
                composition = compositions.ElementAt(indexToCheck).composition.title;
                composer = compositions.ElementAt(indexToCheck).composition.composer;
                period = compositions.ElementAt(indexToCheck).composition.style;
                points += compositions.ElementAt(indexToCheck).points;
            }
        }

        private void AssignCoordinateVariables(List<StudentCoordinateSimple> coordinates, int indexToCheck, ref string coordinateName, ref string coordinateReason)
        {
            if (coordinates.Count() > indexToCheck)
            {
                coordinateName = coordinates.ElementAt(indexToCheck).coordinateName;
                coordinateReason = coordinates.ElementAt(indexToCheck).reason;
            }
        }

        private void AddRow(DataTable table, int studentId, string lastName, string firstName, string grade, string instrument, string type, string track, 
            string theory, string teacher, string timePref, string comp1, string composer1, string period1, string comp2, string composer2, 
            string period2, string comp3, string compser3, string period3, int previousPoints, string room, string auditionTime, int districtCompPoints,
            int theoryPoints, int totalDistrictPoints, string districtAwards, bool badgerEligible, int badgerPoints, int totalYearPoints, int totalPoints, string badgerAwards,
            string coordName1, string coordReason1, string coordName2, string coordReason2, string coordName3, string coordReason3, string coordName4,
            string coordReason4, string coordName5, string coordReason5)
        {
            table.Rows.Add(studentId, lastName, firstName, grade, instrument, type, track, theory, teacher, timePref, comp1, composer1, period1, comp2, composer2,
             period2, comp3, compser3, period3, previousPoints, room, auditionTime, districtCompPoints, theoryPoints, totalDistrictPoints, districtAwards, 
             badgerEligible, badgerPoints, totalYearPoints, totalPoints, badgerAwards, coordName1, coordReason1, coordName2, coordReason2, coordName3, coordReason3, coordName4,
             coordReason4, coordName5, coordReason5);
        }

        /*
         * Create all needed columns in the table
         */
        private DataTable CreateTable()
        {
            DataTable table = new DataTable();

            // Base audition info
            table.Columns.Add("Student Id", typeof(int));
            table.Columns.Add("Last Name", typeof(string));
            table.Columns.Add("First Name", typeof(string));
            table.Columns.Add("Grade", typeof(string));
            table.Columns.Add("Instrument", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Track", typeof(string));
            table.Columns.Add("Theory", typeof(string));
            table.Columns.Add("Teacher", typeof(string));
            table.Columns.Add("Time Req", typeof(string));
            table.Columns.Add("Composition 1", typeof(string));
            table.Columns.Add("Composer 1", typeof(string));
            table.Columns.Add("Period 1", typeof(string));
            table.Columns.Add("Composition 2", typeof(string));
            table.Columns.Add("Composer 2", typeof(string));
            table.Columns.Add("Period 2", typeof(string));
            table.Columns.Add("Composition 3", typeof(string));
            table.Columns.Add("Composer 3", typeof(string));
            table.Columns.Add("Period 3", typeof(string));

            // Points info
            table.Columns.Add("Prev Points", typeof(int));
            table.Columns.Add("Room", typeof(string));
            table.Columns.Add("Aud Time", typeof(string));
            table.Columns.Add("Points", typeof(int));
            table.Columns.Add("Theory Pts", typeof(int));
            table.Columns.Add("Total District Pts", typeof(int));
            table.Columns.Add("District Awards", typeof(string));
            table.Columns.Add("Badger?", typeof(string));
            table.Columns.Add("Badger Pts", typeof(int));
            table.Columns.Add("Total Yr Pts", typeof(int));
            table.Columns.Add("Total Points", typeof(int));
            table.Columns.Add("Badger Awards", typeof(string));

            // Space for coordinates at the end
            table.Columns.Add("Coordinate 1", typeof(string));
            table.Columns.Add("Coord Reason 1", typeof(string));
            table.Columns.Add("Coordinate 2", typeof(string));
            table.Columns.Add("Coord Reason 2", typeof(string));
            table.Columns.Add("Coordinate 3", typeof(string));
            table.Columns.Add("Coord Reason 3", typeof(string));
            table.Columns.Add("Coordinate 4", typeof(string));
            table.Columns.Add("Coord Reason 4", typeof(string));
            table.Columns.Add("Coordinate 5", typeof(string));
            table.Columns.Add("Coord Reason 5", typeof(string));

            return table;
        }

        protected void ddlDistrictSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HighestPermissionTeacher())
                updateTeacherDropdown();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HighestPermissionTeacher())
                updateTeacherDropdown();
        }

        /*
         * Pre:
         * Post: Update the list of available teachers
         */
        private void updateTeacherDropdown()
        {
            ddlTeacher.DataSource = null;
            ddlTeacher.DataBind();
            ddlTeacher.Items.Clear();

            if (ddlDistrictSearch.SelectedIndex > 0)
            {
                int year = Convert.ToInt32(ddlYear.SelectedValue);
                int districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);

                DataTable table = DbInterfaceContact.GetTeachersForEvent(districtId, year);

                if (table != null)
                {
                    //add empty item
                    ddlTeacher.Items.Add(new ListItem("", ""));

                    //add teachers from district
                    ddlTeacher.DataSource = table;

                    ddlTeacher.DataTextField = "ComboName";
                    ddlTeacher.DataValueField = "ContactId";

                    ddlTeacher.DataBind();
                }
                else
                {
                    showErrorMessage("Error: The teachers for the selected event could not be retrieved.");
                }
            }
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

        #endregion
    }
}
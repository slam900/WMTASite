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
    public partial class ScheduleUpdate : System.Web.UI.Page
    {
        // Session variables
        private string auditionSearch = "AuditionData"; //tracks data returned by latest audition search
        private string scheduleData = "ScheduleData";
        private string eventSchedule = "EventSchedule";
        private string judgeTimesSession = "JudgeTimes";  // Tracks the available time for each judge
        private Dictionary<int, int> judgeTimeAllowances;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                checkPermissions();

                Session[auditionSearch] = null;
                Session[scheduleData] = null;
                Session[eventSchedule] = null;
                Session[judgeTimesSession] = null;
                loadYearDropdown();
                loadDistrictDropdown();
            }
            else
            {
                judgeTimeAllowances = (Dictionary<int, int>)Session[judgeTimesSession];
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
                Response.Redirect("../Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("A")))
                    Response.Redirect("../Default.aspx");
            }
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
         *        dropdown is filtered to contain only the current
         *        user's district
         */
        private void loadDistrictDropdown()
        {
            User user = (User)Session[Utility.userRole];

            if (!user.permissionLevel.Contains('A')) //if the user is a district admin, add only their district
            {
                //get own district dropdown info
                string districtName = DbInterfaceStudent.GetStudentDistrict(user.districtId);

                //add new items to dropdown
                ddlDistrictSearch.Items.Add(new ListItem(districtName, user.districtId.ToString()));

                ddlDistrictSearch.SelectedIndex = 1;

                //load the audition
                selectAudition();
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
         * Post: The entered audition is searched for and selected.  If a matching schedule
         *       slot is found, some summary data is shown on the page
         */
        //protected void btnSelectAudition_Click(object sender, EventArgs e)
        //{
        //    int auditionId = -1;

        //    if (Int32.TryParse(txtAuditionId.Text, out auditionId))
        //    {
        //        ScheduleSlot scheduleSlot = DbInterfaceStudentAudition.GetStudentAuditionSchedule(auditionId);

        //        if (scheduleSlot != null)
        //        {
        //            lblAuditionInformation.Text = "Student: " + scheduleSlot.StudentName + ", Start Time: " + FormatTime(scheduleSlot.StartTime) /*+ ", Judge: " + scheduleSlot.JudgeName*/;
        //            lblSelectedAuditionId.Text = auditionId.ToString();
        //        }
        //        else
        //        {
        //            showWarningMessage("No matching audition schedule slots were found.");
        //        }
        //    }
        //    else
        //    {
        //        showWarningMessage("Please enter a numeric audition id.");
        //    }
        //}

        /*
         * Pre:
         * Post: Returns a AM/PM string of the input time
         */
        private string FormatTime(TimeSpan time)
        {
            int hour = time.Hours;
            string minutes = time.Minutes.ToString();
            string amPm = "AM";

            if (hour > 12)
            {
                hour = hour - 12;
                amPm = "PM";
            }
            else if (hour == 12)
                amPm = "PM";

            // Add 0 in front of single-digit minutes
            if (minutes.Length == 1)
                minutes = "0" + minutes;

            return hour + ":" + minutes + " " + amPm;
        }

        /*
         * Pre:
         * Post: Load the current times of the selected judge or clear the times if no judge is selected
         */
        //protected void ddlAuditionJudges_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlAuditionJudges.SelectedIndex > 0)
        //        LoadJudgeTimes(Convert.ToInt32(ddlAuditionJudges.SelectedValue));
        //}

        /*
         * Pre:
         * Post: Load the current times of the judge with the input id or clear the times if no judge is selected
         */
        //private void LoadJudgeTimes(int judgeId)
        //{
        //    ddlTimes.Items.Clear();
        //    ddlTimes.Items.Add(new ListItem("", ""));

        //    List<Tuple<int, string>> judgeTimes = DbInterfaceScheduling.LoadJudgeTimes(judgeId, Convert.ToInt32(lblAuditionId.Text));

        //    foreach (Tuple<int, string> time in judgeTimes)
        //    {
        //        ddlTimes.Items.Add(new ListItem(time.Item2, time.Item1.ToString()));
        //    }
        //}

        /*
         * Pre:
         * Post: Switch the selected audition to the chosen time and update the schedule table
         */
        protected void btnMoveAudition_Click(object sender, EventArgs e)
        {
            // Make sure two valid slots have been entered
            if (SlotValid(txtSlot.Text) && SlotValid(txtNewSlot.Text))
            {
                int currentSlot = Int32.Parse(txtSlot.Text);
                int newSlot = Int32.Parse(txtNewSlot.Text);
                if (currentSlot == newSlot)
                    return;

                DataTable schedule = (DataTable)Session[scheduleData];

                // Get the table indexes for the two slots we are moving and temporarily remove one of the duet records so the indexing doesn't get messed up
                int currentSlotTableIndex = GetTableIndexForSlot(schedule, currentSlot);
                bool isDuet = schedule.Rows[currentSlotTableIndex]["Type"].ToString().ToUpper().Equals("DUET");
                string[] tempRowArray = new string[schedule.Columns.Count]; // Temporary table to hold duet record, if needed

                if (isDuet)
                {
                    for (int i = 0; i < schedule.Columns.Count; i++)
                        tempRowArray[i] = schedule.Rows[currentSlotTableIndex][i].ToString();

                    schedule.Rows.RemoveAt(currentSlotTableIndex);
                }

                // Get new slot's index after removing duet record
                int newSlotTableIndex = GetTableIndexForSlot(schedule, newSlot);

                string currentJudgeId = schedule.Rows[currentSlotTableIndex]["Judge Id"].ToString();
                string newJudgeId = schedule.Rows[newSlotTableIndex]["Judge Id"].ToString();
                string newJudgeName = schedule.Rows[newSlotTableIndex]["Judge Name"].ToString();

                if (newJudgeId.Equals(""))
                {
                    showWarningMessage("Please move the audition to a slot with a judge.");
                    return;
                }

                // Shift audition slots
                if (currentSlot > newSlot)
                    MoveSlotEarlier(currentSlot, newSlot, currentSlotTableIndex, newSlotTableIndex, schedule, newJudgeId, newJudgeName);
                else if (currentSlot < newSlot)
                    MoveSlotLater(currentSlot, newSlot, currentSlotTableIndex, newSlotTableIndex, schedule, newJudgeId, newJudgeName);

                // If moved to different judge, change judge id and name and update list of unused judges
                if (currentJudgeId != newJudgeId && isDuet)
                {
                    tempRowArray[tempRowArray.Length - 2] = newJudgeName;
                    tempRowArray[tempRowArray.Length - 1] = newJudgeId;
                }

                // Update duet partner and add back to table
                if (isDuet)
                {
                    tempRowArray[0] = newSlot.ToString();
                    schedule.Rows.Add(tempRowArray);
                }

                // Resort and bind updated table
                DataView dataView = schedule.DefaultView;
                dataView.Sort = schedule.Columns[0].ColumnName + " ASC";
                schedule = dataView.ToTable();

                //schedule.DefaultView.Sort = schedule.Columns[0].ColumnName + " ASC";
                gvSchedule.DataSource = schedule;
                gvSchedule.DataBind();
                
                Session[scheduleData] = schedule;
                btnSaveOrder.Visible = true;
                btnAssignTimes.Visible = true;

                // Display a message if the judge is overbooked
                int judgeMinutesAvailable = GetJudgeAvailableMinutes(schedule, Convert.ToInt32(newJudgeId));
                if (judgeMinutesAvailable < 0)
                    showWarningMessage("Judge " + newJudgeName + " is overscheduled by " + Math.Abs(judgeMinutesAvailable) + " minutes.");

                showSuccessMessage("The audition has been successfully moved.");
            }
            else
            {
                showWarningMessage("Please indicate an audition to move and a slot to move it to.");
            }

            #region old
            // Make sure an audition id, judge, and time slot are selected
            //if (!txtSlot.Text.Equals("") && Int32.TryParse(txtSlot.Text, out currentSlot) && !txtNewSlot.Text.Equals("") && Int32.TryParse(txtNewSlot.Text, out newSlot))
            //{
                //int auditionOrgId = DbInterfaceStudentAudition.GetAuditionOrgIdByStudentAudition(currentSlot);
                //Audition audition = DbInterfaceAudition.LoadAuditionData(auditionOrgId); 

                //EventSchedule fullSchedule = DbInterfaceScheduling.LoadScheduleData(auditionOrgId);
                //fullSchedule.MoveAudition(currentSlot, Convert.ToInt32(ddlTimes.SelectedValue), audition);

                //// Update auditions in the schedule table
                //DataTable schedule = (DataTable)Session[scheduleData];
                //for (int i = 0; i < schedule.Rows.Count; i++)
                //{
                //    string audId = schedule.Rows[i]["Audition Id"].ToString();
                //    ScheduleSlot slot = fullSchedule.ScheduleSlots.Where(s => s.AuditionId.ToString().Equals(audId)).FirstOrDefault();

                //    // Update start time and judges in table
                //    if (slot != null)
                //    {
                //        schedule.Rows[i]["Start Time"] = slot.StartTime;
                //        schedule.Rows[i]["Judge Name"] = slot.JudgeName;
                //        schedule.Rows[i]["Judge Id"] = slot.JudgeId;
                //        schedule.Rows[i]["Grade"] = slot.Grade;
                //        schedule.Rows[i]["Type"] = slot.AuditionType;
                //        schedule.Rows[i]["Track"] = slot.AuditionTrack;
                //        schedule.Rows[i]["Time Preference"] = slot.TimePreference;
                //        schedule.Rows[i]["Student Id"] = slot.StudentId;
                //        schedule.Rows[i]["Audition Length"] = slot.Minutes;
                //        schedule.Rows[i]["Instrument"] = slot.Instrument;
                //    }
                //}

                //// Sort the table
                //DataView dataView = schedule.DefaultView;
                //dataView.Sort = "Judge Name ASC, Start Time ASC"; 
                //schedule = dataView.ToTable();

                //// After sorting based on time, convert to AM/PM format
                //for (int i = 0; i < schedule.Rows.Count; i++)
                //    schedule.Rows[i]["Start Time"] = FormatTime(TimeSpan.Parse(schedule.Rows[i]["Start Time"].ToString()));

                //gvSchedule.DataSource = schedule;
                //gvSchedule.DataBind();

                //Session[scheduleData] = schedule;
                //Session[eventSchedule] = fullSchedule;
            //}
            //else if (currentSlot == -1)
            //{
            //    showWarningMessage("Please select an audition to move.");
            //}
           // else if (ddlAuditionJudges.SelectedIndex <= 0 || ddlTimes.SelectedIndex <= 0)
            //else if (newSlot == -1)
            //{
            //    showWarningMessage("Please insert a time slot to move the selected audition to.");
            //}
            #endregion old
        }

        /*
         * Get the table's index for the input slot number.  
         * Return -1 if the slot number isn't found
         */
        private int GetTableIndexForSlot(DataTable schedule, int newSlot)
        {
            bool found = false;
            int idx = -1;

            for (int i = 0; !found && i < schedule.Rows.Count; i++)
            {
                if (schedule.Rows[i]["Slot"].ToString().Equals(newSlot.ToString()))
                {
                    idx = i;
                    found = true;
                }
            }

            return idx;
        }

        /*
         * Move a slot later 
         */
        private void MoveSlotLater(int currentSlot, int newSlot, int currentSlotTableIdx, int newSlotTableIdx, DataTable schedule, string newJudgeId, string newJudgeName)
        {
            schedule.Rows[currentSlotTableIdx]["Slot"] = newSlot;
            schedule.Rows[currentSlotTableIdx]["Judge Id"] = newJudgeId;
            schedule.Rows[currentSlotTableIdx]["Judge Name"] = newJudgeName;

            int slotNum = currentSlot;
            bool allowForDuet = true; // Track whether or not we need to decrease the slot number due to a duet
            for (int i = currentSlotTableIdx + 1; !allowForDuet || i <= newSlotTableIdx; i++)
            {
                schedule.Rows[i]["Slot"] = slotNum;

                if (schedule.Rows[i]["Type"].ToString().ToUpper().Equals("DUET") && allowForDuet)
                {
                    slotNum--;  // Need duet partner to have the same slot number
                    allowForDuet = false;
                }
                else if (schedule.Rows[i]["Type"].ToString().ToUpper().Equals("DUET"))
                    allowForDuet = true;

                slotNum++;
            }
        }

        /*
         * Move a slot earlier 
         */
        private void MoveSlotEarlier(int currentSlot, int newSlot, int currentSlotTableIdx, int newSlotTableIdx, DataTable schedule, string newJudgeId, string newJudgeName)
        {
            schedule.Rows[currentSlotTableIdx]["Slot"] = newSlot;
            schedule.Rows[currentSlotTableIdx]["Judge Id"] = newJudgeId;
            schedule.Rows[currentSlotTableIdx]["Judge Name"] = newJudgeName;

            int slotNum = newSlot + 1;
            bool allowForDuet = true; // Track whether or not we need to decrease the slot number due to a duet
            for (int i = newSlotTableIdx; !allowForDuet || i < currentSlotTableIdx; i++)
            {
                schedule.Rows[i]["Slot"] = slotNum;

                if (schedule.Rows[i]["Type"].ToString().ToUpper().Equals("DUET") && allowForDuet)
                {
                    slotNum--;  // Need duet partner to have the same slot number
                    allowForDuet = false;
                }
                else if (schedule.Rows[i]["Type"].ToString().ToUpper().Equals("DUET"))
                    allowForDuet = true;

                slotNum++;
            }
        }

        /*
         * Determine whether the entered slot exists
         */
        private bool SlotValid(string slotString)
        {
            int slot = -1;

            return !slotString.Equals("") && Int32.TryParse(slotString, out slot) && slot > 0 && slot <= gvSchedule.Rows.Count; // TODO - not sure what the upper range should be
        }

        private int GetMaxSlot(DataTable schedule)
        {
            return Convert.ToInt32(schedule.Rows[schedule.Rows.Count - 1]["Slot"]);
        }

        /*
         * Adds the selected audition slot to a new judge
         */
        protected void btnAddToJudge_Click(object sender, EventArgs e)
        {
            if (SlotValid(txtSlot.Text) && ddlOpenJudges.SelectedIndex > 0)
            {
                DataTable schedule = (DataTable)Session[scheduleData];

                int currentSlot = Int32.Parse(txtSlot.Text);
                int currentSlotTableIndex = GetTableIndexForSlot(schedule, currentSlot);
                bool isDuet = schedule.Rows[currentSlotTableIndex]["Type"].ToString().ToUpper().Equals("DUET");
                string[] tempRowArray = new string[schedule.Columns.Count]; // Temporary table to hold duet record, if needed
                int maxSlot = GetMaxSlot(schedule);

                // Temporarily remove a duet partner if we have a duet
                if (isDuet)
                {
                    for (int i = 0; i < schedule.Columns.Count; i++)
                        tempRowArray[i] = schedule.Rows[currentSlotTableIndex][i].ToString();

                    schedule.Rows.RemoveAt(currentSlotTableIndex);
                }

                // Shift all slots between the current slots # and the maximum down by 1 slot
                for (int i = currentSlotTableIndex + 1; i < schedule.Rows.Count; i++)
                    schedule.Rows[i]["Slot"] = Convert.ToInt32(schedule.Rows[i]["Slot"]) - 1;

                // Make the current slot the last one with the new judge
                schedule.Rows[currentSlotTableIndex]["Slot"] = maxSlot;
                schedule.Rows[currentSlotTableIndex]["Judge Name"] = ddlOpenJudges.SelectedItem.Text;
                schedule.Rows[currentSlotTableIndex]["Judge Id"] = ddlOpenJudges.SelectedValue;

                // Update duet partner and add back to table
                if (isDuet)
                {
                    tempRowArray[0] = maxSlot.ToString();
                    tempRowArray[9] = ddlOpenJudges.SelectedItem.Text;
                    tempRowArray[10] = ddlOpenJudges.SelectedValue;
                    schedule.Rows.Add(tempRowArray);
                }

                // Resort and bind updated table
                DataView dataView = schedule.DefaultView;
                dataView.Sort = schedule.Columns[0].ColumnName + " ASC";
                schedule = dataView.ToTable();

                gvSchedule.DataSource = schedule;
                gvSchedule.DataBind();

                Session[scheduleData] = schedule;
                btnSaveOrder.Visible = true;
                btnAssignTimes.Visible = true;

                showSuccessMessage("The audition has been successfully moved.");
            } 
            else
                showWarningMessage("Please indicate an audition to move and a judge to move it to.");
            
        }

        /*
         * Pre:  The AuditionId field must be empty or contain an integer
         * Post: Auditions the match the search criteria are displayed
         */
        protected void btnAuditionSearch_Click(object sender, EventArgs e)
        {
            int districtId = -1, year = -1;

            if (!ddlDistrictSearch.SelectedValue.ToString().Equals(""))
                districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);
            else //if the user did not select a district, but they are not a district admin, only search their district
            {
                User user = (User)Session[Utility.userRole];

                if (!user.permissionLevel.Contains('A'))
                    districtId = user.districtId;
            }

            if (!ddlYear.SelectedValue.ToString().Equals("")) year = Convert.ToInt32(ddlYear.SelectedValue);

            searchAuditions(gvAuditionSearch, districtId, year, auditionSearch);
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post: The input parameters are used to search for existing auditions.  Matchin audition
         *       information is displayed in the input gridview
         * @param gridview is the gridview in which the search results will be displayed
         * @param auditionType is the type of audition being searched for - district, badger keyboard, or badger Vocal/Instrumental
         * @param district is the district id of the audition being searched for
         * @param year is the year of the audition being searched for
         */
        private bool searchAuditions(GridView gridview, int districtId, int year, string session)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceAudition.GetAuditionSearchResults("", "District", districtId, year);

                //If there are results in the table, display them.  Otherwise clear current
                //results and return false
                if (table != null && table.Rows.Count > 0)
                {
                    gridview.DataSource = table;
                    gridview.DataBind();

                    //save the data for quick re-binding upon paging
                    Session[session] = table;
                }
                else
                {
                    showInfoMessage("No events were found matching the search criteria.");

                    clearGridView(gridview);
                    result = false;
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("ScheduleUpdate", "searchAuditions", "gridView: " + gridview + ", districtId: " +
                                 districtId + ", year: " + year + ", session: " + session, "Message: " + e.Message +
                                 "   StackTrace: " + e.StackTrace, -1);
            }

            return result;
        }

        protected void gvAuditionSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAuditionSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        protected void gvAuditionSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvAuditionSearch, e);
        }

        protected void gvAuditionSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectAudition();
        }

        private void selectAudition()
        {
            User user = (User)Session[Utility.userRole];

            if (!user.permissionLevel.Contains('A') && ddlDistrictSearch.SelectedIndex > 0)
            {
                upAuditionSearch.Visible = false;
                upViewSchedule.Visible = true;
                upInputs.Visible = true;
                //pnlButtons.Visible = true;

                int year = DateTime.Today.Year;
                if (DateTime.Today.Month >= 6 && !Utility.reportSuffix.Equals("Test"))
                    year = year + 1;

                ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(year.ToString()));

                int auditionOrgId = DbInterfaceAudition.GetAuditionOrgId(Convert.ToInt32(ddlDistrictSearch.SelectedValue), year);
                lblAudition.Text = ddlDistrictSearch.SelectedItem.Text + " " + ddlYear.Text + " Schedule";
                lblAudition2.Text = lblAudition.Text;
                lblAuditionId.Text = auditionOrgId.ToString();

                LoadScheduleInformation(auditionOrgId);
            }
            else if (user.permissionLevel.Contains('A'))
            {
                upAuditionSearch.Visible = false;
                upViewSchedule.Visible = true;
                upInputs.Visible = true;
                //pnlButtons.Visible = true;

                int index = gvAuditionSearch.SelectedIndex;

                if (index >= 0 && index < gvAuditionSearch.Rows.Count)
                {
                    ddlDistrictSearch.SelectedIndex =
                                ddlDistrictSearch.Items.IndexOf(ddlDistrictSearch.Items.FindByText(
                                gvAuditionSearch.Rows[index].Cells[2].Text));
                    ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(
                                            gvAuditionSearch.Rows[index].Cells[3].Text));

                    lblAudition.Text = ddlDistrictSearch.SelectedItem.Text + " " + ddlYear.Text + " Schedule";
                    lblAudition2.Text = lblAudition.Text;
                    lblAuditionId.Text = gvAuditionSearch.Rows[index].Cells[1].Text;

                    LoadScheduleInformation(Convert.ToInt32(lblAuditionId.Text));
                }
            }
        }

        /*
         * Pre:  audition must exist as the id of an audition in the system
         * Post: The existing data for the audition associated with the auditionId 
         *       is loaded to the page.
         * @param auditionId is the id of the audition being edited
         */
        private void LoadScheduleInformation(int auditionId)
        {
            //LoadAuditionJudges(DbInterfaceAudition.LoadAuditionData(auditionId));

            //load schedule table
            DataTable schedule = DbInterfaceScheduling.LoadScheduleForUpdate(auditionId);

            if (schedule != null && schedule.Rows.Count > 0)
            {
                // Load each judge's available time and juge's that haven't had auditions assigned to them
                LoadJudgeTimeAllowances(auditionId);
                LoadEmptyJudges(auditionId);

                gvSchedule.DataSource = schedule;
                gvSchedule.DataBind();

                Session[scheduleData] = schedule;
            }
            else
            {
                showErrorMessage("Error: The schedule could not be loaded.  Please make sure it has been created and saved.");
                Session[scheduleData] = null;
            }
        }

        /*
         * Load available judges that don't have auditions assigned to them
         */
        private void LoadEmptyJudges(int auditionId)
        {
            // Clear before updating
            ddlOpenJudges.Items.Clear();
            ddlOpenJudges.Items.Add(new ListItem("", ""));

            // Load judge's that have not yet been assigned to any auditions
            DataTable table = DbInterfaceJudge.LoadUnscheduledJudges(auditionId);
            ddlOpenJudges.DataSource = table;
            ddlOpenJudges.DataBind();
        }

        /*
         * Load the total time each judge for the audition has available
         */
        private void LoadJudgeTimeAllowances(int auditionId)
        {
            judgeTimeAllowances = DbInterfaceJudge.LoadAuditionJudgesTimeAllowances(auditionId);
            Session[judgeTimesSession] = judgeTimeAllowances;
        }

        /*
         * Get the number of minutes the judge still has available
         */
        private int GetJudgeAvailableMinutes(DataTable schedule, int judgeId)
        {
            int minutesAvailable = 0;

            if (judgeTimeAllowances != null)
            {
                minutesAvailable = judgeTimeAllowances[judgeId];

                for (int i = 0; i < schedule.Rows.Count; i++)
                {
                    if (schedule.Rows[i]["Judge Id"].ToString().Equals(judgeId.ToString()))
                        minutesAvailable -= Convert.ToInt32(schedule.Rows[i]["Audition Length"]);
                }
            }

            return minutesAvailable;
        }

        //private void LoadAuditionJudges(Audition audition)
        //{
        //    ddlAuditionJudges.Items.Clear();
        //    ddlAuditionJudges.Items.Add(new ListItem("", ""));

        //    try
        //    {
        //        List<Judge> judges = audition.GetEventJudges(true);

        //        //Load each judge to the dropdown
        //        foreach (Judge judge in judges)
        //        {
        //            ddlAuditionJudges.Items.Add(new ListItem(judge.id + ": " + judge.lastName + ", " + judge.firstName, judge.id.ToString()));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        showErrorMessage("Error: An error occurred while loading the event's judges.");
        //        Utility.LogError("ScheduleUpdate", "LoadAuditionJudges", "auditionId: " + audition.auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        //    }
        //}

        /*
         * Save the updates to the schedule order
         */
        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            DataTable schedule = (DataTable)Session[scheduleData];
            
            if (DbInterfaceScheduling.UpdateScheduleOrder(schedule, Convert.ToInt32(lblAuditionId.Text)))
                showSuccessMessage("The schedule order was successfully saved.");
            else
                showErrorMessage("The schedule order could not be saved.");

            LoadEmptyJudges(Convert.ToInt32(lblAuditionId.Text));
        }

        protected void btnAssignTimes_Click(object sender, EventArgs e)
        {
            DataTable schedule = (DataTable)Session[scheduleData];
            Dictionary<int, List<AuditionSlot>> judgeSlots = LoadJudgeSlots(schedule);

            if (judgeSlots != null)
            {
                int auditionOrgId = Convert.ToInt32(lblAuditionId.Text);
                bool judgeOverbooked = false;

                JudgeAuditionOrganizer scheduler = new JudgeAuditionOrganizer(judgeSlots, auditionOrgId);
                bool success = scheduler.SetTimes(judgeOverbooked);

                if (success && judgeOverbooked)
                {
                    showWarningMessage("Times were successfully assigned.  One or more judges are scheduled past the event's end time.");
                    ShowTimes(auditionOrgId);
                }
                else if (success)
                {
                    showSuccessMessage("Times were successfully assigned.");
                    ShowTimes(auditionOrgId);
                }
                else
                    showErrorMessage("Times could not be assigned to the audition slots.");
            }
        }

        /*
         * Show the times that have been assigned to the audition, but are not yet
         * submitted to the final version of the schedule
         */
        private void ShowTimes(int auditionOrgId)
        {
            DataTable scheduleTable = DbInterfaceAudition.LoadEventScheduleDataTable(auditionOrgId, false);

            //load data to page
            if (scheduleTable != null && scheduleTable.Rows.Count > 0)
            {
                gvSchedule.DataSource = scheduleTable;
                gvSchedule.DataBind();
                Session[scheduleData] = scheduleTable;

                // Switch buttons
                btnSaveOrder.Visible = false;
                btnAssignTimes.Visible = false;
                pnlInputs.Visible = false;
                btnContinue.Visible = true;
                btnSave.Visible = true;
                lblAudition2.Visible = true;
            }
            else
            {
                showErrorMessage("Error: The schedule information could not be loaded.  Please make sure that a schedule has been created and saved.");
                Session[scheduleData] = null;
            }
        }

        /*
         * Continue editing the order
         */
        protected void btnContinue_Click(object sender, EventArgs e)
        {
            LoadScheduleInformation(Convert.ToInt32(lblAuditionId.Text));
            btnSaveOrder.Visible = true;
            btnAssignTimes.Visible = true;
            pnlInputs.Visible = true;
            btnContinue.Visible = false;
            btnSave.Visible = false;
            lblAudition2.Visible = false;
        }

        /*
         * Pre:
         * Post: Commit the updated schedule and show a message saying it has been commited
         */
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (DbInterfaceScheduling.CommitSchedule(Convert.ToInt32(lblAuditionId.Text)))
                showSuccessMessage("The schedule was successfully committed");
            else
                showErrorMessage("The schedule could not be committed");
        }

        /*
         * Load the audition slots for each judge
         */
        private Dictionary<int, List<AuditionSlot>> LoadJudgeSlots(DataTable schedule)
        {
            Dictionary<int, List<AuditionSlot>> judgeSlots = new Dictionary<int, List<AuditionSlot>>();
            bool continueLoading = true;

            for (int i = 0; continueLoading && i < schedule.Rows.Count; i++)
            {
                int judgeId = -1;

                if (!Int32.TryParse(schedule.Rows[i]["Judge Id"].ToString(), out judgeId))
                {
                    showErrorMessage("Please assign all slots to a judge.");
                    continueLoading = false;
                }
                else
                {
                    int slot = Convert.ToInt32(schedule.Rows[i]["Slot"]);
                    int auditionId = Convert.ToInt32(schedule.Rows[i]["Audition Id"]);
                    int length = Convert.ToInt32(schedule.Rows[i]["Audition Length"]);
                    bool isDuet = schedule.Rows[i]["Type"].ToString().ToUpper().Equals("DUET");

                    if (judgeSlots.ContainsKey(judgeId))
                    {
                        judgeSlots[judgeId].Add(new AuditionSlot() 
                        { 
                            Slot = slot, 
                            AuditionId = auditionId, 
                            Length = length,
                            IsDuet = isDuet
                        });
                    }
                    else
                    {
                        judgeSlots.Add(judgeId, new List<AuditionSlot>() { 
                            new AuditionSlot()
                        {
                            Slot = slot,
                            AuditionId = auditionId,
                            Length = length,
                            IsDuet = isDuet
                        }});
                    }
                }
            }

            
            return judgeSlots;
        }

        protected void gvSchedule_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSchedule.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvSchedule, e);
        }

        /*
         * Pre:   The tables must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[auditionSearch];
                gvAuditionSearch.DataSource = data;
                gvAuditionSearch.DataBind();

                data = (DataTable)Session[scheduleData];
                gvSchedule.DataSource = data;
                gvSchedule.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("ScheduleUpdate", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        protected void btnClearAuditionSearch_Click(object sender, EventArgs e)
        {
            clearAuditionSearch();
        }

        /*
         * Pre:
         * Post: The Audition Search section is cleared
         */
        private void clearAuditionSearch()
        {
            ddlDistrictSearch.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            gvAuditionSearch.DataSource = null;
            gvAuditionSearch.DataBind();
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
         * Pre: The GridView gv must exist on the current form
         * Post:  The data binding of the GridView is cleared, causing the table to be cleared
         * @param gv is the GridView to be cleared
         */
        private void clearGridView(GridView gv)
        {
            gv.DataSource = null;
            gv.DataBind();
        }

        #region Messages

        /*
         * Pre:
         * Post: Displays the input error message in the top-left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showErrorMessage(string message)
        {
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
         * Post: Displays the input success message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showSuccessMessage(string message)
        {
            lblSuccessMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowSuccess", "showSuccessMessage()", true);
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
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Schedule", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            showErrorMessage("Error: An error occurred.");

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
        #endregion Messages
    }
}
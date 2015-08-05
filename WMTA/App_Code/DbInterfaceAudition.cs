using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/*
 * Author:  Krista Schultz
 * Date:    July 2013
 * This class is responsible for all of the database interactions dealing with 
 * District and Badger Auditions.
 */
public partial class DbInterfaceAudition
{
    /*
     * Pre:  
     * Post: Retrieves the audition date associated with the input year and district id.
     * @param auditionId is the unique id of the audition
     */
    public static string GetAuditionDate(int auditionId)
    {
        string auditionDate = "No Audition Created";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelectById";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                auditionDate = table.Rows[0]["Date"].ToString().Split(' ')[0];
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionDate", "auditionId: " + auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionDate;
    }

    /*
     * Pre:  
     * Post: Retrieves the audition date associated with the input year and district id.
     * @param districtId is the id of the district hosting the audition
     * @param year is the year of the audition
     */
    public static string GetAuditionDate(int districtId, int year)
    {
        string auditionDate = "No Audition Created";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@geoId", districtId);
            cmd.Parameters.AddWithValue("@year",year);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                auditionDate = table.Rows[0]["Date"].ToString().Split(' ')[0];
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionDate", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionDate;
    }

    /*
     * Pre:  
     * Post: Retrieves the audition freeze date associated with the input year and district id.
     * @param districtId is the id of the district hosting the audition
     * @param year is the year of the audition
     */
    public static string GetAuditionFreezeDate(int districtId, int year)
    {
        string freezeDate = "No Audition Created";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@geoId", districtId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                freezeDate = table.Rows[0]["FreezeDate"].ToString().Split(' ')[0];
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionFreezeDate", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return freezeDate;
    }

    /*
     * Pre:  
     * Post: Retrieves the audition freeze date associated with the input year and district id.
     * @param auditionOrgId is the unique id of the audition
     */
    public static string GetAuditionFreezeDateByAuditionOrgId(int auditionOrgId)
    {
        string freezeDate = "No Audition Created";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelectByAuditionOrgId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                freezeDate = table.Rows[0]["FreezeDate"].ToString().Split(' ')[0];
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionFreezeDateByAuditionOrgId", "auditionOrgId: " + auditionOrgId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return freezeDate;
    }

    /*
     * Pre:  
     * Post: Retrieves the audition id associated with the input year and district id.
     * @param districtId is the id of the district hosting the audition
     * @param year is the year of the audition
     */
    public static int GetAuditionOrgId(int districtId, int year)
    {
        int auditionOrgId = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@geoId", districtId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                auditionOrgId = Convert.ToInt32(table.Rows[0]["AuditionOrgId"].ToString().Split(' ')[0]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionOrgId", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionOrgId;
    }

    /*
     * Pre:  
     * Post: Retrieves the audition id and theory test series associated with the input year and district id.
     * @param districtId is the id of the district hosting the audition
     * @param year is the year of the audition
     */
    public static Tuple<int, string> GetAuditionOrgIdAndTestSeries(int districtId, int year)
    {
        Tuple<int, string> result = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@geoId", districtId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                int auditionOrgId = Convert.ToInt32(table.Rows[0]["AuditionOrgId"].ToString().Split(' ')[0]);
                string testSeries = table.Rows[0]["TheoryTestSeries"].ToString();

                result = new Tuple<int, string>(auditionOrgId, testSeries);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionOrgId", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return result;
    }

    /*
     * Pre:  All data in the audition object must be valid.
     *       The audition must not already exist in the system
     * Post: A new audition is added to the system 
     * @param audition is an Audition object holding the data to be associated
     *        with a new audition
     */
    public static void AddNewAudition(Audition audition)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@geoId", audition.districtId);
            cmd.Parameters.AddWithValue("@year", audition.auditionDate.Year);
            cmd.Parameters.AddWithValue("@numJudges", audition.numJudges);
            cmd.Parameters.AddWithValue("@venue", audition.venue);
            cmd.Parameters.AddWithValue("@chairpersonId", audition.chairpersonId);
            cmd.Parameters.AddWithValue("@theoryTest", audition.theoryTestSeries);
            cmd.Parameters.AddWithValue("@startTime1", audition.startTimeSession1);
            cmd.Parameters.AddWithValue("@endTime1", audition.endTimeSession1);
            cmd.Parameters.AddWithValue("@startTime1Display", audition.startTimeDisplaySession1);
            cmd.Parameters.AddWithValue("@endTime1Display", audition.endTimeDisplaySession1);
            cmd.Parameters.AddWithValue("@startTime2", audition.startTimeSession2);
            cmd.Parameters.AddWithValue("@endTime2", audition.endTimeSession2);
            cmd.Parameters.AddWithValue("@startTime2Display", audition.startTimeDisplaySession2);
            cmd.Parameters.AddWithValue("@endTime2Display", audition.endTimeDisplaySession2);
            cmd.Parameters.AddWithValue("@startTime3", audition.startTimeSession3);
            cmd.Parameters.AddWithValue("@endTime3", audition.endTimeSession3);
            cmd.Parameters.AddWithValue("@startTime3Display", audition.startTimeDisplaySession3);
            cmd.Parameters.AddWithValue("@endTime3Display", audition.endTimeDisplaySession3);
            cmd.Parameters.AddWithValue("@startTime4", audition.startTimeSession4);
            cmd.Parameters.AddWithValue("@endTime4", audition.endTimeSession4);
            cmd.Parameters.AddWithValue("@startTime4Display", audition.startTimeDisplaySession4);
            cmd.Parameters.AddWithValue("@endTime4Display", audition.endTimeDisplaySession4);
            cmd.Parameters.AddWithValue("@auditionDate", audition.auditionDate);
            cmd.Parameters.AddWithValue("@freezeDate", audition.freezeDate);
            cmd.Parameters.AddWithValue("@duetsAllowed", audition.duetsAllowed);
            
            adapter.Fill(table);

            if (table.Rows.Count == 1)
                audition.auditionId = Convert.ToInt32(table.Rows[0][0]);
            else
                audition.auditionId = -1;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "AddNewAudition", "Attributes of input audition - districtId: " + 
                             audition.districtId + ", numJudges: " + audition.numJudges + ", venue: " + audition.venue
                             + ", chairpersonId: " + audition.chairpersonId + ", theoryTest: " + audition.theoryTestSeries
                             + ", date: " + audition.auditionDate + ", freezeDate: " + audition.freezeDate + ", duetsAllowed: " +
                             audition.duetsAllowed, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

            audition.auditionId = -1;
        }
        finally
        {
            connection.Close();
        }
    }

    /*
     * Pre:  The id of the input audition must exist in the system
     * Post: The audition's information is edited in the database
     * @param audition is the object holding the updated audition information
     * @returns true if the update is successful and false otherwise
     */
    public static bool EditAudition(Audition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", audition.auditionId);
            cmd.Parameters.AddWithValue("@geoId", audition.districtId);
            cmd.Parameters.AddWithValue("@year", audition.auditionDate.Year);
            cmd.Parameters.AddWithValue("@numJudges", audition.numJudges);
            cmd.Parameters.AddWithValue("@venue", audition.venue);
            cmd.Parameters.AddWithValue("@chairpersonId", audition.chairpersonId);
            cmd.Parameters.AddWithValue("@theoryTest", audition.theoryTestSeries);
            cmd.Parameters.AddWithValue("@startTime1", audition.startTimeSession1);
            cmd.Parameters.AddWithValue("@endTime1", audition.endTimeSession1);
            cmd.Parameters.AddWithValue("@startTime1Display", audition.startTimeDisplaySession1);
            cmd.Parameters.AddWithValue("@endTime1Display", audition.endTimeDisplaySession1);
            cmd.Parameters.AddWithValue("@startTime2", audition.startTimeSession2);
            cmd.Parameters.AddWithValue("@endTime2", audition.endTimeSession2);
            cmd.Parameters.AddWithValue("@startTime2Display", audition.startTimeDisplaySession2);
            cmd.Parameters.AddWithValue("@endTime2Display", audition.endTimeDisplaySession2);
            cmd.Parameters.AddWithValue("@startTime3", audition.startTimeSession3);
            cmd.Parameters.AddWithValue("@endTime3", audition.endTimeSession3);
            cmd.Parameters.AddWithValue("@startTime3Display", audition.startTimeDisplaySession3);
            cmd.Parameters.AddWithValue("@endTime3Display", audition.endTimeDisplaySession3);
            cmd.Parameters.AddWithValue("@startTime4", audition.startTimeSession4);
            cmd.Parameters.AddWithValue("@endTime4", audition.endTimeSession4);
            cmd.Parameters.AddWithValue("@startTime4Display", audition.startTimeDisplaySession4);
            cmd.Parameters.AddWithValue("@endTime4Display", audition.endTimeDisplaySession4);
            cmd.Parameters.AddWithValue("@auditionDate", audition.auditionDate);
            cmd.Parameters.AddWithValue("@freezeDate", audition.freezeDate);
            cmd.Parameters.AddWithValue("@duetsAllowed", audition.duetsAllowed);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "EditAudition", "Attributes of input audition - auditionOrgId: " + 
                             audition.auditionId + ", district id: " + audition.districtId + 
                             ", numJudges: " + audition.numJudges + ", venue: " + audition.venue + ", chairpersonId: " 
                             + audition.chairpersonId + ", theoryTest: " + audition.theoryTestSeries
                             + ", date: " + audition.auditionId + ", freezeDate: " + audition.freezeDate + ", duetsAllowed: " +
                             audition.duetsAllowed, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The entered id must exist in the system
     * Post: The audition data associated with the id is returned in
     *       the form of an Audition object
     * @param id is the id of the audition whose information is being requested
     * @return the audition's information in the form of an Audition object
     */
    public static Audition LoadAuditionData(int id)
    {
        Audition audition = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSelectById";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@auditionId", id);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                int districtId = -1, numJudges = 0;
                if (!table.Rows[0]["GeoId"].ToString().Equals(""))
                    districtId = Convert.ToInt32(table.Rows[0]["GeoId"]);
                if (!table.Rows[0]["NumJudgesReq"].ToString().Equals(""))
                    numJudges = Convert.ToInt32(table.Rows[0]["NumJudgesReq"]);
                string venue = table.Rows[0]["Venue"].ToString();
                string chairperson = table.Rows[0]["ChairpersonId"].ToString();
                string theoryTest = table.Rows[0]["TheoryTestSeries"].ToString();
                
                DateTime auditionDate, freezeDate;
                bool duetsAllowed = true;

                DateTime.TryParse(table.Rows[0]["Date"].ToString(), out auditionDate);
                DateTime.TryParse(table.Rows[0]["FreezeDate"].ToString(), out freezeDate);

                if (!table.Rows[0]["DuetsAllowed"].ToString().Equals(""))
                    duetsAllowed = (bool)table.Rows[0]["DuetsAllowed"];

                audition = new Audition(id, districtId, numJudges, venue, chairperson, theoryTest, 
                                        auditionDate, freezeDate, duetsAllowed);
                GetAuditionSchedule(audition);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "LoadAuditionData", "id: " + id, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

            audition = null;
        }

        connection.Close();

        return audition;
    }

    /*
     * Pre:  The entered id must exist in the system
     * Post: The audition data associated with the id is returned in
     *       the form of an Audition object, including the schedule
     * @param id is the id of the audition whose information is being requested
     * @return the audition's information in the form of an Audition object
     */
    //public static Audition LoadAuditionDataWithSchedule(int id)
    //{
    //    Audition audition = LoadAuditionData(id);
    //    DataTable table = new DataTable();
    //    SqlConnection connection = new
    //        SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

    //    try
    //    {
    //        connection.Open();
    //        string storedProc = "sp_AuditionEventScheduleSelect";

    //        SqlCommand cmd = new SqlCommand(storedProc, connection);

    //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        cmd.Parameters.AddWithValue("@auditionId", id);

    //        adapter.Fill(table);

    //        EventSchedule schedule = new EventSchedule();
    //        for (int i = 0; i < table.Rows.Count; i++)
    //        {
    //            int auditionId = -1, minutes = -1, judgeId = -1;
    //            TimeSpan startTime = TimeSpan.MinValue;
    //            if (!table.Rows[i]["AuditionId"].ToString().Equals(""))
    //                auditionId = Convert.ToInt32(table.Rows[i]["AuditionId"]);
    //            if (!table.Rows[i]["Minutes"].ToString().Equals(""))
    //                minutes = Convert.ToInt32(table.Rows[i]["Minutes"]);
    //            if (!table.Rows[i]["JudgeId"].ToString().Equals(""))
    //                auditionId = Convert.ToInt32(table.Rows[i]["JudgeId"]);
    //            if (!table.Rows[i]["AuditionStartTime"].ToString().Equals(""))
    //                startTime = TimeSpan.Parse(table.Rows[i]["AuditionStartTime"].ToString());
    //            string judgeName = table.Rows[i]["JudgeName"].ToString();

    //            schedule.Add(auditionId, judgeId, judgeName, minutes, startTime);
    //        }

    //        audition.Schedule = schedule;  
    //    }
    //    catch (Exception e)
    //    {
    //        Utility.LogError("DbInterfaceAudition", "LoadAuditionDataWithSchedule", "id: " + id, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

    //        audition = null;
    //    }

    //    connection.Close();

    //    return audition;
    //}

    /*
     * Pre:
     * Post: Returns the schedule information for the event in a data table
     */
    public static DataTable LoadEventScheduleDataTable(int auditionOrgId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionEventScheduleSelectForDataTable";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionOrgId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "LoadEventScheduleDataTable", "auditionOrgId: " + auditionOrgId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: Sets the schedule of the input audition
     */
    public static void GetAuditionSchedule(Audition audition)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionScheduleSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);

            adapter.Fill(table);

            if (table.Rows.Count == 4)
            {
                TimeSpan startTime1 = TimeSpan.Parse(table.Rows[0]["TimePeriodStart"].ToString());
                TimeSpan endTime1 = TimeSpan.Parse(table.Rows[0]["TimePeriodEnd"].ToString());
                TimeSpan startTime2 = TimeSpan.Parse(table.Rows[1]["TimePeriodStart"].ToString());
                TimeSpan endTime2 = TimeSpan.Parse(table.Rows[1]["TimePeriodEnd"].ToString());
                TimeSpan startTime3 = TimeSpan.Parse(table.Rows[2]["TimePeriodStart"].ToString());
                TimeSpan endTime3 = TimeSpan.Parse(table.Rows[2]["TimePeriodEnd"].ToString());
                TimeSpan startTime4 = TimeSpan.Parse(table.Rows[3]["TimePeriodStart"].ToString());
                TimeSpan endTime4 = TimeSpan.Parse(table.Rows[3]["TimePeriodEnd"].ToString());

                audition.startTimeSession1 = startTime1;
                audition.startTimeSession2 = startTime2;
                audition.startTimeSession3 = startTime3;
                audition.startTimeSession4 = startTime4;
                audition.endTimeSession1 = endTime1;
                audition.endTimeSession2 = endTime2;
                audition.endTimeSession3 = endTime3;
                audition.endTimeSession4 = endTime4;
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionSchedule", "id: " + audition.auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();
    }

    /*
     * Get a list of the audition's time slots including start and end times
     */
    public static List<TimeSlot> GetAuditionTimeSlots(int auditionOrgId)
    {
        List<TimeSlot> slots = new List<TimeSlot>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionScheduleSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionOrgId);

            adapter.Fill(table);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                int scheduleId = Convert.ToInt32(table.Rows[i]["ScheduleId"]);
                TimeSpan startTime = TimeSpan.Parse(table.Rows[0]["TimePeriodStart"].ToString());
                TimeSpan endTime = TimeSpan.Parse(table.Rows[0]["TimePeriodEnd"].ToString());

                slots.Add(new TimeSlot() 
                {
                    Order = scheduleId, 
                    StartTime = startTime, 
                    EndTime = endTime 
                });
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionTimeSlots", "auditionOrgId: " + auditionOrgId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return slots.OrderBy(s => s.Order).ToList();
    }

    /*
     * Pre:
     * Post: Return a data table with information to be bound to a 
     *      checkbox list for judge time preferences
     */
    public static DataTable LoadJudgeTimePreferenceOptions(int auditionId) 
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownJudgeTimePref";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "LoadJudgeTimePreferenceOptions", "id: " + auditionId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: Determines whether there is already an audition in the database with the input information
     * @param districtId is the unique identifier of the district in which the audition is to take place
     * @param year is the year in which the audition is to take place
     * @returns true if the audition exists and false otherwise
     */
    public static bool AuditionExists(int districtId, int year)
    {
        bool exists = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionExists";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@geoId", districtId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            //check whether or not the audition exists
            if (table.Rows.Count > 0)
                exists = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "AuditionExists", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return exists;
    }

    /*
     * Pre:
     * Post: Determines whether there is already a state audition in the database with duets enabled
     * @param year is the year in which the audition is to take place
     * @param districtId is the district to ignore (used because you wouldn't want to return that another
     *        duet site already exists if it is the site being edited
     * @returns true if a different duet-enabled audition exists and false otherwise
     */
    public static bool StateDuetSiteExists(int year, int districtId)
    {
        bool exists = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionStateDuetSite";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@geoId", districtId);

            adapter.Fill(table);

            //check whether or not the audition exists
            if (table.Rows.Count > 0)
                exists = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "StateDuetSiteExists", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return exists;
    }

     /*
     * Pre:
     * Post: Determines whether the audition at the input district id allows duets
     * @param year is the year in which the audition is to take place
     * @param districtId is the district to check
     * @returns true if duets are enabled for the district's audition and false otherwise
     */
    public static bool StateSiteAllowsDuets(int auditionOrgId)
    {
        bool allowed = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionStateSiteAllowsDuets";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            //check whether or not duets are allowed
            if (table.Rows.Count > 0)
                allowed = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "StateSiteAllowsDuets", "auditionOrgId: " + auditionOrgId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return allowed;
    }

    /*
     * Pre:  id must be either the empty string or an integer
     * Post: If an id is entered, a data table containing the information for the associated
     *       audition is returned.  If an audition type, district, and/or year are included, the 
     *       existing auditions are filtered based on that information
     * @param id is the student id being searched for
     * @param auditionType is the type of the audition being searched for - District, State Keyboard, State Vocal/Instrumental
     * @param districtId is the district id of the audition being searched for
     * @param year is the year of the audition being searched for
     */
    public static DataTable GetAuditionSearchResults(string id, string auditionType, int districtId, int year)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionSearch";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!id.Equals(""))
                cmd.Parameters.AddWithValue("@auditionId", id);
            else
                cmd.Parameters.AddWithValue("@auditionId", null);

            cmd.Parameters.AddWithValue("@auditionType", auditionType);

            if (districtId != -1)
                cmd.Parameters.Add("@districtId", districtId);
            else
                cmd.Parameters.Add("@districtId", null);

            if (year != -1)
                cmd.Parameters.AddWithValue("@year", year);
            else
                cmd.Parameters.AddWithValue("@year", null);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetAuditionSearchResults", "id: " + id + ", auditionType: " + 
                             auditionType + "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);

            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input student must exist in the system
     * Post: Retrieves the student's District Auditions for the last district audition that occurred
     * @param student is the student whose auditions are being returned
     * @returns a data table containing the student's auditions
     */
    public static DataTable GetDistricts()
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownDistrictDistricts";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetDistricts", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input teacher must exist in the system
     * Post: Retrieves the districts that the input teachers had students participating in for the input year
     */
    public static DataTable GetTeacherDistrictsForYear(int teacherId, int year)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownTeacherStudentDistrictsForYear";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetTeacherDistrictsForYear", "teacherId: " + teacherId + ", year: " + year, 
                "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input audition org id must exist in the system.  The input teacher id must exist or be 0
     * Post: Retrieves the audition information for the input audition
     * @param auditionOrgId is the unique id of the audition
     * @param teacher is either 0 or the id of the teacher to filter on
     */
    public static DataTable GetBadgerDataDump(int auditionOrgId, int teacherId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_BadgerDataDump";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetBadgerDataDump", "auditionOrgId: " + auditionOrgId + ", teacherId: " + teacherId, 
                "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input audition org id must exist in the system.  The input teacher id must exist or be 0
     * Post: Retrieves the audition information for the input audition
     * @param auditionOrgId is the unique id of the audition
     * @param teacher is either 0 or the id of the teacher to filter on
     */
    public static DataTable GetDistrictDataDump(int auditionOrgId, int teacherId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DistrictDataDump";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetDistrictDataDump", "auditionOrgId: " + auditionOrgId + ", teacherId: " + teacherId,
                "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }
}
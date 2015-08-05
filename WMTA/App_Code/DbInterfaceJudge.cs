using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;


public partial class DbInterfaceJudge
{
    /*
     * Pre:  id must be either the empty string or an integer
     * Post: If an id is entered, a data table containing the information for the associated
     *       contact is returned.  If a partial first and/or last name are entered, a data table
     *       containing contacts with first and last names containing the input first and last
     *       names is returned.
     * @param id is the contact id being searched for
     * @param firstName is a full or partial first name that is being searched for
     * @param lastName is a full or partial last name that is being searched for
     * @param isOwnId indicates whether or not the input id is the current user's id
     */
    //public static DataTable GetJudgeAndSelfSearchResults(string id, string firstName, string lastName, bool isOwnId)
    //{
    //    DataTable table = new DataTable();
    //    SqlConnection connection = new
    //        SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

    //    try
    //    {
    //        connection.Open();
    //        string storedProc = "sp_ContactJudgeAndSelfSearch";

    //        SqlCommand cmd = new SqlCommand(storedProc, connection);

    //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        if (!id.Equals(""))
    //            cmd.Parameters.AddWithValue("@contactId", id);
    //        else
    //            cmd.Parameters.AddWithValue("@contactId", null);

    //        cmd.Parameters.AddWithValue("@firstName", firstName);
    //        cmd.Parameters.AddWithValue("@lastName", lastName);
    //        cmd.Parameters.AddWithValue("@isOwnId", isOwnId);

    //        adapter.Fill(table);
    //    }
    //    catch (Exception e)
    //    {
    //        Utility.LogError("DbInterfaceContact", "GetJudgeAndSelfSearchResults", "id: " + id + ", firstName: " + firstName +
    //                         ", lastName: " + lastName + ", isOwnId: " + isOwnId, "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
    //        table = null;
    //    }

    //    connection.Close();

    //    return table;
    //}

    /*
     * Pre:  id must be either the empty string or an integer
     * Post: If an id is entered, a data table containing the information for the associated
     *       judge(s) is returned.  If a partial first and/or last name are entered, a data table
     *       containing contacts with first and last names containing the input first and last
     *       names is returned.
     * @param id is the contact id being searched for
     * @param firstName is a full or partial first name that is being searched for
     * @param lastName is a full or partial last name that is being searched for
     */
    public static DataTable GetJudgeSearchResults(string id, string firstName, string lastName)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgeSearch";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!id.Equals(""))
                cmd.Parameters.AddWithValue("@contactId", id);
            else
                cmd.Parameters.AddWithValue("@contactId", null);

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetJudgeSearchResults", "id: " + id + ", firstName: " + firstName +
                             ", lastName: " + lastName, "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: Returns a list of preferences associated with the input contact id
     * @param contactId is the id of the judge whose preferences are being retrieved
     */
    public static List<JudgePreference> GetJudgePreferences(int contactId)
    {
        List<JudgePreference> preferences = new List<JudgePreference>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePreferenceSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);

            //add each preference to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int prefId = Convert.ToInt32(table.Rows[i]["PreferenceId"]);
                int prefTypeId = Convert.ToInt32(table.Rows[i]["PreferenceTypeId"]);
                string pref = table.Rows[i]["Preference"].ToString();

                preferences.Add(new JudgePreference(prefId, (Utility.JudgePreferences)prefTypeId, pref));
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetJudgePreferences", "contactId: " + contactId, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
            preferences = null;
        }

        connection.Close();

        return preferences;
    }

    /*
     * Pre:   The input contact id must exist in the database
     * Post:  The new preference is added to the database
     * @param contactId is the id of the contact that the new preference is to be associated with
     * @param prefType is the type of the preference
     * @param preferenceValue is the value of the preference
     */
    public static JudgePreference AddJudgePreference(int contactId, Utility.JudgePreferences prefType, string preferenceValue)
    {
        JudgePreference preference = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePreferenceNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@prefTypeId", (int)prefType);
            cmd.Parameters.AddWithValue("@preference", preferenceValue);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                int prefId = Convert.ToInt32(table.Rows[0]["PreferenceId"]);

                preference = new JudgePreference(prefId, prefType, preferenceValue);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "AddJudgePreference", "contactId: " + contactId + ", prefType: " +
                             prefType + ", preferenceValue: " + preferenceValue, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
        }

        connection.Close();


        return preference;
    }

    /*
     * Pre:   
     * Post:  The preference is deleted from the database
     * @param contactId is the id of the contact that the preference is associated with
     * @param prefType is the type of the preference
     * @param preferenceValue is the value of the preference
     * @returns true if the preference is successfully deleted and false otherwise
     */
    public static bool DeleteJudgePreference(int contactId, Utility.JudgePreferences prefType, string preferenceValue)
    {
        bool result = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePreferenceDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@prefTypeId", (int)prefType);
            cmd.Parameters.AddWithValue("@preference", preferenceValue);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "DeleteJudgePreference", "contactId: " + contactId + ", prefType: " +
                             prefType + ", preferenceValue: " + preferenceValue, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
            result = false;
        }

        connection.Close();

        return result;
    }

    /*
     * Load the judge's preferred audition levels for the input audition
     */
    public static List<string> LoadJudgeAuditionLevels(int contactId, int auditionOrgId)
    {
        List<string> auditionLevels = new List<string>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefAudLevelSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each preference to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                auditionLevels.Add(table.Rows[i]["AuditionTrackId"].ToString());
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadJudgeAuditionLevels", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId, 
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            auditionLevels = null;
        }

        connection.Close();

        return auditionLevels;
    }

    /*
     * Load the judge's preferred instruments for the input audition
     */
    public static List<string> LoadJudgeInstruments(int contactId, int auditionOrgId)
    {
        List<string> instruments = new List<string>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefInstrumentSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each preference to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                instruments.Add(table.Rows[i]["Instrument"].ToString());
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadJudgeInstruments", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            instruments = null;
        }

        connection.Close();

        return instruments;
    }

    /*
     * Load the judge's preferred levels for the input audition
     */
    public static List<string> LoadJudgeLevels(int contactId, int auditionOrgId)
    {
        List<string> levels = new List<string>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefCompLevelSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each preference to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                levels.Add(table.Rows[i]["CompLevelId"].ToString());
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadJudgeLevels", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            levels = null;
        }

        connection.Close();

        return levels;
    }

    /*
     * Load the judge's preferred audition type for the input audition
     */
    public static List<string> LoadJudgeAuditionTypes(int contactId, int auditionOrgId)
    {
        List<string> types = new List<string>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefAudTypeSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each preference to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                types.Add(table.Rows[i]["AuditionType"].ToString());
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadJudgeAuditionTypes", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            types = null;
        }

        connection.Close();

        return types;
    }

    /*
     * Load the judge's preferred audition time ids for the input audition
     */
    public static List<string> LoadJudgeTimes(int contactId, int auditionOrgId)
    {
        List<string> timeIds = new List<string>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefTimeSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each preference to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                timeIds.Add(table.Rows[i]["ScheduleId"].ToString());
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadJudgeTimes", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            timeIds = null;
        }

        connection.Close();

        return timeIds;
    }

    /*
     * Load and calculate the total time (in minutes) available for each judge.
     * Returns a map of judge id -> minutes available
     */
    public static Dictionary<int, int> LoadAuditionJudgesTimeAllowances(int auditionOrgId)
    {
        Dictionary<int, int> timeAllowances = new Dictionary<int, int>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionJudgesSessionsSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each judge to the dictionary
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int judgeId = Convert.ToInt32(table.Rows[i]["ContactId"].ToString());
                TimeSpan startTime = TimeSpan.Parse(table.Rows[i]["TimePeriodStart"].ToString());
                TimeSpan endTime = TimeSpan.Parse(table.Rows[i]["TimePeriodEnd"].ToString());
                int sessionMinutes = (int)((endTime - startTime).TotalMinutes);

                if (timeAllowances.ContainsKey(judgeId))
                    timeAllowances[judgeId] = timeAllowances[judgeId] + sessionMinutes;
                else
                    timeAllowances.Add(judgeId, sessionMinutes);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadAuditionJudgesTimeAllowances", "auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            timeAllowances = null;
        }

        connection.Close();

        return timeAllowances;
    }

    /*
     * Load the time periods each judge is available
     * Returns a map of schedule id to start time, end time
     */
    public static Dictionary<int, List<TimeSlot>> LoadAuditionJudgesTimeSlots(int auditionOrgId)
    {
        Dictionary<int, List<TimeSlot>> timePeriods = new Dictionary<int, List<TimeSlot>>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionJudgesSessionsSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            // Add each judge to the dictionary
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int judgeId = Convert.ToInt32(table.Rows[i]["ContactId"].ToString());
                int scheduleId = Convert.ToInt32(table.Rows[i]["ScheduleId"].ToString());
                TimeSpan startTime = TimeSpan.Parse(table.Rows[i]["TimePeriodStart"].ToString());
                TimeSpan endTime = TimeSpan.Parse(table.Rows[i]["TimePeriodEnd"].ToString());

                if (timePeriods.ContainsKey(judgeId))
                    timePeriods[judgeId].Add(new TimeSlot() { Order = scheduleId, StartTime = startTime, EndTime = endTime });
                else
                    timePeriods.Add(judgeId, new List<TimeSlot>() { new TimeSlot() { Order = scheduleId, StartTime = startTime, EndTime = endTime } });
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadAuditionJudgesTimeSlots", "auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            timePeriods = null;
        }

        connection.Close();

        return timePeriods;
    }

    /*
     * Adds a new audition level for the input judge in the input audition
     */
    public static bool AddJudgeAuditionLevel(int contactId, int auditionOrgId, string level)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefAudLevelNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@auditionTrack", level);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "AddJudgeAuditionLevel", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId + ", level: " + level,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Adds a new instrument for the input judge in the input audition
     */
    public static bool AddJudgeInstrument(int contactId, int auditionOrgId, string instrument)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefInstrumentNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@instrument", instrument);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "AddJudgeInstrument", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId + ", instrument: " + instrument,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Adds a new comp level for the input judge in the input audition
     */
    public static bool AddJudgeLevel(int contactId, int auditionOrgId, string level)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefCompLevelNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@auditionComp", level);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "AddJudgeLevel", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId + ", level: " + level,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Adds a new audition type for the input judge in the input audition
     */
    public static bool AddJudgeAuditionType(int contactId, int auditionOrgId, string type)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefAudTypeNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@auditionType", type);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "AddJudgeAuditionType", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId + ", type: " + type,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Adds a new time preference for the input judge in the input audition
     */
    public static bool AddJudgeTime(int contactId, int auditionOrgId, int scheduleId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePrefTimeNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@schedule", scheduleId);
            cmd.Parameters.AddWithValue("@room", "");

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "AddJudgeTime", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId + ", scheduleId: " + scheduleId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Delete the judge's audition preferences for the input audition
     */
    public static void DeleteJudgePreferences(int contactId, int auditionOrgId, bool deleteTimePreferences)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_JudgePreferencesDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@deleteTimePreferences", deleteTimePreferences);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "DeleteJudgePreferences", "contactId: " + contactId + ", auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
        }

        connection.Close();
    }

    /*
     * Load judges associated with the audition that have not yet been scheduled
     */
    public static DataTable LoadUnscheduledJudges(int auditionOrgId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionUnscheduledJudgesSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "LoadUnscheduledJudges", "auditionOrgId: " + auditionOrgId,
                "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);

            table = null;
        }

        connection.Close();

        return table;
    }
}
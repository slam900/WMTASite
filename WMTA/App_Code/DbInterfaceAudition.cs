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
            cmd.Parameters.AddWithValue("@scheduleId", 1);
            cmd.Parameters.AddWithValue("@startTime", audition.startTime);
            cmd.Parameters.AddWithValue("@endTime", audition.endTime);
            cmd.Parameters.AddWithValue("@auditionDate", audition.auditionDate);
            cmd.Parameters.AddWithValue("@freezeDate", audition.freezeDate);
            cmd.Parameters.AddWithValue("@duetsAllowed", audition.duetsAllowed);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                audition.auditionId = Convert.ToInt32(table.Rows[0]["New Audition Id"]);
            else
                audition.auditionId = -1;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "AddNewAudition", "Attributes of input audition - districtId: " + 
                             audition.districtId + ", numJudges: " + audition.numJudges + ", venue: " + audition.venue
                             + ", chairpersonId: " + audition.chairpersonId + ", theoryTest: " + audition.theoryTestSeries
                             + ", startTime: " + audition.startTime+ ", endTime: " + audition.endTime + ", date: " +
                             audition.auditionDate + ", freezeDate: " + audition.freezeDate + ", duetsAllowed: " +
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
            cmd.Parameters.AddWithValue("@startTime", audition.startTime);
            cmd.Parameters.AddWithValue("@endTime", audition.endTime);
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
                             + ", startTime: " + audition.startTime + ", endTime: " + audition.endTime+ ", date: " +
                             audition.auditionId + ", freezeDate: " + audition.freezeDate + ", duetsAllowed: " +
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
                TimeSpan startTime, endTime;
                bool duetsAllowed = true;

                DateTime.TryParse(table.Rows[0]["Date"].ToString(), out auditionDate);
                DateTime.TryParse(table.Rows[0]["FreezeDate"].ToString(), out freezeDate);
                TimeSpan.TryParse(table.Rows[0]["AudStartTime"].ToString(), out startTime);
                TimeSpan.TryParse(table.Rows[0]["AudEndTime"].ToString(), out endTime);

                if (!table.Rows[0]["DuetsAllowed"].ToString().Equals(""))
                    duetsAllowed = (bool)table.Rows[0]["DuetsAllowed"];

                audition = new Audition(id, districtId, numJudges, venue, chairperson, theoryTest, 
                                        auditionDate, freezeDate, startTime, endTime, duetsAllowed);
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
    public static bool StateSiteAllowsDuets(int year, int districtId)
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

            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@geoId", districtId);

            adapter.Fill(table);

            //check whether or not duets are allowed
            if (table.Rows.Count > 0)
                allowed = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "StateSiteAllowsDuets", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
     * @param auditionType is the type of the audition being searched for - District, State Keyboard, State Non-Keyboard
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
}
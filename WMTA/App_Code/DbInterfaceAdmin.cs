using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;

public partial class DbInterfaceAdmin
{
    /*
     * Pre:  The input level must exist
     * Post: A tuple is returned containing the minimum length as Item1 and the maximum length as Item2
     * @param compLevelId is the level to retrieve limits from
     * @returns the min and max lengths
     */
    public static Tuple<int, int> LoadLevelLengthLimits(string compLevelId)
    {
        Tuple<int, int> limits = new Tuple<int, int>(0, 0);
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionLengthLimitsLoad";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compLevelId", compLevelId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                int minimum = 0, maximum = 0;
                if (!table.Rows[0]["Min"].ToString().Equals(""))
                    minimum = Convert.ToInt32(table.Rows[0]["Min"]);
                if (!table.Rows[0]["Max"].ToString().Equals(""))
                    maximum = Convert.ToInt32(table.Rows[0]["Max"]);

                limits = new Tuple<int, int>(minimum, maximum);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "LoadLevelLengthLimits", "compLevelId: " + compLevelId, "Message: " +
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return limits;
    }

    /*
     * Pre:  The input level must exist
     * Post: The length limits for the input level are updated
     * @param compLevelId is the level to retrieve limits from
     * @param min is the minimum length
     * @param max is the maximum length
     * @returns whether or not the update was successful
     */
    public static bool UpdateLevelLengthLimits(string compLevelId, int min, int max)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionLengthLimitsUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compLevelId", compLevelId);
            cmd.Parameters.AddWithValue("@min", min);
            cmd.Parameters.AddWithValue("@max", max);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "LoadLevelLengthLimits", "compLevelId: " + compLevelId, "Message: " +
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The input fee type combo must exist
     * Post: The fee type is updated
     * @returns whether or not the update was successful
     */
    public static bool UpdateTrackFees(string type, string track, string region, decimal fee)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_FeesUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@track", track);
            cmd.Parameters.AddWithValue("@region", region);
            cmd.Parameters.AddWithValue("@fee", fee);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "UpdateTrackFees", "type: " + type + ", track: " + track + ", region: " + region + ", fee: " + fee, 
                "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }
}

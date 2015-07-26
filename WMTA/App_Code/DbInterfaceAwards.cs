using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public partial class DbInterfaceAwards
{
    /*
     * Pre:  The input district id must exist
     * Post: Retrieves the award information for the input district for the input year
     */
    public static DataTable GetAwardData(int year, int districtId, int teacherId, bool districtAudition)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AwardsView";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@districtId", districtId);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@isDistrictAudition", districtAudition);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceAudition", "GetDistrictDataDump", "year: " + year + ", districtId: " + districtId + 
                ", teacherId: " + teacherId +  ", isDistrictAudition: " + districtAudition, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

            table = null;
        }

        connection.Close();

        return table;
    }
}
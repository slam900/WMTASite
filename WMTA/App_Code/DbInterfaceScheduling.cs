using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;


public class DbInterfaceScheduling
{
    /*
     * Pre:
     * Post: The new room is associated with the audition
     * @param auditionOrgId is the unique id of the audition to assign the room to
     * @param room is the room identifier
     * @returns true if there were no errors
     */
    public static bool AddRoom(int auditionOrgId, string room)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionRoomNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@room", room);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceScheduling", "AddRoom", "auditionOrgId: " + auditionOrgId + ", room: " + room, 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  No judges may be scheduled in the room
     * Post: The room is deleted from the audition
     * @param auditionOrgId is the unique id of the audition to assign the room to
     * @param room is the room identifier
     * @returns true if there were no errors
     */
    public static bool DeleteRoom(int auditionOrgId, string room)
    {
        bool success = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionRoomDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
            cmd.Parameters.AddWithValue("@room", room);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                success = Convert.ToBoolean(table.Rows[0]["Result"]);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceScheduling", "DeleteRoom", "auditionOrgId: " + auditionOrgId + ", room: " + room,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: Retrieves the rooms for the input audition id
     * @param auditionId is the id of the audition
     * @returns a data table containing the audition rooms in the "Room" column
     */
    public static DataTable GetAuditionRooms(int auditionOrgId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionRoomSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceScheduling", "GetAuditionRooms", "auditionOrgId: " + auditionOrgId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }
}
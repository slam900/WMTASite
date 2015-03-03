using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class Utility
{
    public const string version = "2.6";

    //enumeration for types of judge preferences - these numbers should be equal to the
    //type ids in the JudgePrefType database table
    public enum JudgePreferences { AuditionType = 1, AuditionLevel = 2, CompositionLevel = 3, 
                                   Instrument = 4, Time = 5 };

    //enumeration for page actions
    public enum Action { Add = 1, Edit = 2, Delete = 3 };

    //session variable name for session variable containing data of current user
    public const string userRole = "UserRole";

   //report server variables
    public const string ssrsUsername = "wismusta_reportservr";
    public const string ssrsPassword = "33wi8mu8ta44";

    /***  Live Variables ***/
    public const string ssrsDomain = "localhost";
    public const string ssrsUrl = "http://localhost/ReportServer_SQL_Service";
    public const string reportSuffix = "";
    
    /*** Test Variables ***/
    //public const string ssrsDomain = "sunflower.arvixe.com";
    //public const string ssrsUrl = "http://sunflower.arvixe.com/ReportServer_SQL_Service";
    //public const string reportSuffix = "Test";

    //Ovation email and password
    public const string ovationEmail = "WMTAOvation@gmail.com";
    public const string ovationPassword = "wiMTA2013*";
    public const string compositionEmail = "wmtacompositionhotline@gmail.com";
    public const string compositionPassword = "WMTAcomp2014";

    /*
     * Pre:
     * Post: A new error entry is added to the error log table
     * @param page is the page or class in which the error occurred in
     * @param method is the name of the method the error occurred in
     * @param inputs is the list of inputs for the method
     * @param description is the description of the error
     * @param contactId is the id of the current user
     */
    public static void LogError(string page, string method, string inputs, string description, int contactId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new 
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ErrorNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@page", page);
            cmd.Parameters.AddWithValue("@method", method);
            cmd.Parameters.AddWithValue("@inputs", inputs);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);
        }
        catch
        {

        }
    }

    /*
     * Pre:  time should be in the form of either xx:xx:xx or x:xx:xx
     * Post: The input time is converted to military time
     */
    public static string ConvertToPm(string time)
    {
        int charsBeforeSemiColon = 0, hour;
        string hourStr;
        bool found = false;

        //find first semi-colon
        while (!found && charsBeforeSemiColon < time.Length)
        {
            if (time.ElementAt(charsBeforeSemiColon) == ':')
            {
                found = true;
                charsBeforeSemiColon--;
            }

            charsBeforeSemiColon++;
        }

        if (charsBeforeSemiColon == 1)
        {
            hour = Convert.ToInt16(time.ElementAt(0).ToString());
            hourStr = (hour + 12).ToString();
            time = hourStr + time.Substring(1);
        }
        else if (charsBeforeSemiColon == 2)
        {
            hour = Convert.ToInt16(time.Substring(0, 2));
            hourStr = (hour + 12).ToString();
            time = hourStr + time.Substring(2);
        }

        return time;
    }

    /*
         * Pre:
         * Post: Gets the id of the current teacher or -1 if the current user isn't a teacher
         */
    public static int GetTeacherId(User user)
    {
        int id = 0;

        if (user.permissionLevel.Contains('T') && !(user.permissionLevel.Contains('D') || user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
        {
            id = user.contactId;
        }

        return id;
    }

    /*
     * Pre:
     * Post: Gets the district id of the current user if they are a district admin or teacher
     */
    public static int GetDistrictId(User user)
    {
        int districtId = -1;

        if (!(user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
        {
            districtId = user.districtId;
        }

        return districtId;
    }
}
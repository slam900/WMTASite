using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;

/*
 * Author:  Krista Schultz
 * Date:    February 2013
 * This class is responsible for all of the database interactions regarding compositions.
 */
public class DbInterfaceComposition
{
    /*
     * Pre:
     * Post: The new composition is added to the system and the composition's id is returned
     * @param title is the title of the new composition
     * @param composer is the composer of the new composition
     * @param style is the style of the new composition
     * @param time is the playing time of the composition
     * @param level is the competition level of the composition
     * @returns the id of the new composition
     */
    public static int AddComposition(string title, string composer, string style, double time, string level)
    {
        int id = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compositionName", title.Trim());
            cmd.Parameters.AddWithValue("@composer", composer.Trim());
            cmd.Parameters.AddWithValue("@style", style);
            cmd.Parameters.AddWithValue("@playingTime", time);
            cmd.Parameters.AddWithValue("@compLevelId", level);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                id = Convert.ToInt32(table.Rows[0]["New Composition Id"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "AddComposition", "title: " + title + ", composer: " + composer +
                             ", style: " + style + ", time: " + time + ", level: " + level, "Message: " + e.Message +
                             "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return id;
    }

    /*
     * Pre: The input composition id must exist in the system
     * Post: The composition information is updated 
     * @param id is the id of the composition
     * @param title is the title of the composition
     * @param composer is the composer of the composition
     * @param style is the style of the composition
     * @param time is the playing time of the composition
     * @param level is the competition level of the composition
     * @returns the id of the new composition
     */
    public static bool EditComposition(int id, string title, string composer, string style, double time, string level)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compositionId", id);
            cmd.Parameters.AddWithValue("@compositionName", title.Trim());
            cmd.Parameters.AddWithValue("@composer", composer.Trim());
            cmd.Parameters.AddWithValue("@style", style);
            cmd.Parameters.AddWithValue("@playingTime", time);
            cmd.Parameters.AddWithValue("@compLevelId", level);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "EditComposition", "id: " + id + ", title: " + title + ", composer: " +
                             composer + ", style: " + style + ", time: " + time + ", level: " + level, "Message: " + e.Message +
                             "   Stack Trace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre: The input composition id must exist in the system
     * Post: The composition information is updated 
     * @param fromId is the id of the first composition in the range to mark as reviewed
     * @param toId is the id of the last composition in the range to mark as reviewed
     */
    public static bool MarkCompositionsReviewed(int fromId, int toId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionsMarkReviewed";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@fromCompositionId", fromId);
            cmd.Parameters.AddWithValue("@toCompositionId", toId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "MarkCompositionsReviewed", "fromId: " + fromId + ", toId: " + toId, 
                "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre: 
     * Post: The composition with the input id is deleted
     * @param id is the id of the composition to be deleted
     * @returns the id of the new composition
     */
    public static bool DeleteComposition(int id)
    {
        bool success = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compositionId", id);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                success = Convert.ToBoolean(table.Rows[0]["Result"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "DeleteComposition", "id: " + id, "Message: " + e.Message +
                             "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The input composition id must exist in the system
     * Post: The composition information associated with the input composition
     *       id is returned in the form of a Composition object
     * @param compositionId is the composition id associated with the composition
     *        information being requested
     * @returns the composition information associated with the input compositionId
     *          in the form of a Composition object
     */
    public static Composition GetComposition(int compositionId)
    {
        Composition composition = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compositionId", compositionId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                string title = table.Rows[0]["CompositionName"].ToString();
                string composer = table.Rows[0]["Composer"].ToString();
                string style = table.Rows[0]["Style"].ToString();
                double time = 0;
                if (!table.Rows[0]["PlayingTime"].ToString().Equals(""))
                    time = Convert.ToDouble(table.Rows[0]["PlayingTime"]);
                string level = table.Rows[0]["CompLevelId"].ToString();

                composition = new Composition(compositionId, title, composer, style, level, time);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "GetComposition", "compositionId: " + compositionId, "Message: " +
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return composition;
    }

    /*
     * Pre:
     * Post: Determines whether a composition exists with the input title and 
     *       composer name
     * @param title is the composition title to look for
     * @param composer is the composer name to look for
     * @returns true if there is a composition in the database with the input
     *          title and composer
     */
    public static bool CompositionExists(string title, string composer)
    {
        bool exists = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionTitleComposerMatch";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@composer", composer);

            adapter.Fill(table);

            //if a row is returned, the composition exists
            if (table.Rows.Count > 0)
                exists = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "CompositionExists", "title: " + title + ", composer: " + composer,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            exists = true;
        }

        connection.Close();
        
        return exists;
    }

    /*
     * Pre:  The entered style and competition level must exist in the system
     * Post: A data table containing the information for the associated composition
     *       is returned.  
     * @param style is the style of compositions to be included in the result
     * @param compLevelId is the competition level of compositions to be included in
     *        the result
     * @returns a Data Table consisting of compositions matching the search criteria
     */
    public static DataTable GetCompositionSearchResults(string style, string compLevelId, string composer)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownCompositionFiltered";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compStyle", style);
            cmd.Parameters.AddWithValue("@compLevel", compLevelId);
            cmd.Parameters.AddWithValue("@composer", composer);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "GetCompositionSearchResults", "style: " + style + ", compLevelId: " +
                             compLevelId + ", composer: " + composer, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  
     * Post: Replaces the composition with the id of idToReplace with the composition with
     *       the id replacementId.  The composition with the id of idToReplace is deleted
     *       from the system
     * @param idToReplace is the id of the composition to be replaced
     * @param replacementId is the id of the compositino to replace the other composition
     */
    public static bool ReplaceComposition(int idToReplace, int replacementId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionReplace";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@idToReplace", idToReplace);
            cmd.Parameters.AddWithValue("@replacementId", replacementId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "ReplaceComposition", "idToReplace: " + idToReplace + ", replacementId: " + replacementId, 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  
     * Post: Determines how many times a particular composition has been used in a student event 
     * @param id is the id of the composition
     */
    public static int GetCompositionUsageCount(int id)
    {
        int count = 0;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionUsed";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compositionId", id);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                count = Convert.ToInt32(table.Rows[0]["NumUses"]);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "GetCompositionUsageCount", "id: " + id, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return count;
    }

    /*
     * Pre:
     * Post: Retrieves data of all compositions containing the input string
     *       in their title.  Also retrieves the number of times the composition
     *       has been used in a student event
     * @param title is the title string to search for
     * @param composer is an optional parameter to search only the composer with the input name
     * @returns tuples of composition data with the number of times the composition
     *          has been used.  Returns null if there was an error
     */
    public static List<Tuple<Composition, int>> CompositionTitleQuery(string titleQuery, string composerName)
    {
        List<Tuple<Composition, int>> results = new List<Tuple<Composition, int>>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionTitleQuery";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@titleQuery", titleQuery);
            cmd.Parameters.AddWithValue("@composer", composerName);

            adapter.Fill(table);

            for (int i = 0; i < table.Rows.Count; i++) 
            {
                int id = Convert.ToInt32(table.Rows[i]["CompositionId"]);
                string title = table.Rows[i]["CompositionName"].ToString();
                string composer = table.Rows[i]["Composer"].ToString();
                string style = table.Rows[i]["Style"].ToString();
                double playingTime = Convert.ToDouble(table.Rows[i]["PlayingTime"]);
                string compLvlId = table.Rows[i]["CompLevelId"].ToString();
                int timesUsed = Convert.ToInt32(table.Rows[i]["NumTimesUsed"]);

                Composition comp = new Composition(id, title, composer, style, compLvlId, playingTime);

                results.Add(new Tuple<Composition,int>(comp, timesUsed));
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "CompositionTitleQuery", "titleQuery: " + titleQuery, 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            results = null;
        }

        connection.Close();

        return results;
    }

    /*
     * Pre: The input composition id must exist in the system
     * Post: The composition information is updated 
     * @param currentName is the existing composer name
     * @param newName is the name to replace the current name with
     */
    public static bool ReplaceComposer(string currentName, string newName)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ComposerUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@currentName", currentName);
            cmd.Parameters.AddWithValue("@newName", newName.Trim());

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "ReplaceComposer", "currentName: " + currentName + ", newName: " + newName, "Message: " + e.Message +
                             "   Stack Trace: " + e.StackTrace, -1);

            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The entered style and competition level must exist in the system
     * Post: A data table containing the information for the associated composer
     *       is returned.  
     * @param style is the style of compositions by the composers to be included in the result
     * @param compLevelId is the competition level of compositions by the composers to be included in
     *        the result
     * @returns a Data Table consisting of composers matching the search criteria
     */
    public static DataTable GetComposerSearchResults(string style, string compLevelId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownComposerFiltered";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@compStyle", style);
            cmd.Parameters.AddWithValue("@compLevel", compLevelId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceComposition", "GetComposerSearchResults", "style: " + style + ", compLevelId: " + compLevelId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            table =  null;
        }

        connection.Close();

        return table;
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;

/*
 * Author:  Krista Miller
 * Date:    July 2014
 * This class is responsible for all of the database interactions regarding feedback.
 */
public class DbInterfaceFeedback
{
    /*
     * Pre:
     * Post: The new feedback is added to the system and the feedbacks's id is returned
     * @param name is the name of the person giving feedback
     * @param email is the email address of the person giving feedback
     * @param feedbackType is the type of feedback being given
     * @param importance is the importance of the issue 
     * @param functionality is the affected functionality of the website
     * @param description is the description of the feedback
     * @returns the id of the new feedback
     */
    public static void AddFeedback(string name, string email, string feedbackType, Utility.Importance importance,
                                   string functionality, string description)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_FeedbackNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@feedbackType", feedbackType);
            cmd.Parameters.AddWithValue("@importance", (int)importance);
            cmd.Parameters.AddWithValue("@functionality", functionality);
            cmd.Parameters.AddWithValue("@description", description);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceFeedback", "AddFeedback", "name: " + name + ", email: " + email +
                             ", feedbackType: " + feedbackType + ", importance: " + importance + ", functionality: " + functionality +
                             ", description: " + description, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();
    }
}
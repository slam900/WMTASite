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
    public static bool AddFeedback(string name, string email, string feedbackType, string importance,
                                   string functionality, string description)
    {
        bool success = true;
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
            cmd.Parameters.AddWithValue("@importance", importance);
            cmd.Parameters.AddWithValue("@functionality", functionality);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@dateEntered", DateTime.Today.Date.ToShortDateString());

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceFeedback", "AddFeedback", "name: " + name + ", email: " + email +
                             ", feedbackType: " + feedbackType + ", importance: " + importance + ", functionality: " + functionality +
                             ", description: " + description, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

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
    public static bool UpdateFeedback(Feedback feedback)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_FeedbackUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", feedback.id);
            cmd.Parameters.AddWithValue("@feedbackType", feedback.feedbackType);
            cmd.Parameters.AddWithValue("@importance", feedback.importance);
            cmd.Parameters.AddWithValue("@functionality", feedback.functionality);
            cmd.Parameters.AddWithValue("@description", feedback.description);
            cmd.Parameters.AddWithValue("@assignedTo", feedback.assignedTo);
            cmd.Parameters.AddWithValue("@completed", feedback.completed);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceFeedback", "AddFeedback", "id: " + feedback.id + ", feedbackType: " + feedback.feedbackType +
                             ", importance: " + feedback.importance + ", functionality: " + feedback.functionality +
                             ", description: " + feedback.description, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  Feedback with the input id must exist in the system
     * Post: The feedback is loaded from the database
     * @param id is the unique id of the feedback to load
     * @returns the feedback information
     */
    public static Feedback LoadFeedback(int id)
    {
        Feedback feedback = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_FeedbackSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                string name = table.Rows[0]["Name"].ToString();
                string email = table.Rows[0]["Email"].ToString();
                string feedbackType = table.Rows[0]["FeedbackType"].ToString();
                string importance = table.Rows[0]["Importance"].ToString();
                string functionality = table.Rows[0]["Functionality"].ToString();
                string description = table.Rows[0]["Description"].ToString();
                string assignedTo = table.Rows[0]["AssignedTo"].ToString();
                
                bool complete = false;
                if (!table.Rows[0]["Completed"].ToString().Equals("")) complete = Convert.ToBoolean(table.Rows[0]["Completed"]);

                DateTime dateEntered = DateTime.MinValue, dateComplete = DateTime.MinValue;
                if (!table.Rows[0]["DateEntered"].ToString().Equals("")) dateEntered = Convert.ToDateTime(table.Rows[0]["DateEntered"]);
                if (!table.Rows[0]["DateComplete"].ToString().Equals("")) dateComplete = Convert.ToDateTime(table.Rows[0]["DateComplete"]);

                feedback = new Feedback(name, email, feedbackType, importance, functionality, description, assignedTo, complete, dateEntered, dateComplete);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceFeedback", "LoadFeedback", "id: " + id, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return feedback;
    }
}
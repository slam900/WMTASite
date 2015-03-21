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
 * This class is responsible for all of the database interactions related to contacts.
 */
public partial class DbInterfaceContact
{
    /*
     * Pre:  
     * Post:  Sets the contact type and district id for the input user
     * @param user holds the necessary login information of the current system user
     */
    public static void ContactLogin(User currUser)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactPermissionsSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@mtnaId", currUser.mtnaId);
            cmd.Parameters.AddWithValue("@firstInitial", currUser.firstInitial);
            cmd.Parameters.AddWithValue("@lastName", currUser.lastName);

            adapter.Fill(table);

            //if results are returned the contact exists
            if (table.Rows.Count > 0)
            {
                currUser.contactId = Convert.ToInt32(table.Rows[0]["ContactId"]);
                currUser.permissionLevel = table.Rows[0]["ContactType"].ToString();
                currUser.districtId = Convert.ToInt32(table.Rows[0]["GeoId"]);
            }
            else
            {
                currUser.permissionLevel = "";
                currUser.districtId = -1;
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "ContactLogin", "firstInitial: " + currUser.firstInitial + ", lastName: " + currUser.lastName +
                             ", MTNA Id: " + currUser.mtnaId + ", contactId: " + currUser.contactId, "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
        }

        connection.Close();
    }

    /*
     * Pre:  
     * Post:  Determines whether a contact matching the input information exists
     * @param firstName is the first name to look for
     * @param lastName is the last name to look for
     * @param email is the email address to look for
     * @returns true if a contact exists with the input first name, last name, and email 
     *          address and false otherwise
     */
    public static bool ContactExists(string firstName, string lastName, string email)
    {
        bool result = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactExists";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@email", email);

            adapter.Fill(table);

            //if results are returned the contact exists
            if (table.Rows.Count > 0)
                result = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "ContactExists", "firstName: " + firstName + ", lastName: " + lastName +
                             ", email: " + email, "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
            result = true;
        }

        connection.Close();

        return result;
    }

    /*
     * Pre:  The contact id in the input must exist in the system.
     * Post: The contact associated with the entered contact id is returned
     * @param contactId is the id of the contact 
     * @returns the contact information
     */
    public static Contact GetContact(int contactId)
    {
        Contact contact = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                string firstName = table.Rows[0]["FirstName"].ToString();
                string mi = table.Rows[0]["MI"].ToString();
                string lastName = table.Rows[0]["LastName"].ToString();
                int districtId = Convert.ToInt32(table.Rows[0]["DistrictId"]);
                string contactTypeId = table.Rows[0]["ContactType"].ToString();
                string street = table.Rows[0]["Address"].ToString();
                string city = table.Rows[0]["City"].ToString();
                string state = table.Rows[0]["State"].ToString();

                int zip = -1;
                if (!table.Rows[0]["Zip"].ToString().Equals("")) zip = Convert.ToInt32(table.Rows[0]["Zip"]);

                string phone = table.Rows[0]["Phone"].ToString();
                string email = table.Rows[0]["EmailAddress"].ToString();

                contact = new Contact(contactId, firstName, mi, lastName, email, phone, districtId, contactTypeId);
                contact.setAddress(street, city, state, zip);
            }
            else
                contact = null;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetContact", "contactId: " + contactId, "Message: " + e.Message + 
                             "   StackTrace: " + e.StackTrace, -1);
            contact = null;
        }

        connection.Close();

        return contact;
    }

    /*
     * Pre:  The contact id in the input Contact must exist in the system.
     * Post: The contact information is updated
     * @param contact contains the updated information of the contact
     * @returns the id of the new contact
     */
    public static int CreateContact(Contact contact)
    {
        int contactId = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@firstName", contact.firstName);
            cmd.Parameters.AddWithValue("@mi", contact.middleInitial);
            cmd.Parameters.AddWithValue("@lastName", contact.lastName);
            cmd.Parameters.AddWithValue("@geoId", contact.districtId);
            cmd.Parameters.AddWithValue("@address", contact.street);
            cmd.Parameters.AddWithValue("@city", contact.city);
            cmd.Parameters.AddWithValue("@state", contact.state);
            cmd.Parameters.AddWithValue("@zip", contact.zip);
            cmd.Parameters.AddWithValue("@phone", contact.phone);
            cmd.Parameters.AddWithValue("@emailAddress", contact.email);
            cmd.Parameters.AddWithValue("@contactType", contact.contactTypeId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                contactId = Convert.ToInt32(table.Rows[0]["ContactId"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "CreateContact", "firstName: " + contact.firstName + ", mi: " + 
                             contact.middleInitial + ", lastName: " + contact.lastName + ", geoId: " + contact.districtId +
                             ", address: " + contact.street + ", city: " + contact.city + ", state: " + contact.state +
                             ", zip: " + contact.zip + ", phone: " + contact.phone + ", emailAddress: " + contact.email +
                             ", contactType: " + contact.contactTypeId, "Message: " + e.Message + "   StackTrace: " + 
                             e.StackTrace, -1);
            contactId = -1;
        }

        connection.Close();

        return contactId;
    }

    /*
     * Pre:  The contact id in the input Contact must exist in the system.
     * Post: The contact information is updated
     * @param contact contains the updated information of the contact
     */
    public static Contact UpdateContact(Contact contact)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contact.id);
            cmd.Parameters.AddWithValue("@firstName", contact.firstName);
            cmd.Parameters.AddWithValue("@mi", contact.middleInitial);
            cmd.Parameters.AddWithValue("@lastName", contact.lastName);
            cmd.Parameters.AddWithValue("@geoId", contact.districtId);
            cmd.Parameters.AddWithValue("@address", contact.street);
            cmd.Parameters.AddWithValue("@city", contact.city);
            cmd.Parameters.AddWithValue("@state", contact.state);
            cmd.Parameters.AddWithValue("@zip", contact.zip);
            cmd.Parameters.AddWithValue("@phone", contact.phone);
            cmd.Parameters.AddWithValue("@emailAddress", contact.email);
            cmd.Parameters.AddWithValue("@contactType", contact.contactTypeId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "UpdateContact", "contactId: " + contact.id + ", firstName: " +
                             contact.firstName + ", mi: " + contact.middleInitial + ", lastName: " + contact.lastName +
                             ", geoId: " + contact.districtId + ", address: " + contact.street + ", city: " +
                             contact.city + ", state: " + contact.state + ", zip: " + contact.zip + ", phone: " +
                             contact.phone + ", emailAdress: " + contact.email + ", contactType: " + contact.contactTypeId,
                             "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
            contact = null;
        }

        connection.Close();

        return contact;
    }

    /*
     * Pre:  The contact id in the input Contact must exist in the system.
     * Post: The contact information is deleted
     * @param contactId is the id of the contact to be deleted
     */
    public static bool DeleteContact(int contactId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "DeleteContact", "contactId: " + contactId,
                             "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  id must be either the empty string or an integer
     * Post: If an id is entered, a data table containing the information for the associated
     *       contact is returned.  If a partial first and/or last name are entered, a data table
     *       containing contacts with first and last names containing the input first and last
     *       names is returned.
     * @param id is the contact id being searched for
     * @param firstName is a full or partial first name that is being searched for
     * @param lastName is a full or partial last name that is being searched for
     * @param districtId is the id of the district to search in
     */
    public static DataTable GetContactSearchResults(string id, string firstName, string lastName, int districtId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactSearch";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!id.Equals(""))
                cmd.Parameters.AddWithValue("@contactId", id);
            else
                cmd.Parameters.AddWithValue("@contactId", null);

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@districtId", districtId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetContactSearchResults", "id: " + id + ", firstName: " + firstName +
                             ", lastName: " + lastName + ", districtId: " + districtId, "Message: " + e.Message + 
                             "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: If the contact with the input id was ever registered, their most recent
     *       MTNA id will be returned.  Note: each contact should only ever have 1 MTNA id
     * @param contactId is the id of the contact whose MTNA id is being retrieved
     * @returns the input contacts MTNA id
     */
    public static string GetMtnaId(int contactId)
    {
        string mtnaId = "";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactMtnaSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                mtnaId = table.Rows[0]["MtnaId"].ToString();
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetMtnaId", "contactId: " + contactId, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return mtnaId;
    }

    /*
     * Pre:
     * Post: Registers the contact with the input contact id for the input year and
     *       sets their MTNA id to the input MTNA Id
     * @param contactId is the id of the contact being registered
     * @param year is the year that the contact is being registered for
     * @param mtnaId is the MTNA id of the contact for the input year (should be the same for all years)
     * @returns whether the contact was successfully registered
     */
    public static bool RegisterContact(int contactId, int year, string mtnaId)
    {
        bool result = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactMtnaUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@mtnaId", mtnaId);
            cmd.Parameters.AddWithValue("@notes", "");

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "RegisterContact", "contactId: " + contactId + ", year: " + year + ", mtnaId: " + mtnaId,
                             "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
            table = null;
            result = false;
        }

        connection.Close();

        return result;
    }

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
    public static DataTable GetJudgeAndSelfSearchResults(string id, string firstName, string lastName, bool isOwnId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactJudgeAndSelfSearch";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!id.Equals(""))
                cmd.Parameters.AddWithValue("@contactId", id);
            else
                cmd.Parameters.AddWithValue("@contactId", null);

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@isOwnId", isOwnId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetJudgeAndSelfSearchResults", "id: " + id + ", firstName: " + firstName +
                             ", lastName: " + lastName + ", isOwnId: " + isOwnId, "Message: " + e.Message + "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  id must be either the empty string or an integer
     * Post: If an id is entered, a data table containing the information for the associated
     *       contact is returned.  If a partial first and/or last name are entered, a data table
     *       containing teachers with first and last names containing the input first and last
     *       names is returned.
     * @param id is the contact id being searched for
     * @param firstName is a full or partial first name that is being searched for
     * @param lastName is a full or partial last name that is being searched for
     */
    public static DataTable TeacherSearch(string id, string firstName, string lastName)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_TeacherSearch";

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
            Utility.LogError("DbInterfaceContact", "TeacherSearch", "id: " + id + ", firstName: " + firstName +
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
     * Pre:  The contact id in the input must exist in the system.
     * Post: The contact's name associated with the entered contact id is returned
     * @param contactId is the id of the contact whose name is being requested
     * @returns the name of the contact
     */
    public static string GetContactName(int contactId)
    {
        string name = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                name = table.Rows[0]["Name"].ToString();
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetContactName", "contactId: " + contactId, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
        }

        connection.Close();

        return name;
    }

    /*
     * Pre: 
     * Post: A data table containing the dropdown information for the teachers of the entered
     *       district is returned.
     * @param districtId is the id of the district to search for teachers in
     */
    public static DataTable GetTeachersFromDistrict(int districtId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownTeacherFromDistrict";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@districtId", districtId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetTeachersFromDistrict", "districtId: " + districtId, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre: 
     * Post: A data table containing the dropdown information for the teachers of
     *      students participating in the event indicated by the inputs
     */
    public static DataTable GetTeachersForEvent(int districtId, int year)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownEventTeachers";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@districtId", districtId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetTeachersForEvent", "districtId: " + districtId + ", year: " + year, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre: 
     * Post: A data table containing the dropdown information for the teachers of
     *      students participating in the year's Badger events
     */
    public static DataTable GetTeachersForBadgerEvent(int year)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownBadgerTeachers";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceContact", "GetTeachersForBadgerEvent", "year: " + year, "Message: " + e.Message +
                             "   StackTrace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  
     * Post:  Determines whether or not the contact is associated with any students
     * @param contactId is the unique id of the contact
     * @returns true if the contact is associated with any students and false otherwise
     */
    public static bool ContactHasStudents(int contactId)
    {
        bool hasStudents = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_ContactGetStudents";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactId", contactId);

            adapter.Fill(table);

            //if results are returned the contact exists
            if (table.Rows.Count > 0)
            {
                hasStudents = true;
            }
        }
        catch (Exception e)
        {
            hasStudents = true;
            Utility.LogError("DbInterfaceContact", "ContactHasStudents", "contactId: " + contactId, "Message: " + 
                             e.Message + "   StackTrace: " + e.StackTrace, -1);
        }

        connection.Close();

        return hasStudents;
    }
}
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
 * This class is responsible for all of the database interactions related to student information.
 */
public partial class DbInterfaceStudent
{
    /*
     * Pre:  The student object must have a first name, last name, disctrict, 
     *       and current teacher.  
     *       The middle initial may not be more than two characters in length. 
     *       The district id must be an integer. 
     *       The two contact ids may not be more than five characters in length.  TODO: ensure this holds when adding new contacts
     *       If there is no previous teacher, the previous teacher id must be "0".
     * Post: The new student's information will be entered into the database
     *       and their unique id will be returned
     * @param newStudent is a Student object containing the information of the
     *        new student that is to be added to the database
     * @return the new student's id
     */
    public static int AddNewStudent(Student newStudent)
    {
        int studentId = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@firstName", newStudent.firstName);
            cmd.Parameters.AddWithValue("@mi", newStudent.middleInitial);
            cmd.Parameters.AddWithValue("@lastName", newStudent.lastName);
            cmd.Parameters.AddWithValue("@grade", newStudent.grade);
            cmd.Parameters.AddWithValue("@geoId", newStudent.districtId);
            cmd.Parameters.AddWithValue("@prevContactId", newStudent.prevTeacherId);
            cmd.Parameters.AddWithValue("@curContactId", newStudent.currTeacherId);
            cmd.Parameters.AddWithValue("@legacyPoints", newStudent.legacyPoints);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                studentId = Convert.ToInt32(table.Rows[0]["StudentId"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "AddNewStudent", "firstName: " + newStudent.firstName + ", mi: " +
                             newStudent.middleInitial + ", lastName: " + newStudent.lastName + ", grade: " +
                             newStudent.grade + ", geoId: " + newStudent.districtId + ", prevContactId: " +
                             newStudent.prevTeacherId + ", curContactId: " + newStudent.getCurrTeacher() + ", legacyPoints: " +
                             newStudent.legacyPoints, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return studentId;
    }

    /*
     * Pre:  The id of the input student must exist in the system
     * Post: The student's information is edited in the database
     * @param student is the object holding the updated student information
     * @returns true if the update is successful and false otherwise
     */
    public static bool EditStudent(Student student)
    {
        bool success = true;
        int year = DateTime.Today.Year;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        if (DateTime.Today.Month >= 6) year = year + 1;

        // Look at current year no matter what if on the test site
        if (Utility.reportSuffix.Equals("Test")) //delete this
            year = DateTime.Today.Year; 

        try
        {
            connection.Open();
            string storedProc = "sp_StudentUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);
            cmd.Parameters.AddWithValue("@firstName", student.firstName);
            cmd.Parameters.AddWithValue("@mi", student.middleInitial);
            cmd.Parameters.AddWithValue("@lastName", student.lastName);
            cmd.Parameters.AddWithValue("@geoId", student.districtId);
            cmd.Parameters.AddWithValue("@prevContactId", student.prevTeacherId);
            cmd.Parameters.AddWithValue("@curContactId", student.currTeacherId);
            cmd.Parameters.AddWithValue("@grade", student.grade);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@legacyPoints", student.legacyPoints);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "EditStudent", "studentId: " + student.id + ", firstName: " + 
                             student.firstName + ", mi: " + student.middleInitial + ", lastName: " + student.lastName +
                             ", geoId: " + student.districtId + ", prevContactId: " + student.prevTeacherId +
                             ", curContactId: " + student.currTeacherId + ", grade: " + student.grade + ", year: " +
                             year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The id of the input student must exist in the system
     * Post: The student's information is deleted in the database
     * @param studentId is the id of the student to delete
     * @returns true if the delete is successful and false otherwise
     */
    public static bool DeleteStudent(int studentId)
    {
        bool result = true;
        DataTable table = new DataTable();
        SqlConnection connection = new 
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try{
            connection.Open();
            string storedProc = "sp_StudentDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", studentId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "DeleteStudent", "studentId: " + studentId , "Message: " + 
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
            result = false;
        }

        connection.Close();

        return result;
    }

    /*
     * Pre:
     * Post: Determines whether there is an existing student with the input name
     * @param firstName is the first name
     * @param lastName is the last name
     * @returns the data of matching students
     */
    public static DataTable StudentExists(string firstName, string lastName)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentExists";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "StudentExists", "firstName: " + firstName + ", lastName: " +
                             lastName, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            return null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The entered id must exist in the system
     * Post: The student data associated with the id is returned in
     *       the form of a Student object
     * @param id is the id of the student whose information is being requested
     * @return the student's information in the form of a Student object
     */
    public static Student LoadStudentData(int id)
    {
        Student student = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", id);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                string firstName = table.Rows[0]["StudentFirstName"].ToString();
                string middleInitial = table.Rows[0]["StudentMI"].ToString();
                string lastName = table.Rows[0]["StudentLastName"].ToString();
                int districtId = -1, teacherId = 0, prevTeacherId = 0, legacyPoints = 0;
                if (!table.Rows[0]["DistrictId"].ToString().Equals(""))
                    districtId = Convert.ToInt32(table.Rows[0]["DistrictId"]);
                if (!table.Rows[0]["CurrentTeacherId"].ToString().Equals(""))
                    teacherId = Convert.ToInt32(table.Rows[0]["CurrentTeacherId"]);
                if (!table.Rows[0]["PreviousTeacherId"].ToString().Equals(""))
                    prevTeacherId = Convert.ToInt32(table.Rows[0]["PreviousTeacherId"]);
                if (!table.Rows[0]["LegacyPoints"].ToString().Equals(""))
                    legacyPoints = Convert.ToInt32(table.Rows[0]["LegacyPoints"]);

                student = new Student(id, firstName, middleInitial, lastName, districtId, teacherId, prevTeacherId);
                student.legacyPoints = legacyPoints;
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "LoadStudentData", "id: " + id, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return student;
    }

    /*
     * Pre:  The student id must exist in the system
     * Post: If the student's grade has been previously entered for the current
     *       year or previous years, it will be calculated and returned.  Otherwise
     *       the empty string will be returned.
     * @param id is the id of the student whose current grade is being requested
     * @returns the current grade of the student
     */
    public static string GetStudentGrade(int id)
    {
        string grade = "";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current year is June or later, get the grade for the next year
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            connection.Open();
            string storedProc = "sp_StudentAuditionGrade2";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", id);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            if (table.Rows.Count > 0 && !table.Rows[0]["NextGrade"].ToString().Equals("Grade Not Found"))
                grade = table.Rows[0]["NextGrade"].ToString();

            //check if the student's grade is a number.  If it is and it is greater
            //than 12, set grade to Audult
            int gradeInt = 0;
            bool gradeIsNum = int.TryParse(grade, out gradeInt);

            if (gradeIsNum && gradeInt > 12)
                grade = "Adult";
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetStudentGrade", "id: " + id, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return grade;
    }

    /*
     * Pre:  The student id must exist in the system
     * Post: If the student's theory level has been previously entered for the current
     *       year, it will retrieved.  Otherwise the empty string will be returned.
     * @param studentId is the id of the student whose theory level is being requested
     * @returns the current theory level of the student
     */
    public static string GetTheoryLevel(int studentId)
    {
        string theoryLevel = "";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current year is June or later, get the grade for the next year
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            connection.Open();
            string storedProc = "sp_StudentTheoryLevelSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", studentId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                theoryLevel = table.Rows[0]["TheoryLevel"].ToString();
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetTheoryLevel", "studentId: " + studentId, "Message: " + 
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return theoryLevel;
    }

    /*
     * Pre:  The district id in the input must exist in the system
     * Post: The district associated with the entered district id is returned
     * @param districtId is the id of the district
     * @returns the name of the district
     */
    public static string GetStudentDistrict(int districtId)
    {
        string district = "";
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DistrictSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DistrictId", districtId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                district = table.Rows[0]["GeoName"].ToString();
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetStudentDistrict", "districtId: " + districtId, "Message: " + 
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return district;
    }

    /*
     * Pre:  The student id must exist in the system
     * Post: Retrieves the total points awarded to the student with the input id
     * @param studentId is the id of the student whose point total is being requested
     * @returns the current point total of the student
     */
    public static int GetTotalPoints(int studentId)
    {
        int pointTotal = 0;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentPointsTotal";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", studentId);

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                Int32.TryParse(table.Rows[0]["TotalPoints"].ToString(), out pointTotal);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetPointTotal", "studentId: " + studentId, "Message: " +
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return pointTotal;
    }

    /*
     * Pre:  id must be either the empty string or an integer
     * Post: If an id is entered, a data table containing the information for the associated
     *       student is returned.  If a partial first and/or last name are entered, a data table
     *       containing students with first and last names containing the input first and last
     *       names is returned.
     * @param id is the student id being searched for
     * @param firstName is a full or partial first name that is being searched for
     * @param lastName is a full or partial last name that is being searched for
     * @param districtId si the id of the district to search in
     */
    public static DataTable GetStudentSearchResults(string id, string firstName, string lastName, int districtId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentSearch";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!id.Equals(""))
                cmd.Parameters.AddWithValue("@studentId", id);
            else
                cmd.Parameters.AddWithValue("@studentId", null);

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@districtId", districtId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetStudentSearchResults", "id: " + id + ", firstName: " + firstName +
                             ", lastName: " + lastName + ", districtId: " + districtId, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
    * Pre:  id must be either the empty string or an integer
    * Post: If an id is entered, a data table containing the information for the associated
    *       student is returned.  If a partial first and/or last name are entered, a data table
    *       containing students with first and last names containing the input first and last
    *       names is returned.
    * @param id is the student id being searched for
    * @param firstName is a full or partial first name that is being searched for
    * @param lastName is a full or partial last name that is being searched for
    * @param teacherId is the contact id of the teacher whose students are being searched
    */
    public static DataTable GetStudentSearchResultsForTeacher(string id, string firstName, string lastName, int teacherId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentSearchForTeacher";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!id.Equals(""))
                cmd.Parameters.AddWithValue("@studentId", id);
            else
                cmd.Parameters.AddWithValue("@studentId", null);

            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetStudentSearchResults", "id: " + id + ", firstName: " + firstName +
                             ", lastName: " + lastName + ", teacherId: " + teacherId, "Message: " + e.Message +
                             "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The student must have an entry in the DataStudentYearHistory table
     * Post: The year id for the student is retrieved for the current year
     * @param studentId is the id of the student whose year id is needed
     * @returns the year id for the student
     */
    public static int GetStudentYearId(int studentId)
    {
        int yearId = -1, year = DateTime.Now.Year;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        //set year to next year if it is June or later
        if (DateTime.Now.Month >= 6) year = year + 1;

        // Look at current year no matter what if on the test site
        if (Utility.reportSuffix.Equals("Test")) //delete this
            year = DateTime.Today.Year; 

        try
        {
            connection.Open();
            string storedProc = "sp_StudentYearSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", studentId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            if (table.Rows.Count > 0) yearId = Convert.ToInt32(table.Rows[0]["YearId"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "GetStudentYear", "studentId: " + studentId, "Message: " + 
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return yearId;
    }

    /*
     * Pre:  The two teachers must exist
     * Post: The students assigned to from From teacher are moved to the To teacher
     * @param fromContactId is the id of the contact to move students from
     * @param toContactId is the id of the contact to move students to
     * @returns true if successful and false otherwise
     */
    public static bool TransferStudents(int fromContactId, int toContactId)
    {
        bool result = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentTransfer";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@contactIdFrom", fromContactId);
            cmd.Parameters.AddWithValue("@contactIdTo", toContactId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudent", "TransferStudents", "fromContactId: " + fromContactId + " , toContactID: " + toContactId, 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            result = false;
        }

        connection.Close();

        return result;
    }
}
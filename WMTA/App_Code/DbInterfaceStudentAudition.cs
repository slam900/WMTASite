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
 * This class is responsible for all of the database interactions dealing with 
 * student District and Badger Auditions.
 */
public partial class DbInterfaceStudentAudition
{
    /*
     * Pre:  The instrument and grade in the input parameters must exist in
     *       the system.
     *       The instrument must be "Instrument", "Voice", "Piano", or "Organ"
     * Post: The theory test level corresponding to the entered instrument
     *       and grade is returned
     * @param instrument is the instrument the student is playing
     * @param grade is the current grade of the student
     * @returns a table containing the valid theory test levels
     */
    public static DataTable GetTheoryTestLevel(string instrument, string grade, string auditionTrack)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_TheoryTestSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            //only options in the Instrument field of ConfigTheoryTestLevel 
            //are Piano, Organ, Voice, and Instrument
            if (!instrument.Equals("Piano") && !instrument.Equals("Organ") && !instrument.Equals("Voice"))
                instrument = "Instrument";

            cmd.Parameters.AddWithValue("@instrument", instrument);
            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.AddWithValue("@auditionTrack", auditionTrack);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetTheoryTestLevel", "instrument: " + instrument + ", grade: " +
                             grade + ", auditionTrack: " + auditionTrack, "Message: " + e.Message + "   Stack Trace: " +
                             e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: Determines whether or not the student is allowed to take the chosen theory level.
     *       If the student has gotten a 5 on the input theory level or any higher theory level,
     *       they must go up at least one level
     * @param studentId is the id of the student
     * @param theoryLevel is the theory level the student wishes to take
     * @returns the number of times the student has not gotten a 5 on the input theory level or
     *          any higher level
     */
    public static int CountTheoryLevel5s(int studentId, string theoryLevel)
    {
        int result = 0;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentTheoryTest5sCount";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", studentId);
            cmd.Parameters.AddWithValue("@theoryLevel", theoryLevel);

            adapter.Fill(table);

            for (int i = 0; i < table.Rows.Count; i++)
                result = result + Convert.ToInt32(table.Rows[i]["Count"]);
        }
        catch (Exception e)
        {
           Utility.LogError("DbInterfaceStudentAudition", "CountTheoryLevel5s", "studentId: " + studentId + ", theoryLevel: " +
                            theoryLevel, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return result;
    }

    /*
     * Pre:  Grade and audition type must exist in the system or be the empty string
     *       The instrument must be "Instrument", "Voice", "Piano", or "Organ"
     * Post: The theory test level corresponding to the entered instrument
     *       and grade is returned
     * @param instrument is the instrument the student is playing
     * @param grade is the current grade of the student
     * @returns the student's theory test level
     */
    public static DataTable GetValidAuditionTracks(string grade, string auditionType)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownTrackFiltered";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            //Kindergarteners have the same requirements as 1st graders and
            //adults the same as 12th graders
            if (grade.Equals("K"))
                grade = "1";
            else if (grade.Equals("A") || grade.Equals("Adult"))
                grade = "12";

            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.Add("@type", auditionType);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetValidAuditionTracks", "grade: " + grade + ", auditionType: " +
                             auditionType, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            table = null;
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input AudTrackRequirements object must contain a grade between
     *       1 and 12 and an audition track that exists in the system.
     * Post: The composition requirements are set for the input object
     * @param req is an object representing the composition requirements for
     *        a student's audition based on their grade and audition track
     * @returns true if the composition requirements are successfully retrieved and
     *          false otherwise
     */
    public static void GetAuditionTrackRequirements(AudTrackRequirements req)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_CompositionRequirementsSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@grade", req.grade);
            cmd.Parameters.Add("@track", req.track);

            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                req.requiredNumStyles = Convert.ToInt32(table.Rows[0]["NumDiffStyles"]);

                List<string> styles = null;
                string reqStyles = table.Rows[0]["StyleRequirement"].ToString();

                if (!reqStyles.Equals(""))
                    styles = reqStyles.Split('/').ToList();

                req.requiredStyles = styles;
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetAuditionTrackRequirements", "grade: " + req.grade +
                             ", track: " + req.track, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();
    }

    /*
     * Pre:
     * Post: Determines whether there is already an audition in the database with the input information
     * @param auditionId is an audition id that should be excluded from the search for duplicates
     * @param auditionOrgId is the id of the specific audition
     * @param yearId is the year id of the student
     * @param track is the audition track
     * @param instrument is the instrument used in the audidiont
     * @param type is the audition type
     */
    public static bool AuditionExists(int auditionId, int auditionOrgId, int yearId, string track, string instrument, string type)
    {
        bool exists = false;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
                connection.Open();
                string storedProc = "sp_StudentAuditionExists";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@auditionId", auditionId);
                cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
                cmd.Parameters.AddWithValue("@yearId", yearId);
                cmd.Parameters.AddWithValue("@auditionTrack", track);
                cmd.Parameters.AddWithValue("@instrument", instrument);
                cmd.Parameters.AddWithValue("@auditionType", type);

                adapter.Fill(table);

                //set the ids for the audition
                if (table.Rows.Count > 0)
                    exists = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "AuditionExists", "auditionOrgId: " + auditionOrgId + ", yearId: " +
                             yearId + ", track: " + track + ", instrument: " + instrument + ", type: " + type,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            exists = true;
        }

        connection.Close();

        return exists;
    }

    

    /*
     * Pre:
     * Post: A new audition is created for a student.  This includes adding the audition
     *      compositions and coordinating students, if needed
     * @param audition holds all information needed for the audition
     * @returns true if the audition was successfully created and false otherwise
     */
    public static bool CreateStudentDistrictAudition(DistrictAudition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, set up the audition for the next year
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            int auditionOrgId = GetAuditionOrgId(audition.districtId, year);

            if (auditionOrgId != -1)
            {

                connection.Open();
                string storedProc = "sp_StudentAuditionNew";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@studentId", audition.student.id);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@grade", audition.student.grade);
                cmd.Parameters.AddWithValue("@theoryLevel", audition.theoryLevel);
                cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
                cmd.Parameters.AddWithValue("@instrument", audition.instrument);
                cmd.Parameters.AddWithValue("@accompanist", audition.hasAccompanist());
                cmd.Parameters.AddWithValue("@accompanistName", audition.accompanist);
                cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
                cmd.Parameters.AddWithValue("@auditionTrack", audition.auditionTrack);
                cmd.Parameters.AddWithValue("@coordRideType", audition.hasCoordinates());
                cmd.Parameters.AddWithValue("@driveTime", 0);

                if (audition.am)
                    cmd.Parameters.AddWithValue("@request", "AM");
                else if (audition.pm)
                    cmd.Parameters.AddWithValue("@request", "PM");
                else if (audition.earliest)
                    cmd.Parameters.AddWithValue("@request", "E");
                else if (audition.latest)
                    cmd.Parameters.AddWithValue("@request", "L");
                else
                    cmd.Parameters.AddWithValue("@request", "");

                adapter.Fill(table);

                //set the ids for the audition
                if (table.Rows.Count == 1)
                {
                    int audId = Convert.ToInt32(table.Rows[0]["AuditionId"]);
                    int yearId = Convert.ToInt32(table.Rows[0]["YearId"]);

                    audition.auditionId = audId;
                    audition.yearId = yearId;

                    foreach (AuditionCompositions comp in audition.compositions)
                        success = success && CreateAuditionCompositions(audId, comp);

                    //if the audition is a duet, create the duet partner's audition
                    if (success && audition.auditionType.ToUpper().Equals("DUET"))
                    {
                        Student student = null;
                        int i = 0;
                        bool found = false;

                        //search for the correct coordinate type
                        while (i < audition.coordinates.Count && !found)
                        {
                            if (audition.coordinates[i].reason.ToUpper().Equals("DUET"))
                            {
                                student = audition.coordinates[i].student;
                                found = true;
                            }

                            i++;
                        }

                        success = CreateDuetPartnerDistrictAudition(audition, student);
                    }
                }
                else
                    success = false;
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateStudentDistrictAudition", "studentId: " +
                             audition.student.id + ", grade: " + audition.student.grade + 
                             ", theoryLevel: " + audition.theoryLevel + ", instrument: " + audition.instrument +
                             ", accompanist: " + audition.hasAccompanist() + ", accompanistName: " + audition.accompanist +
                             ", auditionType: " + audition.auditionType + ", auditionTrack: " + audition.auditionTrack +
                             ", coordRideType: " + audition.hasCoordinates(), "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;

        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: An audition is updated for a student.  This includes updating the audition
     *      compositions and coordinating students, if needed
     * @param audition holds all information needed for the audition
     * @returns true if the audition was successfully updated and false otherwise
     */
    public static bool UpdateStudentDistrictAudition(DistrictAudition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, set up the audition for the next year
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            int auditionOrgId = GetAuditionOrgId(audition.districtId, year);

            if (auditionOrgId != -1)
            {

                connection.Open();
                string storedProc = "sp_StudentAuditionUpdate";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);
                cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
                cmd.Parameters.AddWithValue("@grade", audition.student.grade);
                cmd.Parameters.AddWithValue("@theoryLevel", audition.theoryLevel);
                cmd.Parameters.AddWithValue("@instrument", audition.instrument);
                cmd.Parameters.AddWithValue("@accompanist", audition.hasAccompanist());
                cmd.Parameters.AddWithValue("@accompanistName", audition.accompanist);
                cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
                cmd.Parameters.AddWithValue("@auditionTrack", audition.auditionTrack);
                cmd.Parameters.AddWithValue("@coordRideType", audition.hasCoordinates());
                cmd.Parameters.AddWithValue("@driveTime", 0);

                if (audition.am)
                    cmd.Parameters.AddWithValue("@request", "AM");
                else if (audition.pm)
                    cmd.Parameters.AddWithValue("@request", "PM");
                else if (audition.earliest)
                    cmd.Parameters.AddWithValue("@request", "E");
                else if (audition.latest)
                    cmd.Parameters.AddWithValue("@request", "L");
                else 
                    cmd.Parameters.AddWithValue("@request", "");

                adapter.Fill(table);

                //set the ids for the audition
                if (table.Rows.Count == 1 && table.Rows[0]["UpdateMessage"].ToString().Equals("Update Complete"))
                {
                    //delete existing compositions
                    DbInterfaceStudentAudition.DeleteAuditionCompositions(audition.auditionId);

                    //add compositions
                    foreach (AuditionCompositions comp in audition.compositions)
                        success = success && CreateAuditionCompositions(audition.auditionId, comp);

                    //if the audition is a duet, update the duet partner's audition
                    if (success && audition.auditionType.ToUpper().Equals("DUET"))
                    {
                        Student student = null;
                        int i = 0, partnerAudId = 0;
                        bool found = false;

                        //search for the correct coordinate type
                        while (i < audition.coordinates.Count && !found)
                        {
                            if (audition.coordinates[i].reason.ToUpper().Equals("DUET"))
                            {
                                student = audition.coordinates[i].student;
                                partnerAudId = GetAuditionDuetPartnerAuditionId(audition.auditionId);
                                found = true;
                            }

                            i++;
                        }

                        success = UpdateDuetPartnerDistrictAudition(audition, student, partnerAudId);
                    }
                }
                else
                    success = false;
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "UpdateStudentDistrictAudition", "auditionId: " + audition.auditionId +
                             ", grade: " + audition.student.grade + ", theoryLevel: " + audition.theoryLevel + 
                             ", instrument: " + audition.instrument + ", accompanist: " + audition.hasAccompanist() + 
                             ", accompanistName: " + audition.accompanist + ", auditionType: " + audition.auditionType + 
                             ", auditionTrack: " + audition.auditionTrack + ", coordRideType: " + audition.hasCoordinates(), 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;

        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: A student audition and all associated information is deleted
     * @param auditionId is the id of the audition to be deleted
     * @returns true if the audition was successfully deleted and false otherwise
     */
    public static bool DeleteStudentDistrictAudition(int auditionId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "DeleteStudentDistrictAudition", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;

        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: An audition is updated for a student.  This includes updating the coordinating students, if needed
     * @param audition holds all information needed for the audition
     * @returns true if the audition was successfully updated and false otherwise
     */
    public static bool UpdateStudentStateAudition(StateAudition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, set up the audition for the next year
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            connection.Open();
            string storedProc = "sp_StudentAuditionUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);
            cmd.Parameters.AddWithValue("@auditionOrgId", audition.auditionOrgId);
            cmd.Parameters.AddWithValue("@grade", audition.districtAudition.student.grade);
            cmd.Parameters.AddWithValue("@theoryLevel", audition.districtAudition.theoryLevel);
            cmd.Parameters.AddWithValue("@instrument", audition.districtAudition.instrument);
            cmd.Parameters.AddWithValue("@accompanist", audition.districtAudition.hasAccompanist());
            cmd.Parameters.AddWithValue("@accompanistName", audition.districtAudition.accompanist);
            cmd.Parameters.AddWithValue("@auditionType", audition.districtAudition.auditionType);
            cmd.Parameters.AddWithValue("@auditionTrack", audition.districtAudition.auditionTrack);
            cmd.Parameters.AddWithValue("@coordRideType", audition.hasCoordinates());
            cmd.Parameters.AddWithValue("@driveTime", audition.driveTime);

            if (audition.am)
                cmd.Parameters.AddWithValue("@request", "AM");
            else if (audition.pm)
                cmd.Parameters.AddWithValue("@request", "PM");
            else if (audition.earliest)
                cmd.Parameters.AddWithValue("@request", "E");
            else if (audition.latest)
                cmd.Parameters.AddWithValue("@request", "L");
            else
                cmd.Parameters.AddWithValue("@request", "");

            adapter.Fill(table);

            //set the ids for the audition
            if (table.Rows.Count == 1 && table.Rows[0]["UpdateMessage"].ToString().Equals("Update Complete"))
            {

                //if the audition is a duet, update the duet partner's audition
                if (success && audition.districtAudition.auditionType.ToUpper().Equals("DUET"))
                {
                    Student student = null;
                    int i = 0, partnerAudId = 0;
                    bool found = false;

                    //search for the correct coordinate type
                    while (i < audition.coordinates.Count && !found)
                    {
                        if (audition.coordinates[i].reason.ToUpper().Equals("DUET"))
                        {
                            student = audition.coordinates[i].student;
                            partnerAudId = GetAuditionDuetPartnerAuditionId(audition.auditionId);
                            found = true;
                        }

                        i++;
                    }

                    success = UpdateDuetPartnerStateAudition(audition, student, partnerAudId);
                }
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateStudentDistrictAudition", "auditionId: " + audition.auditionId +
                             ", grade: " + audition.districtAudition.student.grade + ", theoryLevel: " + 
                             audition.districtAudition.theoryLevel + ", instrument: " + audition.districtAudition.instrument + 
                             ", accompanist: " + audition.districtAudition.hasAccompanist() + ", accompanistName: " + 
                             audition.districtAudition.accompanist + ", auditionType: " + audition.districtAudition.auditionType +
                             ", auditionTrack: " + audition.districtAudition.auditionTrack + ", coordRideType: " + audition.hasCoordinates(), 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }


    /*
     * Pre:
     * Post: A new audition is created for a student.  This includes adding the audition
     *       coordinating students, if needed
     * @param audition holds all information needed for the audition
     * @returns true if the audition was successfully created and false otherwise
     */
    public static bool CreateStudentStateAudition(StateAudition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, set up the audition for the next year
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            connection.Open();
            string storedProc = "sp_StudentAuditionNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", audition.districtAudition.student.id);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@grade", audition.districtAudition.student.grade);
            cmd.Parameters.AddWithValue("@theoryLevel", audition.districtAudition.theoryLevel);
            cmd.Parameters.AddWithValue("@auditionOrgId", audition.auditionOrgId);
            cmd.Parameters.AddWithValue("@instrument", audition.instrument);
            cmd.Parameters.AddWithValue("@accompanist", audition.districtAudition.hasAccompanist());
            cmd.Parameters.AddWithValue("@accompanistName", audition.districtAudition.accompanist);
            cmd.Parameters.AddWithValue("@auditionType", audition.districtAudition.auditionType);
            cmd.Parameters.AddWithValue("@auditionTrack", "State");
            cmd.Parameters.AddWithValue("@coordRideType", audition.hasCoordinates());
            cmd.Parameters.AddWithValue("@driveTime", audition.driveTime);

            if (audition.am)
                cmd.Parameters.AddWithValue("@request", "AM");
            else if (audition.pm)
                cmd.Parameters.AddWithValue("@request", "PM");
            else if (audition.earliest)
                cmd.Parameters.AddWithValue("@request", "E");
            else if (audition.latest)
                cmd.Parameters.AddWithValue("@request", "L");
            else
                cmd.Parameters.AddWithValue("@request", "");
            
            adapter.Fill(table);

            //set the ids for the audition
            if (table.Rows.Count == 1)
            {
                int audId = Convert.ToInt32(table.Rows[0]["AuditionId"]);
                int yearId = Convert.ToInt32(table.Rows[0]["YearId"]);

                audition.setAuditionIds(audId, yearId);

                foreach (AuditionCompositions comp in audition.districtAudition.compositions)
                    success = success && CreateAuditionCompositions(audId, comp);

                //if the audition is a duet, create the duet partner's audition
                if (success && audition.districtAudition.auditionType.ToUpper().Equals("DUET"))
                {
                    Student student = null;
                    int i = 0;
                    bool found = false;

                    //search for the correct coordinate type
                    while (i < audition.coordinates.Count && !found)
                    {
                        if (audition.coordinates[i].reason.ToUpper().Equals("DUET"))
                        {
                            student = audition.coordinates[i].student;
                            found = true;
                        }
                    }

                    success = CreateDuetPartnerStateAudition(audition, student);
                }
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateStudentDistrictAudition", "studentId: " + 
                             audition.districtAudition.student.id + ", grade: " + 
                             audition.districtAudition.student.grade + ", theoryLevel: " +
                             audition.districtAudition.theoryLevel + ", auditionOrgId: " + audition.auditionOrgId +
                             ", accompanist: " + audition.districtAudition.hasAccompanist() + ", accompanistName: " +
                             audition.districtAudition.accompanist + ", auditionType: " + audition.districtAudition.auditionType +
                             ", auditionTrack: State " + ", coordRideType: " + audition.hasCoordinates() + ", driveTime: " +
                             audition.driveTime, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;

        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: A new audition is created for a student.  
     * @param audition holds all information needed for the audition
     * @returns true if the audition was successfully created and false otherwise
     */
    public static bool CreateStudentHsOrCompositionAudition(HsVirtuosoCompositionAudition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentHsOrCompositionAuditionNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", audition.student.id);
            cmd.Parameters.AddWithValue("@year", audition.year);
            cmd.Parameters.AddWithValue("@grade", audition.student.grade);
            cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
            cmd.Parameters.AddWithValue("@points", audition.points);

            adapter.Fill(table);

            //set the ids for the audition
            if (table.Rows.Count == 1)
            {
                int audId = Convert.ToInt32(table.Rows[0]["AuditionId"]);

                audition.auditionId = audId;
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateStudentHsOrCompositionAudition", "studentId: " +
                             audition.student.id + ", year: " + audition.year + ", grade: " + 
                             audition.student.grade + ", auditionType: " + audition.auditionType +
                             ", points: " + audition.points, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;

        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The audition must already exist in the database
     * Post: The points for an HS Virtuoso or Composition audition are updated
     * @param audition holds all information needed for the audition
     * @returns true if the points were successfully updated and false otherwise
     */
    public static bool UpdateStudentHsOrCompositionAudition(HsVirtuosoCompositionAudition audition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentHsOrCompositionAuditionUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);
            cmd.Parameters.AddWithValue("@points", audition.points);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "UpdateStudentHsOrCompositionAudition", "auditionId: " +
                             audition.auditionId + ", points: " + audition.points, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The input audition id and student must exist in the system
     * Post: Retrieves the audition information associated with the
     *       input audition id
     * @param student is the the student to whom the audition belongs
     * @param year is the year of the audition
     * @param auditionType is the audition type (HS Virtuoso or Composition)
     * @returns the information associated with the input audition id
     */
    public static HsVirtuosoCompositionAudition GetStudentHsOrCompositionAudition(
                                               Student student, int year, string auditionType)
    {
        HsVirtuosoCompositionAudition audition = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentHsOrCompositionAuditionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@auditionType", auditionType);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count > 0)
            {
                int auditionId = Convert.ToInt32(table.Rows[0]["AuditionId"]);
                int points = Convert.ToInt32(table.Rows[0]["Quantity"]);

                audition = new HsVirtuosoCompositionAudition(auditionId, student, year, 
                                                             points, auditionType);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentHsOrCompositionAudition", "studentId: " +
                             student.id + ", year: " + year + ", auditionType: " + auditionType, "Message: " + 
                             e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return audition;
    }

    /*
     * Pre:  The input audition id and student must exist in the system
     * Post: Retrieves the district audition information associated with
     *       the input audition id
     * @param auditionId is the id of the audition that is being retrieved
     * @param student is the student to whom the audition belongs
     * @returns the information associated with the input audition id
     */
    public static DistrictAudition GetStudentDistrictAudition(int auditionId, Student student)
    {
        DistrictAudition audition = new DistrictAudition(student);
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionSelectByAuditionId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count > 0)
            {
                string instrument = table.Rows[0]["Instrument"].ToString();
                string type = table.Rows[0]["AuditionType"].ToString();
                string track = table.Rows[0]["AuditionTrack"].ToString();
                string theoryLvl = table.Rows[0]["TheoryLevel"].ToString();
                int auditionOrgId = Convert.ToInt32(table.Rows[0]["AuditionOrgId"]);
                int yearId = Convert.ToInt32(table.Rows[0]["YearId"]);
                int districtId = Convert.ToInt32(table.Rows[0]["DistrictId"]);

                //find if there was a time request
                bool am = false, pm = false, earliest = false, latest = false;
                if (table.Rows[0]["TimeRequest"].ToString().Equals("AM"))
                    am = true;
                else if (table.Rows[0]["TimeRequest"].ToString().Equals("PM"))
                    pm = true;
                else if (table.Rows[0]["TimeRequest"].ToString().Equals("E"))
                    earliest = true;
                else if (table.Rows[0]["TimeRequest"].ToString().Equals("L"))
                    latest = true;

                //get accompanist information
                string accompanist = "";
                if (!table.Rows[0]["AccompanistNeeded"].ToString().Equals("") && (bool)table.Rows[0]["AccompanistNeeded"])
                    accompanist = table.Rows[0]["Accompanist"].ToString();

                //get theory points
                int theoryPoints = 0;
                if (!table.Rows[0]["Points"].ToString().Equals(""))
                    theoryPoints = Convert.ToInt32(table.Rows[0]["Points"]);

                //set audition information
                audition.auditionId = auditionId;
                audition.setAuditionInfo(instrument, accompanist, type, track, districtId, theoryLvl, auditionOrgId);
                audition.setTimeConstraints(am, pm, earliest, latest);
                audition.yearId = yearId;
                audition.theoryPoints = theoryPoints;

                List<AuditionCompositions> compositions = DbInterfaceStudentAudition.GetAuditionCompositions(auditionId);
                audition.setCompositions(compositions);

                List<int> id = new List<int>();
                id.Add(auditionId);
                List<StudentCoordinate> coordinates = DbInterfaceStudentAudition.GetAuditionCoordinates(id);
                audition.coordinates = coordinates;
            }
            else
                audition = null;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentDistrictAudition", "auditionId: " + audition,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            audition = null;
        }

        connection.Close();

        return audition;
    }

    /*
     * Pre:  The input audition id and student must exist in the system
     * Post: Retrieves the district audition information associated with
     *       the input state audition id and student
     * @param auditionId is the id of the state audition associated with the district audition
     * @param student is the student to whom the audition belongs
     * @returns the information associated with the input audition id
     */
    public static DistrictAudition GetStudentDistrictAuditionFromStateAudition(int stateId, Student student)
    {
        DistrictAudition districtAudition = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentDistrictAuditionSelectByStateAudition";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@stateId", stateId);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count == 1)
            {
                int districtId = Convert.ToInt32(table.Rows[0]["AuditionId"]);
                districtAudition = GetStudentDistrictAudition(districtId, student);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentDistrictAuditionFromStateAudition", "stateId: " +
                             stateId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return districtAudition;
    }

    /*
     * Pre:  The input audition id and student must exist in the system
     * Post: Retrieves the state audition information associated with
     *       the input audition id
     * @param auditionId is the id of the audition that is being retrieved
     * @param student is the student to whom the audition belongs
     * @returns the information associated with the input audition id
     */
    public static StateAudition GetStudentStateAudition(DistrictAudition distAudition, int auditionId)
    {
        StateAudition audition = new StateAudition(distAudition, auditionId);
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionSelectByAuditionId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count > 0)
            {
                string instrument = table.Rows[0]["Instrument"].ToString();
                string type = table.Rows[0]["AuditionType"].ToString();
                string track = table.Rows[0]["AuditionTrack"].ToString();
                string theoryLvl = table.Rows[0]["TheoryLevel"].ToString();
                string amPmRequest = table.Rows[0]["TimeRequest"].ToString();
                bool am = false, pm = false, earliest = false, latest = false;
                int driveTime = 0;
                if (!table.Rows[0]["DriveTimeToState"].ToString().Equals(""))
                    driveTime = Convert.ToInt32(table.Rows[0]["DriveTimeToState"].ToString());
                int auditionOrgId = Convert.ToInt32(table.Rows[0]["AuditionOrgId"]);
                string accompanist = "";

                if (amPmRequest.Equals("AM"))
                    am = true;
                else if (amPmRequest.Equals("PM"))
                    pm = true;
                else if (amPmRequest.Equals("E"))
                    earliest = true;
                else if (amPmRequest.Equals("L"))
                    latest = true;

                if (!table.Rows[0]["AccompanistNeeded"].ToString().Equals("") && (bool)table.Rows[0]["AccompanistNeeded"])
                    accompanist = table.Rows[0]["Accompanist"].ToString();

                audition.setTimeConstraints(am, pm, earliest, latest);
                audition.driveTime = driveTime;
                audition.auditionOrgId = auditionOrgId;

                List<int> id = new List<int>();
                id.Add(auditionId);
                List<StudentCoordinate> coordinates = DbInterfaceStudentAudition.GetAuditionCoordinates(id);
                audition.setStudentCoordinates(coordinates);
            }
            else
                audition = null;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentStateAudition", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            audition = null;
        }

        connection.Close();

        return audition;
    }

    /*
     * Pre:
     * Post: Retrieves the points earned for the input audition id
     * @param auditionId is the id of the audition
     * @returns the number of points entered for the audition
     */
    public static int GetStateAuditionPoints(int auditionId)
    {
        int points = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentStatePointsSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@stateAuditionId", auditionId);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count > 0)
                points = Convert.ToInt32(table.Rows[0]["Quantity"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStateAuditionPoints", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return points;
    }

    /*
     * Pre:
     * Post: Signifies whether or not the input student has any district 
     *       auditions created for the current year
     * @param student is the student whose auditions are being searched for
     * @returns true if the student has at least one district audition for the
     *          current year and false otherwise
     */
    public static bool StudentHasDistrictAudition(Student student)
    {
        bool hasAudition = false;
        int auditionOrgId = GetAuditionOrgId(student.districtId, DateTime.Now.Year);
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);
            cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count > 0) hasAudition = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "StudentHasDistrictAudition", "studentId: " + student.id +
                             "auditionOrgId: " + auditionOrgId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return hasAudition;
    }

    /*
     * Pre:
     * Post: Signifies whether or not the input student has any state 
     *       auditions created for the current year
     * @param student is the student whose auditions are being searched for
     * @returns true if the student has at least one state audition for the
     *          current year and false otherwise
     */
    public static bool StudentHasStateAudition(Student student)
    {
        bool hasAudition = false;
        int yearId = DbInterfaceStudent.GetStudentYearId(student.id);
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentStateAuditionSelectByYearId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@yearId", yearId);

            adapter.Fill(table);

            //if a result was returned the student has an audition
            if (table.Rows.Count > 0) hasAudition = true;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "StudentHasStateAudition", "studentId: " + student.id,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return hasAudition;
    }

    /*
     * Pre:
     * Post: A new audition is created for a student.  This is different from CreateStudentDistrictAudition
     *       because in that method a user entered the student's grade and other information
     */
    private static bool CreateDuetPartnerDistrictAudition(DistrictAudition audition, Student student)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, use the next year for the audition
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            int auditionOrgId = GetAuditionOrgId(audition.districtId, year);

            if (auditionOrgId != -1)
            {

                connection.Open();
                string storedProc = "sp_StudentAuditionNew";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@studentId", student.id);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@grade", student.grade);
                cmd.Parameters.AddWithValue("@theoryLevel", DbInterfaceStudent.GetTheoryLevel(student.id));
                cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
                cmd.Parameters.AddWithValue("@instrument", audition.instrument);
                cmd.Parameters.AddWithValue("@accompanist", audition.hasAccompanist());
                cmd.Parameters.AddWithValue("@accompanistName", audition.accompanist);
                cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
                cmd.Parameters.AddWithValue("@auditionTrack", audition.auditionTrack);
                cmd.Parameters.AddWithValue("@coordRideType", true);
                cmd.Parameters.AddWithValue("@driveTime", 0);

                if (audition.am)
                    cmd.Parameters.AddWithValue("@request", "AM");
                else if (audition.pm)
                    cmd.Parameters.AddWithValue("@request", "PM");
                else if (audition.earliest)
                    cmd.Parameters.AddWithValue("@request", "E");
                else if (audition.latest)
                    cmd.Parameters.AddWithValue("@request", "L");
                else
                    cmd.Parameters.AddWithValue("@request", "");

                adapter.Fill(table);

                //set the ids for the audition
                if (table.Rows.Count == 1)
                {
                    int audId = Convert.ToInt32(table.Rows[0]["AuditionId"]);
                    int yearId = Convert.ToInt32(table.Rows[0]["YearId"]);

                    //find the partner in the list of coordinates and set the audition and year ids
                    int i = 0;
                    bool found = false;

                    while (i < audition.coordinates.Count && !found)
                    {
                        if (audition.coordinates[i].reason.ToUpper().Equals("DUET"))
                        {
                            audition.coordinates[i].setAuditionYearIds(audId, yearId);
                            found = true;
                        }
                    }

                    foreach (AuditionCompositions comp in audition.compositions)
                        success = success && CreateAuditionCompositions(audId, comp);
                }
                else
                    success = false;
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateDuetPartnerDistrictAudition", "studentId: " + student.id +
                             ", grade: " + student.grade + ", theoryLevel: " + audition.theoryLevel +
                             ", instrument: " + audition.instrument + ", accompanist: " + audition.hasAccompanist() + 
                             ", accompanistName: " + audition.accompanist + ", auditionType: " + audition.auditionType + 
                             ", auditionTrack: " + audition.auditionTrack + ", coordRideType: true " , 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: An audition is updated for the input student.  This is different from UpdateStudentDistrictAudition
     *       because in that method a user entered the student's grade and other information
     * @param audition holds the audition information to be updated
     * @param student holds the information of the student
     * @returns true if the audition is successfully updated
     */
    private static bool UpdateDuetPartnerDistrictAudition(DistrictAudition audition, Student student, int partnersId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, use the next year for the audition
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            int auditionOrgId = GetAuditionOrgId(audition.districtId, year);

            if (auditionOrgId != -1)
            {

                connection.Open();
                string storedProc = "sp_StudentAuditionUpdate";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@auditionId", partnersId);
                cmd.Parameters.AddWithValue("@auditionOrgId", auditionOrgId);
                cmd.Parameters.AddWithValue("@grade", student.grade);
                cmd.Parameters.AddWithValue("@theoryLevel", DbInterfaceStudent.GetTheoryLevel(student.id));
                cmd.Parameters.AddWithValue("@instrument", audition.instrument);
                cmd.Parameters.AddWithValue("@accompanist", audition.hasAccompanist());
                cmd.Parameters.AddWithValue("@accompanistName", audition.accompanist);
                cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
                cmd.Parameters.AddWithValue("@auditionTrack", audition.auditionTrack);
                cmd.Parameters.AddWithValue("@coordRideType", audition.hasCoordinates());
                cmd.Parameters.AddWithValue("@driveTime", 0);

                if (audition.am)
                    cmd.Parameters.AddWithValue("@request", "AM");
                else if (audition.pm)
                    cmd.Parameters.AddWithValue("@request", "PM");
                else if (audition.earliest)
                    cmd.Parameters.AddWithValue("@request", "E");
                else if (audition.latest)
                    cmd.Parameters.AddWithValue("@request", "L");
                else
                    cmd.Parameters.AddWithValue("@request", "");

                adapter.Fill(table);

                //update the compositions
                if (table.Rows.Count == 1 && table.Rows[0]["UpdateMessage"].ToString().Equals("Update Complete"))
                {
                    DbInterfaceStudentAudition.DeleteAuditionCompositions(partnersId);

                    foreach (AuditionCompositions comp in audition.compositions)
                        success = success && CreateAuditionCompositions(partnersId, comp);
                }
                else
                    success = false;
            }
            else
                success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "UpdateDuetPartnerDistrictAudition", "auditionId: " + partnersId +
                             ", grade: " + student.grade + ", theoryLevel: " + audition.theoryLevel +
                             ", instrument: " + audition.instrument + ", accompanist: " + audition.hasAccompanist() +
                             ", accompanistName: " + audition.accompanist + ", auditionType: " + audition.auditionType +
                             ", auditionTrack: " + audition.auditionTrack + ", coordRideType: true ", "Message: " + e.Message +
                             "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: An audition is updated for the input student.  This is different from UpdateStudentStateAudition
     *       because in that method a user entered the student's grade and other information
     * @param audition holds the audition information to be updated
     * @param student holds the information of the student
     * @returns true if the audition is successfully updated
     */
    private static bool UpdateDuetPartnerStateAudition(StateAudition audition, Student student, int partnersId)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, use the next year for the audition
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            connection.Open();
            string storedProc = "sp_StudentAuditionUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", partnersId);
            cmd.Parameters.AddWithValue("@auditionOrgId", audition.auditionOrgId);
            cmd.Parameters.AddWithValue("@grade", student.grade);
            cmd.Parameters.AddWithValue("@theoryLevel", DbInterfaceStudent.GetTheoryLevel(partnersId));
            cmd.Parameters.AddWithValue("@instrument", audition.districtAudition.instrument);
            cmd.Parameters.AddWithValue("@accompanist", audition.districtAudition.hasAccompanist());
            cmd.Parameters.AddWithValue("@accompanistName", audition.districtAudition.accompanist);
            cmd.Parameters.AddWithValue("@auditionType", audition.districtAudition.auditionType);
            cmd.Parameters.AddWithValue("@auditionTrack", audition.districtAudition.auditionTrack);
            cmd.Parameters.AddWithValue("@coordRideType", audition.hasCoordinates());
            cmd.Parameters.AddWithValue("@driveTime", audition.driveTime);

            if (audition.am)
                cmd.Parameters.AddWithValue("@request", "AM");
            else if (audition.pm)
                cmd.Parameters.AddWithValue("@request", "PM");
            else if (audition.earliest)
                cmd.Parameters.AddWithValue("@request", "E");
            else if (audition.latest)
                cmd.Parameters.AddWithValue("@request", "L");
            else
                cmd.Parameters.AddWithValue("@request", "");

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "UpdateDuetPartnerStateAudition", "auditionId: " + partnersId +
                             ", auditionOrgId: " + audition.auditionOrgId + ", grade: " + student.grade + 
                             ", theoryLevel: " + audition.districtAudition.theoryLevel + ", instrument: " + 
                             audition.districtAudition.instrument + ", accompanist: " + audition.districtAudition.hasAccompanist() +
                             ", accompanistName: " + audition.districtAudition.accompanist + ", auditionType: " + 
                             audition.districtAudition.auditionType + ", auditionTrack: " + 
                             audition.districtAudition.auditionTrack + ", coordRideType: " + audition.hasCoordinates(), 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;

        }

        connection.Close();

        return success;
    }

    /*
     * Pre:
     * Post: A new audition is created for a student.  This is different from CreateStudentStateAudition
     *       because in that method a user entered the student's grade and other information
     */
    private static bool CreateDuetPartnerStateAudition(StateAudition audition, Student student)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            int year;

            //if the current month is June or later, use the next year for the audition
            if (DateTime.Today.Month >= 6)
                year = DateTime.Today.AddYears(1).Year;
            else
                year = DateTime.Today.Year;

            // Look at current year no matter what if on the test site
            if (Utility.reportSuffix.Equals("Test")) //delete this
                year = DateTime.Today.Year; 

            connection.Open();
            string storedProc = "sp_StudentAuditionNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@grade", student.grade);
            cmd.Parameters.AddWithValue("@theoryLevel", DbInterfaceStudent.GetTheoryLevel(student.id));
            cmd.Parameters.AddWithValue("@auditionOrgId", audition.auditionOrgId);
            cmd.Parameters.AddWithValue("@instrument", audition.instrument);
            cmd.Parameters.AddWithValue("@accompanist", audition.districtAudition.hasAccompanist());
            cmd.Parameters.AddWithValue("@accompanistName", audition.districtAudition.accompanist);
            cmd.Parameters.AddWithValue("@auditionType", audition.districtAudition.auditionType);
            cmd.Parameters.AddWithValue("@auditionTrack", "State");
            cmd.Parameters.AddWithValue("@coordRideType", true);
            cmd.Parameters.AddWithValue("@driveTime", audition.driveTime);

            if (audition.am)
                cmd.Parameters.AddWithValue("@request", "AM");
            else if (audition.pm)
                cmd.Parameters.AddWithValue("@request", "PM");
            else if (audition.earliest)
                cmd.Parameters.AddWithValue("@request", "E");
            else if (audition.latest)
                cmd.Parameters.AddWithValue("@request", "L");
            else
                cmd.Parameters.AddWithValue("@request", "");

            adapter.Fill(table);

            //set the ids for the audition
            if (table.Rows.Count == 1)
            {
                int audId = Convert.ToInt32(table.Rows[0]["AuditionId"]);
                int yearId = Convert.ToInt32(table.Rows[0]["YearId"]);

                //find the partner in the list of coordinates and set the audition and year ids
                int i = 0;
                bool found = false;

                while (i < audition.coordinates.Count && !found)
                {
                    if (audition.coordinates[i].reason.ToUpper().Equals("DUET"))
                    {
                        audition.coordinates[i].setAuditionYearIds(audId, yearId);
                        found = true;
                    }
                }

                foreach (AuditionCompositions comp in audition.districtAudition.compositions)
                    success = success && CreateAuditionCompositions(audId, comp);
            }
            success = false;
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateDuetPartnerStateAudition", "studentId: " + student.id +
                             ", grade: " + student.grade + ", theoryLevel: " + audition.districtAudition.theoryLevel + 
                             ", instrument: " + audition.instrument + ", accompanist: " + audition.districtAudition.hasAccompanist() +
                             ", accompanistName: " + audition.districtAudition.accompanist + ", auditionType: " +
                             audition.districtAudition.auditionType + ", auditionTrack: State" + ", coordRideType: true" + 
                             ", driveTime: " + audition.driveTime, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }


    /*
     * Pre:  The input audition id must exist in the system
     * Post: The duet partner associated with the input audition id is retrieved
     * @param auditionId is the id of the audition of which the duet partner is needed
     * @returns the duet partner's information
     */
    private static Student GetDuetPartner(int auditionId)
    {
        Student partner = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionCoordRideSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId1", auditionId);

            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                bool found = false;
                int i = 0;

                while (i < table.Rows.Count && !found)
                {
                    if (table.Rows[i]["CoordType"].ToString().ToUpper().Equals("DUET"))
                    {
                        int id = Convert.ToInt32(table.Rows[i]["StudentId"]);
                        string firstName = table.Rows[i]["FirstName"].ToString();
                        string mi = table.Rows[i]["MI"].ToString();
                        string lastName = table.Rows[i]["LastName"].ToString();
                        string grade = table.Rows[i]["Grade"].ToString();
                        int districtId = Convert.ToInt32(table.Rows[i]["GeoId"]);
                        int currTeacher = 0, prevTeacher = 0;
                        if (!table.Rows[0]["CurrentTeacherId"].ToString().Equals(""))
                            currTeacher = Convert.ToInt32(table.Rows[i]["CurrentTeacherId"]);
                        if (!table.Rows[0]["PreviousTeacherId"].ToString().Equals(""))
                            prevTeacher = Convert.ToInt32(table.Rows[i]["PreviousTeacherId"]);

                        partner = new Student(id, firstName, mi, lastName, grade, districtId,
                                              currTeacher, prevTeacher);
                        found = true;
                    }

                    i++;
                }
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetDuetPartner", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return partner;
    }

    /*
     * Pre:  The input audition id must exist in the system
     * Post: Retrieves the audition id of the duet partner associated with the audition id
     * @param audition id is the id of the audition associated with the partner audition
     *        being searched for
     * @returns the audition id of the duet partner or -1, if not found
     */
    public static int GetAuditionDuetPartnerAuditionId(int auditionId)
    {
        int id = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DuetPartnerAuditionIdSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId1", auditionId);

            adapter.Fill(table);

            if (table.Rows.Count > 0)
                id = Convert.ToInt32(table.Rows[0]["AuditionId"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetAuditionDuetPartnerAuditionId", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return id;
    }

    /*
     * Pre:
     * Post: Checks whether or not an audition has been created for the given
     *       district and year
     * @param geoId is the id of the district the audition is in
     * @param year is the year of the audition
     * @returns true if the audition has been created and false otherwise
     */
    public static int GetAuditionOrgId(int geoId, int year)
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

            cmd.Parameters.AddWithValue("@geoId", geoId);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);

            //if a result was returned the audition has been created
            if (table.Rows.Count == 1)
                auditionOrgId = Convert.ToInt32(table.Rows[0]["AuditionOrgId"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetAuditionOrgId", "geoId: " + geoId + ", year: " + year,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionOrgId;
    }

    /*
     * Pre:
     * Post: Returns the audition org id associated with the input student audition
     */
    public static int GetAuditionOrgIdByStudentAudition(int auditionId)
    {
        int auditionOrgId = -1;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionOrgIdSelectByStudentAuditionId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentAuditionId", auditionId);

            adapter.Fill(table);

            //if a result was returned the audition has been created
            if (table.Rows.Count == 1)
                auditionOrgId = Convert.ToInt32(table.Rows[0]["AuditionOrgId"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetAuditionOrgIdByStudentAudition", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionOrgId;
    }

    /*
     * Pre:  The audition must already exist in the system
     * Post: The composition for the audition are entered into the database
     * @param auditionId is the unique identifier of the audition
     * @param composition is the composition to be added for the audition
     * @returns true if the composition was successfully added and false otherwise
     */
    private static bool CreateAuditionCompositions(int auditionId, AuditionCompositions composition)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionCompositionNew";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);
            cmd.Parameters.AddWithValue("@compositionId", composition.composition.compositionId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateAuditionCompositions", "auditionId: " + auditionId +
                             ", compositionId: " + composition.composition.compositionId, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The audition and composition must already exist in the system
     * Post: The composition and points are updated for the input audition
     * @param auditionId is the unique identifier for the audition
     * @param composition holds the composition information and point total
     * @returns true if the composition was successfully updated and false otherwise
     */
    public static bool UpdateAuditionCompositionPoints(DistrictAudition audition, 
                                              AuditionCompositions composition, string auditionLevel)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentPointsUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);
            cmd.Parameters.AddWithValue("@compositionId", composition.composition.compositionId);
            cmd.Parameters.AddWithValue("@qty", composition.points);
            cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
            cmd.Parameters.AddWithValue("@auditionLevel", auditionLevel);
            cmd.Parameters.AddWithValue("@studentId", audition.student.id);
 
            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "UpdateAuditionCompositionPoints", "auditionId: " + audition.auditionId +
                             ", compositionId: " + composition.composition.compositionId + ", qty: " + composition.points +
                             ", auditionType:  " + audition.auditionType + ", auditionLevel: " + auditionLevel + ", studentId: " +
                             audition.student.id, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The audition must already exist in the system
     * Post: The composition and points are updated for the input audition
     * @param auditionId is the unique identifier for the audition
     * @param composition holds the composition information and point total
     * @returns true if the composition was successfully updated and false otherwise
     */
    public static bool UpdateAuditionTheoryPoints(DistrictAudition audition, int points)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentPointsUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);
            cmd.Parameters.AddWithValue("@compositionId", 2);
            cmd.Parameters.AddWithValue("@qty", points);
            cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
            cmd.Parameters.AddWithValue("@auditionLevel", "District");
            cmd.Parameters.AddWithValue("@studentId", audition.student.id);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "UpdateAuditionTheoryPoints", "auditionId: " + audition.auditionId +
                             ", compositionId: 2, qty: " + points + ", auditionType: " + audition.auditionType +
                             ", auditionLevel: District, studentId: " + audition.student.id, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The audition must already exist in the system
     * Post: The compositions for the audition are deleted from the database
     * @param auditionId is the unique identifier of the audition
     */
    private static void DeleteAuditionCompositions(int auditionId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionCompositionsDelete";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "DeleteAuditionCompositions", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();
    }

    /*
     * Pre:  The input audition id must exist in the system
     * Post: The compositions for the audition are retrieved
     * @param auditionId is the id of the audition for which the compositions are needed
     * @returns a list of the compositions for the audition
     */
    public static List<AuditionCompositions> GetAuditionCompositions(int auditionId)
    {
        List<AuditionCompositions> compositions = new List<AuditionCompositions>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionCompositionSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);

            //add each composition to list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int compId = Convert.ToInt32(table.Rows[i]["CompositionId"]);
                string name = table.Rows[i]["Composition"].ToString();
                string composer = table.Rows[i]["Composer"].ToString();
                string style = table.Rows[i]["Style"].ToString();
                string level = table.Rows[i]["CompLevel"].ToString();

                int points = 0;
                if (!table.Rows[i]["Quantity"].ToString().Equals(""))
                    points = Convert.ToInt32(table.Rows[i]["Quantity"]);

                double time = 0;
                if (!table.Rows[0]["PlayingTime"].ToString().Equals(""))
                    time = Convert.ToDouble(table.Rows[i]["PlayingTime"]);

                Composition comp = new Composition(compId, name, composer, style, level, time);
                AuditionCompositions audComp = new AuditionCompositions(comp, points);

                compositions.Add(audComp);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetAuditionCompositions", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return compositions;
    }

    /*
     * Pre:  The audition must already exist in the system
     * Post: The coordinates for the audition are entered into the database
     * @param auditionId is the unique identifier of the audition
     * @param coords is the list of coordinates to be added for the audition
     * @returns true if the coordinates were successfully added and false otherwise
     */
    public static bool CreateAuditionCoordinate(int auditionId1, int auditionId2, string reason)
    {
        bool success = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionCoordRideNew";
           
            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId1", auditionId1);
            cmd.Parameters.AddWithValue("@auditionId2", auditionId2);
            cmd.Parameters.AddWithValue("@type", reason);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "CreateAuditionCoordinate", "auditionId1: " + auditionId1 +
                             ", auditionId2: " + auditionId2 + ", reason: " + reason, "Message: " + e.Message + 
                             "   Stack Trace: " + e.StackTrace, -1);
            success = false;
        }

        connection.Close();

        return success;
    }

    /*
     * Pre:  The input audition id must already exist in the system
     * Post: The coordinates from the input list that are in the system are removed
     * @param coordinates is the list of coordinates that are to be removed from the database
     * @param auditionId is an audition id associated with the coordinate
     */
    public static void RemoveAuditionCoordinates(List<StudentCoordinateSimple> coordinates, int auditionId)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        //remove each input coordinate
        foreach (StudentCoordinateSimple coord in coordinates)
        {
            try
            {
                connection.Open();
                string storedProc = "sp_StudentAuditionCoordRideDelete";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@auditionId1", auditionId);
                cmd.Parameters.AddWithValue("@auditionId2", coord.auditionId);
                cmd.Parameters.AddWithValue("@type", coord.reason);

                adapter.Fill(table);
            }
            catch (Exception e)
            {
                Utility.LogError("DbInterfaceStudentAudition", "RemoveAuditionCoordinates", "auditionId1: " + auditionId +
                                 ", auditionId2: " + coord.auditionId + ", type: " + coord.reason, "Message: " + 
                                 e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            connection.Close();
        }
    }

    /*
     * Pre:
     * Post: Any students that have been coordinated with other auditions for the current
     *       student are coordinated with the new audition and the new audition is coordinated
     *       with the existing coordinates for the other students
     * @param student is the student who had a new audition added
     * @param auditionIds is the list of audition ids for the input student
     * @returns true if the coordinates were successfully created
     */
    public static bool UpdateExistingCoordinates(List<int> auditionIds)
    {
        bool success = true;
        List<StudentCoordinateSimple> coordinates = GetAllCoordinatesNotDuet(auditionIds);
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        //add coordinate records for each combination of audition ids
        foreach (StudentCoordinateSimple currCoord in coordinates)
        {
            foreach (int audId in auditionIds)
            {
                try
                {
                    connection.Open();
                    string storedProc = "sp_StudentAuditionCoordRideNew";

                    SqlCommand cmd = new SqlCommand(storedProc, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@auditionId1", audId);
                    cmd.Parameters.AddWithValue("@auditionId2", currCoord.auditionId);
                    cmd.Parameters.AddWithValue("@type", currCoord.reason);

                    adapter.Fill(table);
                }
                catch (Exception e)
                {
                    Utility.LogError("DbInterfaceStudentAudition", "UpdateExistingCoordinates", "auditionId1: " + audId +
                                     ", auditionId2: " + currCoord.auditionId + ", type: " + currCoord.reason,
                                     "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
                    success = false;
                }

                connection.Close();
            }
        }

        return success;
    }

    /*
     * Pre:
     * Post: Gets coordinate records that are coordinated with one or more audition ids
     *       in the input list 
     * @param auditionIds is the list of audition ids for which coordinating 
     *        auditions need to be found
     * @returns a list of auditions that are coordinated with ids in the input list
     */
    private static List<StudentCoordinate> GetAuditionCoordinates(List<int> auditionIds)
    {
        List<StudentCoordinate> coordinates = new List<StudentCoordinate>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        //look for coordinates for each id
        foreach (int id in auditionIds)
        {
            try
            {
                connection.Open();
                string storedProc = "sp_StudentAuditionCoordRideSelect";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@auditionId1", id);

                adapter.Fill(table);

                //add all unique coordinates to the list.  
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string reason = table.Rows[i]["CoordType"].ToString().ToUpper();
                    int studId = Convert.ToInt32(table.Rows[i]["StudentId"]);
                    string firstName = table.Rows[i]["FirstName"].ToString();
                    string mi = table.Rows[i]["MI"].ToString();
                    string lastName = table.Rows[i]["LastName"].ToString();
                    string grade = table.Rows[i]["Grade"].ToString();
                    int districtId = Convert.ToInt32(table.Rows[0]["GeoId"]);
                    int auditionGeoId = Convert.ToInt32(table.Rows[0]["AuditionGeoId"]);
                    int currTeacher = 0, prevTeacher = 0;
                    if (!table.Rows[i]["CurrentTeacherId"].ToString().Equals(""))
                        currTeacher = Convert.ToInt32(table.Rows[i]["CurrentTeacherId"]);
                    if (!table.Rows[i]["PreviousTeacherId"].ToString().Equals(""))
                        prevTeacher = Convert.ToInt32(table.Rows[i]["PreviousTeacherId"]);
                    Student student = new Student(studId, firstName, mi, lastName, grade, districtId,
                                                  currTeacher, prevTeacher);

                    //get all other audition ids for the duet partner and add 
                    int currId = Convert.ToInt32(table.Rows[i]["AuditionId2"]);
                    bool isDistrictAudition = auditionGeoId != 15 && auditionGeoId != 16 && auditionGeoId != 19 
                                           && auditionGeoId != 20 && auditionGeoId != 21 && auditionGeoId != 22
                                           && auditionGeoId != 23 && auditionGeoId != 24;
                    //List<int> coordIds = GetStudentAuditionsFromAuditionId(currId);

                    StudentCoordinate coord = new StudentCoordinate(student, reason, true, isDistrictAudition);
                    //coord.setAuditionIds(coordIds);

                    if (!coordinates.Contains(coord)) coordinates.Add(coord);
                }
            }
            catch (Exception e)
            {
                Utility.LogError("DbInterfaceStudentAudition", "GetAuditionCoordinates", "auditionId1: " + id, "Message: " +
                                 e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            connection.Close();
        }

        return coordinates;
    }
    /*
     * Pre:
     * Post: Gets coordinate records that are coordinated with one or more audition ids
     *       in the input list that are not duets
     * @param auditionIds is the list of audition ids for which coordinating 
     *        auditions need to be found
     * @returns a list of auditions that are coordinated with ids in the input list
     */
    private static List<StudentCoordinateSimple> GetAllCoordinatesNotDuet(List<int> auditionIds)
    {
        List<StudentCoordinateSimple> coordinates = new List<StudentCoordinateSimple>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        //look for coordinates for each id
        foreach (int id in auditionIds)
        {
            try
            {
                connection.Open();
                string storedProc = "sp_StudentAuditionCoordRideSelect";

                SqlCommand cmd = new SqlCommand(storedProc, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@auditionId1", id);

                adapter.Fill(table);

                //add all unique coordinates to the list.  If the coordinate reason is duet
                //make sure all of the duet partners other auditions have been coordinated with
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string reason = table.Rows[i]["CoordType"].ToString().ToUpper();

                    if (!reason.Equals("DUET"))
                    {
                        int currId = Convert.ToInt32(table.Rows[i]["AuditionId2"]);
                        StudentCoordinateSimple coord = new StudentCoordinateSimple(currId, reason);

                        if (!coordinates.Contains(coord)) coordinates.Add(coord);
                    }
                    else
                    {
                        //get all other audition ids for the duet partner and add as carpool
                        int currId = Convert.ToInt32(table.Rows[i]["AuditionId2"]);
                        List<int> coordIds = GetStudentAuditionsFromAuditionId(currId);

                        foreach (int audId in coordIds)
                        {
                            StudentCoordinateSimple coord = new StudentCoordinateSimple(audId, "CARPOOL");

                            if (!coordinates.Contains(coord)) coordinates.Add(coord);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LogError("DbInterfaceStudentAudition", "GetAllCoordinatesNotDuet", "auditionId1: " + id, "Message: " +
                                 e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            connection.Close();
        }

        return coordinates;
    }

    /*
     * Pre:  The input audition id must exist for a student
     * Post: All audition ids for the student with the input audition id are retrieved
     * @param auditionId is one audition id for a student that will be used to find
     *        the student's other audition ids
     * @returns a list containing all other audition ids for the student
     */
    private static List<int> GetStudentAuditionsFromAuditionId(int auditionId)
    {
        List<int> auditionIds = new List<int>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionSelectByAuditionId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);

            //add all unique auditions to the list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int currId = Convert.ToInt32(table.Rows[i]["AuditionId"]);

                if (!auditionIds.Contains(currId)) auditionIds.Add(currId);
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentAuditionsFromAuditionId", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionIds;
    }

    

    /*
     * Pre:
     * Post: Gets the list of district audition ids associated with a given year id
     * @param yearId is the year id associated with a student's auditions for a given year
     * returns a list of the audition ids associated with the input year id
     */
    public static List<int> GetStudentDistrictAuditionIds(int yearId)
    {
        List<int> auditionIds = new List<int>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionSelectByYearId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@yearId", yearId);

            adapter.Fill(table);

            //add each district audition id to list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int geoId = Convert.ToInt32(table.Rows[i]["DistrictId"]);
                if (geoId != 15 && geoId != 16 && geoId != 19 && geoId != 20 && geoId != 21 && 
                    geoId != 22 && geoId != 23 && geoId != 24)
                    auditionIds.Add(Convert.ToInt32(table.Rows[i]["AuditionId"]));
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentDistrictAuditionIds", "yearId: " + yearId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionIds;
    }

    /*
    * Pre:
    * Post: Gets the list of state audition ids associated with a given year id
    * @param yearId is the year id associated with a student's auditions for a given year
    * returns a list of the audition ids associated with the input year id
    */
    public static List<int> GetStudentStateAuditionIds(int yearId)
    {
        List<int> auditionIds = new List<int>();
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionSelectByYearId";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@yearId", yearId);

            adapter.Fill(table);

            //add each state audition id to list
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int geoId = Convert.ToInt32(table.Rows[i]["DistrictId"]);
                if (geoId == 15 || geoId == 16 || geoId == 20 || geoId == 21 || geoId == 22)
                    auditionIds.Add(Convert.ToInt32(table.Rows[i]["AuditionId"]));
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentStateAuditionIds", "yearId: " + yearId, "Message: " +
                                 e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return auditionIds;
    }

    
    /*
     * Pre:  The input student must exist in the system
     * Post: Retrieves the student's District Auditions for the upcoming audition
     * @param student is the student whose auditions are being returned
     * @returns a data table containing the student's auditions
     */
    public static DataTable GetDistrictAuditionsForDropdown(Student student)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownDistrictAuditionOptions";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetDistrictAuditionsForDropdown", "studentId: " + student.id,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
    public static DataTable GetDistrictAuditionsForPointDropdown(Student student)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownDistrictAuditionPointOptions";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetDistrictAuditionsForPointDropdown", "studentId: " + student.id,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input student must exist in the system
     * Post: Retrieves the student's State Auditions for the upcoming audition
     * @param student is the student whose auditions are being returned
     * @returns a data table containing the student's auditions
     */
    public static DataTable GetStateAuditionsForDropdown(Student student)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownStateAuditionOptions";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStateAuditionsForDropdown", "studentId: " + student.id,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

       connection.Close();

        return table;
    }

    /*
     * Pre:  The input student must exist in the system
     * Post: Retrieves the student's District Auditions for the input year
     * @param student is the student whose auditions are being returned
     * @param year is the year of the auditions being returned
     * @returns a data table containing the student's auditions
     */
    public static DataTable GetDistrictAuditionsForDropdownByYear(Student student, int year)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownDistrictAuditionOptionsByYear";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetDistrictAuditionsForDropdownByYear", "studentId: " + 
                             student.id + ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input student must exist in the system
     * Post: Retrieves the student's State Auditions for the input year
     * @param student is the student whose auditions are being returned
     * @param year is the year of the auditions being returned
     * @returns a data table containing the student's auditions
     */
    public static DataTable GetStateAuditionsForDropdownByYear(Student student, int year)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownStateAuditionOptionsByYear";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);
            cmd.Parameters.AddWithValue("@year", year);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStateAuditionsForDropdownByYear", "studentId: " + student.id +
                             ", year: " + year, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:  The input student must exist in the system
     * Post: Retrieves the District Audition information for auditions that received 
     *       scores of 14 or 15 for compositions and 4 or 5 for the theory test
     * @param student is the student whose auditions are being returned
     * @returns a data table containing the eligible auditions
     */
    public static DataTable GetAuditionsEligibleForState(Student student)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownStateCompOptions";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", student.id);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetAuditionsEligibleForState", "studentId: " + student.id, 
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: Retrieves the state audition sites based on the  input
     *       instrument
     * @param instrument is the instrument for which the state audition
     *        site is needed
     * @returns a DataTable containing the state sites
     */
    public static DataTable GetStateSites(string instrument)
    {
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_DropDownStateAuditionSite";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@instrument", instrument.ToUpper());

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStateSites", "instrument: " + instrument,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return table;
    }

    /*
     * Pre:
     * Post: Returns the first year of recorded auditions
     */
    public static int GetFirstAuditionYear()
    {
        int firstYear = DateTime.Now.Year;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_AuditionGetFirstYear";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            adapter.Fill(table);

            if (table.Rows.Count == 1)
                firstYear = Convert.ToInt32(table.Rows[0]["Year"]);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetFirstAuditionYear", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
        }

        connection.Close();

        return firstYear;
    }

    /*
     * Pre: The input audition id must have been previously created
     * Post: Points are entered for the input audition id
     * @param auditionId is the id of the audition 
     * @param points is the number of points earned
     */
    public static bool EnterStateAuditionPoints(StateAudition audition)
    {
        bool result = true;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentPointsUpdate";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", audition.auditionId);
            cmd.Parameters.AddWithValue("@compositionId", 0);
            cmd.Parameters.AddWithValue("@qty", audition.points);
            cmd.Parameters.AddWithValue("@auditionType", audition.auditionType);
            cmd.Parameters.AddWithValue("@auditionLevel", "State");
            cmd.Parameters.AddWithValue("@studentId", audition.districtAudition.student.id);

            adapter.Fill(table);
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "EnterStateAuditionPoints", "auditionId: " + audition.auditionId +
                             ", compositionId: 0, qty: " + audition.points + ", auditionType: " + audition.auditionType +
                             ", auditionLevel: State, studentId: " + audition.districtAudition.student.id,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            result = false;
        }

        connection.Close();

        return result;
    }

    /*
     * Pre: The input audition id must have been previously created and scheduled
     * Post: The schedule information for the input audition is retrieved
     * @param auditionId is the id of the student's audition
     */
    public static ScheduleSlot GetStudentAuditionSchedule(int auditionId)
    {
        ScheduleSlot scheduleSlot = null;
        DataTable table = new DataTable();
        SqlConnection connection = new
            SqlConnection(ConfigurationManager.ConnectionStrings["WmtaConnectionString"].ConnectionString);

        try
        {
            connection.Open();
            string storedProc = "sp_StudentAuditionScheduleSelect";

            SqlCommand cmd = new SqlCommand(storedProc, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@auditionId", auditionId);

            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                int judgeId = -1, minutes = 0;
                TimeSpan startTime = TimeSpan.MinValue;

                string judgeName = table.Rows[0]["JudgeName"].ToString();
                string studentName = table.Rows[0]["StudentName"].ToString();
                Int32.TryParse(table.Rows[0]["JudgeId"].ToString(), out judgeId);
                Int32.TryParse(table.Rows[0]["Minutes"].ToString(), out minutes);
                TimeSpan.TryParse(table.Rows[0]["AuditionStartTime"].ToString(), out startTime);

                scheduleSlot = new ScheduleSlot(auditionId, judgeId, judgeName, minutes, startTime, "", "", "", "", "", -1);
                scheduleSlot.StudentName = studentName;
            }
        }
        catch (Exception e)
        {
            Utility.LogError("DbInterfaceStudentAudition", "GetStudentAuditionSchedule", "auditionId: " + auditionId,
                             "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            scheduleSlot = null;
        }

        connection.Close();

        return scheduleSlot;
    }
}
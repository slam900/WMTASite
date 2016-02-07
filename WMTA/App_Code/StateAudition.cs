using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   04/24/13
 * This class represents a Badger State Audition in the system. Each Badger
 * Audition must have an associated District Audition in order to be created
 * successfully
 */
public class StateAudition
{
    public DistrictAudition districtAudition { get; private set; }
    public int auditionId { get; set; }
    public int yearId { get { return this.districtAudition.yearId; } }
    public int auditionOrgId { get; set; }
    public int driveTime { get; set; }
    public bool am { get; private set; }
    public bool pm { get; private set; }
    public bool earliest { get; private set; }
    public bool latest { get; private set; }
    public List<StudentCoordinate> coordinates { get; private set; }
    public int points { get; set; }
    public string auditionType { get { return this.districtAudition.auditionType; } }
    public string instrument { get { return this.districtAudition.instrument; } }
    public int siteId { get; set; }
    public string awards { get; set; }

    /*
     * Pre:  The entered  audition id and student must exist in the system
     * Post: The State Audition is instantiated based off of its own id or the 
     *       associated district audition id
     * @param auditionId is the audition id of the audition or the associated district audition
     * @param student is the student associated with the audition
     * @param isIdDistrictId indicates whether the input id belongs to the state or district audition
     */
    public StateAudition(int auditionId, Student student, bool idIsDistrictId)
    {
        //if the input id was the district audition id, load the district audition
        if (idIsDistrictId)
            loadDistrictAudition(auditionId, student);
        //if the input id was the state audition id, load the state audition
        else
        {
            this.auditionId = auditionId;
            //TODO currently doesn't load coordinates, compositions, time constraints, etc.
            getAssociatedDistrictAudition(student);
        }

        am = false;
        pm = false;
        earliest = false;
        latest = false;
        coordinates = new List<StudentCoordinate>();
        points = 0;
    }

    public StateAudition(DistrictAudition districtAudition)
    {
        this.districtAudition = districtAudition;

        am = false;
        pm = false;
        earliest = false;
        latest = false;
        coordinates = new List<StudentCoordinate>();
    }

    public StateAudition(DistrictAudition districtAudition, int auditionId)
    {
        this.districtAudition = districtAudition;
        this.auditionId = auditionId;
        getAuditionPoints();

        am = false;
        pm = false;
        earliest = false;
        latest = false;
        coordinates = new List<StudentCoordinate>();
    }

    /*
     * Pre: The entered district audition id and student must exist in the system
     * Post: The district audition information is retrieved
     * @param districtAuditionId is the audition id of the associated
     *        district audition
     */
    private void loadDistrictAudition(int districtAuditionId, Student student)
    {
        districtAudition = DbInterfaceStudentAudition.GetStudentDistrictAudition(districtAuditionId, student);
    }

    /*
     * Pre: The entered student must exist in the system
     * Post: The district audition information is retrieved
     * @param student is the student associated with the audition
     */
    private void getAssociatedDistrictAudition(Student student)
    {
        districtAudition = DbInterfaceStudentAudition.GetStudentDistrictAuditionFromStateAudition(auditionId, student);
    }

    /*
     * Pre:  The current audition must already exist in the system
     * Post: The audition points are loaded if there are any
     */
    private void getAuditionPoints()
    {
        points = DbInterfaceStudentAudition.GetStateAuditionPoints(auditionId);
    }

    /*
     * Pre:  If the user did not specify a to and from time, they should be entered 
     *       as the minimum DateTime value.
     * Post: The time constraints for the audition are set.  
     * @param am signifies that the student would prefer a morning audition
     * @param pm signifies that the student would prefer an afternoon audition
     * @param earliest signifies if the student cannot audition until a certain time
     * @param latest signifies if the student cannot audition after a certain time
     */
    public void setTimeConstraints(bool am, bool pm, bool earliest, bool latest)
    {
        this.am = am;
        this.pm = pm;
        this.earliest = earliest;
        this.latest = latest;
    }

    /*
     * Pre:
     * Post: The audition id for the current audition is set
     * @param auditionId is the unique identifier used to identify this 
     *        specific audition in the database
     */
    public void setAuditionIds(int auditionId, int yearId)
    {
        this.auditionId = auditionId;
        this.districtAudition.yearId = yearId;
    }

    /*
     * Pre:
     * Post: The new coordinate student is added to the audition's list of coordinates
     * @coordinate holds the coordinate information
     */
    public void addStudentCoordinate(StudentCoordinate coordinate)
    {
        coordinates.Add(coordinate);
    }

    /*
     * Pre:
     * Post: The coordinates in the input list are set as the audition's coordinates
     * @param coordinates is the list of coordinates
     */
    public void setStudentCoordinates(List<StudentCoordinate> coordinates)
    {
        this.coordinates = coordinates;
    }

    /*
     * Pre:
     * Post: Adds the new audition to thr database and updates all coordinate information for the student
     */
    public bool addToDatabase()
    {
        bool success = DbInterfaceStudentAudition.CreateStudentStateAudition(this);

        if (success)
        {
            //create coordinates
            if (coordinates.Count > 0)
                success = success && createCoordinateRides();

            //update coordinate rides that already exist for the student
            List<int> existingAuditionIds = DbInterfaceStudentAudition.GetStudentStateAuditionIds(yearId);
            success = success && DbInterfaceStudentAudition.UpdateExistingCoordinates(existingAuditionIds);
        }

        return success;
    }

    /*
     * Pre:  The audition must already exist in the database
     * Post: The audition is updated with the current information
     * @param coordinatesToRemove contains a list of coordinates that were removed
     *        from the audition
     */
    public bool updateInDatabase(List<StudentCoordinateSimple> coordinatesToRemove)
    {
        bool success = DbInterfaceStudentAudition.UpdateStudentStateAudition(this);

        //update coordinates
        if (success)
        {
            //if coordinates were removed from the audition, remove them in the database
            if (coordinatesToRemove.Count > 0)
                removeCoordinates(coordinatesToRemove);

            //update coordinates
            foreach (StudentCoordinate coord in coordinates)
            {
                if (coord.auditionIds.Count() > 0)
                    success = success && DbInterfaceStudentAudition.CreateAuditionCoordinate(auditionId, coord.auditionIds.ElementAt(0), coord.reason);
                success = success && DbInterfaceStudentAudition.UpdateExistingCoordinates(DbInterfaceStudentAudition.GetStudentStateAuditionIds(DbInterfaceStudent.GetStudentYearId(coord.student.id)));
            }

            if (coordinates.Count > 0)
                success = success && DbInterfaceStudentAudition.UpdateExistingCoordinates(DbInterfaceStudentAudition.GetStudentStateAuditionIds(DbInterfaceStudent.GetStudentYearId(districtAudition.student.id)));

        }

        return success;
    }

    /*
     * Pre:
     * Post: The audition information is deleted from the database
     * @returns true if the audition is successfully deleted and false otherwise
     */
    public bool deleteFromDatabase()
    {
        return DbInterfaceStudentAudition.DeleteStudentDistrictAudition(auditionId);
    }

    /*
     * Pre:  The audition must have previously existed in the system
     * Post: The coordinates in the input list are removed from the system for each of the current student's auditions
     * @param coordinatesToRemove contains a list of coordinates that were removed
     *        from the audition
     */
    private void removeCoordinates(List<StudentCoordinateSimple> coordinatesToRemove)
    {
        List<int> currStudAuditionIds = DbInterfaceStudentAudition.GetStudentStateAuditionIds(DbInterfaceStudent.GetStudentYearId(districtAudition.student.id));

        foreach (int id in currStudAuditionIds)
        {
            DbInterfaceStudentAudition.RemoveAuditionCoordinates(coordinatesToRemove, id);
        }
    }

    /*
     * Pre: The audition must have been previously added to the database
     * Post: The audition points are added to the database
     */
    public bool submitPoints()
    {
        //enter points
        bool success = DbInterfaceStudentAudition.EnterStateAuditionPoints(this);

        //add partner points if audition is a duet
        if (auditionType.ToUpper().Equals("DUET"))
            submitDuetPartnerPoints();

        return success;
    }

    /*
     * Pre:  The audition must have been previously added to the database
     * Post: The audition points for the duet partner associated with the current audition are updated
     */
    private bool submitDuetPartnerPoints()
    {
        //get partner id
        int duetPartnerId = DbInterfaceStudentAudition.GetAuditionDuetPartnerAuditionId(auditionId);
        int temp;

        //switch audition id to duet partner's id for point entry
        temp = auditionId;
        auditionId = duetPartnerId;

        //enter points
        bool success = DbInterfaceStudentAudition.EnterStateAuditionPoints(this);

        //change back to original id
        auditionId = temp;

        return success;
    }

    /*
     * Pre:  Auditions of both students must have been previously created
     * Post: A new coordinating student entry is created
     */
    private bool createCoordinateRides()
    {
        bool success = true;

        foreach (StudentCoordinate coord in coordinates)
        {
            foreach (int i in coord.auditionIds)
                success = success && DbInterfaceStudentAudition.CreateAuditionCoordinate(auditionId, i, coord.reason);
        }

        return success;
    }

    /*
     * Pre:
     * Post: Checks whether or not the audition needs to be coordinated with other
     *       student's auditions
     * @param returns true if coordination is needed and false otherwise
     */
    public bool hasCoordinates() { return coordinates.Count > 0; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   03/11/13
 * This class represents a District Audition in the system.
 */
public class DistrictAudition
{
    public int auditionId { get; set; }
    public int yearId { get; set; }
    public Student student { get; private set; }
    public string instrument { get; private set; }
    public string accompanist { get; private set; }
    public string auditionType { get; set; }
    public string auditionTrack { get; private set; }
    public string theoryLevel { get; private set; }
    public bool am { get; private set; }
    public bool pm { get; private set; }
    public bool earliest { get; private set; }
    public bool latest { get; private set; }
    public List<AuditionCompositions> compositions { get; private set; }
    public List<StudentCoordinate> coordinates { get; set; }
    public int districtId { get; private set; }
    public int auditionOrgId { get; private set; }
    public int theoryPoints { get; set; }
    public double auditionLength { get; set; }

    /*
     * Pre:
     * Post: The student for the audition is set
     */
    public DistrictAudition(Student student)
    {
        this.student = student;
        compositions = new List<AuditionCompositions>();
        am = false;
        pm = false;
        earliest = false;
        latest = false;
        coordinates = new List<StudentCoordinate>();
        auditionLength = 0;
    }

    /*
     * Pre:
     * Post: The base audition information for the audition is set
     */
    public void setAuditionInfo(string instrument, string accompanist, string auditionType, string auditionTrack,
                                int districtId, string theoryLevel, int auditionOrgId)
    {
        this.instrument = instrument;
        this.accompanist = accompanist;
        this.auditionType = auditionType;
        this.auditionTrack = auditionTrack;
        this.theoryLevel = theoryLevel;
        this.districtId = districtId;
        this.auditionOrgId = auditionOrgId;
    }

    /*
     * Pre:
     * Post: A new composition is added to the list of compositions for the audition
     */
    public void addComposition(AuditionCompositions newComp)
    {
        compositions.Add(newComp);

        auditionLength = auditionLength + newComp.composition.playingTime;
    }

    /*
     * Pre:
     * Post: Sets the compositions for the audition equal to the compositions
     *       in the input list
     */
    public void setCompositions(List<AuditionCompositions> comps)
    {
        compositions = comps;

        //auditionLength = 0;

        //foreach (AuditionCompositions comp in comps)
        //    auditionLength = auditionLength + comp.composition.playingTime;
    }

    /* 
     * Pre:  The entered composition id must refer to a composition 
     *       in the current list of compositions
     * Post: The point total is updated 
     * @param compId is the id of the compositions that the input
     *        points are associated with
     * @param points is the point total for the composition
     */
    public void setCompositionPoints(int compId, int points)
    {
        bool found = false;
        int i = 0;

        //search for the composition with the input composition id and
        //update when found
        while (i < compositions.Count && !found)
        {
            if (compositions[i].composition.compositionId == compId)
            {
                compositions[i].points = points;
                found = true;
            }

            i++;
        }
    }

    /*
     * Pre:  The compositions must have previously existed in the database
     * Post: The composition and theory points are updated in the database
     */
    public bool submitPoints()
    {
        bool success = true;

        foreach (AuditionCompositions audComp in compositions)
            success = success && DbInterfaceStudentAudition.UpdateAuditionCompositionPoints(this, audComp, "District");

        success = success && DbInterfaceStudentAudition.UpdateAuditionTheoryPoints(this, theoryPoints);
        
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
        bool success = true;

        //switch audition id to duet partner's id for point entry
        temp = auditionId;
        auditionId = duetPartnerId;

        //enter points
        foreach (AuditionCompositions audComp in compositions)
            success = success && DbInterfaceStudentAudition.UpdateAuditionCompositionPoints(this, audComp, "District");

        //change back to original id
        auditionId = temp;

        return success;
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
     * Post: The new coordinate student is added to the audition's list of coordinates
     * @param coordinate holds the coordinate information
     */
    public void addStudentCoordinate(StudentCoordinate coordinate)
    {
        coordinates.Add(coordinate);
    }

    /*
     * Pre:
     * Post: Adds the new audtion to the database and updates all 
     *       coordinate information for the student
     */
    public bool addToDatabase()
    {
        bool success = DbInterfaceStudentAudition.CreateStudentDistrictAudition(this);

        if (success)
        {
            //create coordinates
            if (coordinates.Count > 0)
                success = success && createCoordinateRides();

            //update coordinate rides that already exist for the student
            List<int> existingAuditionIds = DbInterfaceStudentAudition.GetStudentDistrictAuditionIds(yearId);
            success = success && DbInterfaceStudentAudition.UpdateExistingCoordinates(existingAuditionIds);
        }

        return success;
    }

    /*
     * Pre:  The audition must have previously existed in the system
     * Post: The audition information is updated in the database
     * @param coordinatesToRemove contains a list of coordinates that were removed
     *        from the audition 
     * @returns true if the audition is successfully updated and false otherwise
     */
    public bool updateInDatabase(List<StudentCoordinateSimple> coordinatesToRemove)
    {
        bool success = DbInterfaceStudentAudition.UpdateStudentDistrictAudition(this);

        //update coordinates
        if (success)
        {
            //if coordinates were removed from the audition, remove them in the database
            if (coordinatesToRemove.Count > 0)
                removeCoordinates(coordinatesToRemove);

            //update coordinates
            foreach (StudentCoordinate coord in coordinates)
            {
                success = success && DbInterfaceStudentAudition.CreateAuditionCoordinate(auditionId, coord.auditionIds.ElementAt(0), coord.reason);
                success = success && DbInterfaceStudentAudition.UpdateExistingCoordinates(DbInterfaceStudentAudition.GetStudentDistrictAuditionIds(DbInterfaceStudent.GetStudentYearId(coord.student.id)));
            }

            if (coordinates.Count > 0)
                success = success && DbInterfaceStudentAudition.UpdateExistingCoordinates(DbInterfaceStudentAudition.GetStudentDistrictAuditionIds(DbInterfaceStudent.GetStudentYearId(student.id)));
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
        List<int> currStudAuditionIds = DbInterfaceStudentAudition.GetStudentDistrictAuditionIds(DbInterfaceStudent.GetStudentYearId(student.id));

        foreach (int id in currStudAuditionIds)
        {
            DbInterfaceStudentAudition.RemoveAuditionCoordinates(coordinatesToRemove, id);
        }
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
     * Post: Checks whether or not the student requires an accompanist for
     *       this audition
     * @returns true if there will be an accompanist and false otherwise
     */
    public bool hasAccompanist() { return !accompanist.Equals(""); }

    /*
     * Pre:
     * Post: Checks whether or not the audition needs to be coordinated with other
     *       student's auditions
     * @param returns true if coordination is needed and false otherwise
     */
    public bool hasCoordinates() { return coordinates.Count > 0; }

    
}
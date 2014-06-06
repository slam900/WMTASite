using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   03/11/13
 * This class holds the necessary data to add a coordinating student
 * to an audition
 */
public class StudentCoordinate
{
    public Student student { get; private set; }
    public string reason { get; private set; }
    public int yearId { get; private set; }
    public List<int> auditionIds { get; private set; }   //holds all audition ids for the student for the current year

	public StudentCoordinate(Student student, string reason, bool additionalInfoNeeded, bool districtAudition)
	{
        this.student = student;
        this.reason = reason;

        if (additionalInfoNeeded)
        {
            this.auditionIds = new List<int>();
            setYearId();
            setAuditionIds(districtAudition);
        }
	}

    /*
     * Pre:  The student must have an entry in the DataStudentYearHistory table
     *       for the current year.
     * Post: The year id is set for the current student
     */
    private void setYearId()
    {
        //if (!reason.ToUpper().Equals("DUET"))
            yearId = DbInterfaceStudent.GetStudentYearId(student.id);
        //else
        //    yearId = -1;
    }

    /*
     * Pre:  
     * Post: The audition ids are set for the current student's auditions for the year
     * @param districtAudition signifies whether the current coordinate is being created for
     *        a district or state audition
     */
    private void setAuditionIds(bool districtAudition)
    {
        if (districtAudition)
            auditionIds = DbInterfaceStudentAudition.GetStudentDistrictAuditionIds(yearId);
        else
            auditionIds = DbInterfaceStudentAudition.GetStudentStateAuditionIds(yearId);
    }

    public void setAuditionIds(List<int> ids) { auditionIds = ids; }

    /*
     * Pre:  The reason must be "Duet"
     * Post: Sets the audition id and year id for a duet partner
     */
    public void setAuditionYearIds(int auditionId, int yearId)
    {
        this.yearId = yearId;
        auditionIds.Add(auditionId);
    }

    /*
     * Pre:
     * Post: Returns true if the input object has the same student and reason as the current one
     */
    public override bool Equals(object obj)
    {
        StudentCoordinate other = (StudentCoordinate)obj;
        return other.student.id == student.id && other.reason.Equals(reason);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
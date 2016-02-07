using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   Feburary 2013
 * This class represents a Student in the system.
 */ 
public class Student : Person
{
    public int districtId { get; private set; }
    public int legacyPoints { get; private set; }
    public int legacyPtsYear { get; private set; }
    public int currTeacherId { get; private set; }
    public string teacherName { get; set; }
    public int prevTeacherId { get; private set; }
    public string grade { get; set; }
    public string theoryLevel { get; set; }
    public int totalPoints { get; set; }

	public Student(int id, int districtId, int currTeacherId, int prevTeacherId) : base(id)
	{
        this.districtId = districtId;
        this.currTeacherId = currTeacherId;
        this.prevTeacherId = prevTeacherId;
        computeCurrentGrade();
        getYearsTheoryLevel();
	}

    public Student(int id, string firstName, string middleInitial, string lastName, int districtId, 
        int currTeacherId, int prevTeacherId) : base(id, firstName, middleInitial, lastName)
    {
        this.districtId = districtId;
        this.currTeacherId = currTeacherId;
        this.prevTeacherId = prevTeacherId;
        computeCurrentGrade();
        getYearsTheoryLevel();
    }

    public Student(int id, string firstName, string middleInitial, string lastName, string grade, 
                   int districtId, int currTeacherId, int prevTeacherId) 
                   : base(id, firstName, middleInitial, lastName)
    {
        this.districtId = districtId;
        this.currTeacherId = currTeacherId;
        this.prevTeacherId = prevTeacherId;
        this.grade = grade;
    }

    // Used for full data dump
    public Student(int id, string firstName, string middleInitial, string lastName, string grade,
        int districtId, string teacherName, string theoryLevel) : base(id, firstName, middleInitial, lastName)
    {
        this.grade = grade;
        this.districtId = districtId;
        this.teacherName = teacherName;
        this.theoryLevel = theoryLevel;
    }

    public void SetLegacyPoints(int points, int year)
    {
        legacyPoints = points;
        legacyPtsYear = year;
    }

    /*
     * Pre:
     * Post: The new student is added to the database
     */
    public void addToDatabase()
    {
        id = DbInterfaceStudent.AddNewStudent(this);
    }

    /*
     * Pre:  The student information must have been previously entered
     *       into the database, meaning that there is a unique id
     * Post: The student's information is updated
     * @returns true if the information is successfully updated and false otherwise
     */
    public bool editDatabaseInformation()
    {
        return DbInterfaceStudent.EditStudent(this);
    }

    /*
     * Pre:  The student information must have been previously entered
     *       into the database, meaning that there is a unique id
     * Post: The student's information is deleted
     * @returns true if the information is successfully deleted and false otherwise
     */
    public bool deleteDatabaseInformation()
    {
        return DbInterfaceStudent.DeleteStudent(id);
    }

    /*
     * Pre:  The districtId must exist in the system
     * Post: The name of the district with the current districtId
     *       is returned.
     */ 
    public string getDistrict()
    {
        return DbInterfaceStudent.GetStudentDistrict(districtId);
    }

    /*
     * Pre:  The currTeacherId must exist in the system.
     * Post: The name of the teacher with a teacher id of
     *       currTeacherId is returned.
     */ 
    public string getCurrTeacher()
    {
        return DbInterfaceContact.GetContactName(currTeacherId);
    }

    /*
     * Pre:
     * Post: If the student with the set id has already been registered for
     *       an audition in the current year or previous years, the current
     *       grade will be computed.  Otherwise the grade will be set to ""
     */ 
    private void computeCurrentGrade()
    {
        grade = DbInterfaceStudent.GetStudentGrade(id);
    }

    /*
     * Pre:
     * Post: The theory level for the student is retrieved if it has already
     *       been set for the current year
     */
    private void getYearsTheoryLevel()
    {
        theoryLevel = DbInterfaceStudent.GetTheoryLevel(id);
    }

    /*
     * Pre:
     * Post: Retrieves the total points awarded to the student
     */
    public int getTotalPoints()
    {
        totalPoints = DbInterfaceStudent.GetTotalPoints(id);

        return totalPoints;
    }
}
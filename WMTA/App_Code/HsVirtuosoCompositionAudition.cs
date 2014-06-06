using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   9/4/13
 * This class represents a HS Virtuoso or Composition audition in the system
 */
public class HsVirtuosoCompositionAudition
{
    public int auditionId { get; set; }
    public int year { get; private set; }
    public int points { get; set; }
    public string auditionType { get; private set; }
    public Student student { get; private set; }

    /*
     * Constructor to instantiate an existing audition
     */
	public HsVirtuosoCompositionAudition(int auditionId, Student student, int year, int points, string auditionType)
	{
        this.auditionId = auditionId;
        this.student = student;
        this.year = year;
        this.points = points;
        this.auditionType = auditionType;
	}

    /*
     * Constructor to create a new audition
     */
    public HsVirtuosoCompositionAudition(Student student, int year, string auditionType)
    {
        this.student = student;
        this.year = year;
        this.auditionType = auditionType;
        this.auditionId = -1;
        this.points = 0;
    }

    /*
     * Pre:
     * Post: Adds the new audition to the database and sets the audition's id
     */
    public bool addToDatabase()
    {
        return DbInterfaceStudentAudition.CreateStudentHsOrCompositionAudition(this);
    }

    /*
     * Pre:  The audition must already exist in the database
     * Post: The audition is updated with the current information
     */
    public bool updateInDatabase()
    {
        return DbInterfaceStudentAudition.UpdateStudentHsOrCompositionAudition(this);
    }
}
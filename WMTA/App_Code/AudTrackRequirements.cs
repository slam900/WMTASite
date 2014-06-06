using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   March 2013
 * This class holds the compositions requirements for a student
 * audition based on their grade and selected audition track
 */
public class AudTrackRequirements
{
    public string track { get; private set; }
    public int grade { get; private set; }
    public int requiredNumStyles { get; set; }
    public List<string> requiredStyles { get; set; }

    /*
     * Pre:  The entered grade must be between 1 and 12.
     *       The audition track must exist in the system.
     * Post: The required number of styles and required styles
     *       are retrieved based on the entered grade and 
     *       audition track
     * @param track is the audition track of the student for the audition
     * @param grade is the current grade of the student
     */
	public AudTrackRequirements(string track, int grade)
	{
        this.track = track;
        this.grade = grade;
        getRequirements();
	}

    /*
     * Pre:
     * Post: The requirements for the audition track and grade are
     *       retrieved from the database
     */
    private void getRequirements()
    {
        DbInterfaceStudentAudition.GetAuditionTrackRequirements(this);
    }
}
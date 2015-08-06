using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   07/10/13
 * This class represents an audition in the system. 
 */
public class Audition
{
    public int auditionId { get; set; }
    public int districtId { get; private set; }
    public int numJudges { get; private set; }
    public string venue { get; private set; }
    public string chairpersonId { get; private set; }
    public string theoryTestSeries { get; private set; }
    public DateTime auditionDate { get; private set; }
    public DateTime freezeDate { get; private set; }
    public TimeSpan startTimeSession1 { get; set; }
    public TimeSpan endTimeSession1 { get; set; }
    public TimeSpan startTimeSession2 { get; set; }
    public TimeSpan endTimeSession2 { get; set; }
    public TimeSpan startTimeSession3 { get; set; }
    public TimeSpan endTimeSession3 { get; set; }
    public TimeSpan startTimeSession4 { get; set; }
    public TimeSpan endTimeSession4 { get; set; }
    public string startTimeDisplaySession1 { get; set; }
    public string startTimeDisplaySession2 { get; set; }
    public string startTimeDisplaySession3 { get; set; }
    public string startTimeDisplaySession4 { get; set; }
    public string endTimeDisplaySession1 { get; set; }
    public string endTimeDisplaySession2 { get; set; }
    public string endTimeDisplaySession3 { get; set; }
    public string endTimeDisplaySession4 { get; set; }
    //public EventSchedule Schedule { get; set; }

    public bool duetsAllowed { get; private set; }  /*Used for State auditions only - should only have one Badger Keyboard site per year where duets are allowed */
    private ScheduleData scheduleData { get; set; }

    /* Constructor to instantiate audition as well as create it in the database */
	public Audition(int districtId, int numJudges, string venue, string chairpersonId, string theoryTestSeries,
        DateTime auditionDate, DateTime freezeDate, TimeSpan startTime1, TimeSpan endTime1, TimeSpan startTime2, 
        TimeSpan endTime2, TimeSpan startTime3, TimeSpan endTime3, TimeSpan startTime4, TimeSpan endTime4,
        string startTimeDisplaySession1, string startTimeDisplaySession2, string startTimeDisplaySession3,
        string startTimeDisplaySession4, string endTimeDisplaySession1, string endTimeDisplaySession2,
        string endTimeDisplaySession3, string endTimeDisplaySession4, bool duetsAllowed)
	{
        this.districtId = districtId;
        this.numJudges = numJudges;
        this.venue = venue;
        this.chairpersonId = chairpersonId;
        this.theoryTestSeries = theoryTestSeries;
        this.auditionDate = auditionDate;
        this.freezeDate = freezeDate;
        this.startTimeSession1 = startTime1;
        this.endTimeSession1 = endTime1;
        this.startTimeSession2 = startTime2;
        this.endTimeSession2 = endTime2;
        this.startTimeSession3 = startTime3;
        this.endTimeSession3 = endTime3;
        this.startTimeSession4 = startTime4;
        this.endTimeSession4 = endTime4;
        this.startTimeDisplaySession1 = startTimeDisplaySession1;
        this.startTimeDisplaySession2 = startTimeDisplaySession2;
        this.startTimeDisplaySession3 = startTimeDisplaySession3;
        this.startTimeDisplaySession4 = startTimeDisplaySession4;
        this.endTimeDisplaySession1 = endTimeDisplaySession1;
        this.endTimeDisplaySession2 = endTimeDisplaySession2;
        this.endTimeDisplaySession3 = endTimeDisplaySession3;
        this.endTimeDisplaySession4 = endTimeDisplaySession4;
        this.duetsAllowed = duetsAllowed;
        scheduleData = new ScheduleData();

        addNewAudition();
	}

    /* Constructor to instantiate existing audition */
    public Audition(int auditionId, int districtId, int numJudges, string venue, string chairpersonId, string theoryTestSeries,
        DateTime auditionDate, DateTime freezeDate, TimeSpan startTime1, TimeSpan endTime1, TimeSpan startTime2, TimeSpan endTime2,
        TimeSpan startTime3, TimeSpan endTime3, TimeSpan startTime4, TimeSpan endTime4, string startTimeDisplaySession1, 
        string startTimeDisplaySession2, string startTimeDisplaySession3, string startTimeDisplaySession4, string endTimeDisplaySession1, 
        string endTimeDisplaySession2, string endTimeDisplaySession3, string endTimeDisplaySession4, bool duetsAllowed)
    {
        this.auditionId = auditionId;
        this.districtId = districtId;
        this.numJudges = numJudges;
        this.venue = venue;
        this.chairpersonId = chairpersonId;
        this.theoryTestSeries = theoryTestSeries;
        this.auditionDate = auditionDate;
        this.freezeDate = freezeDate;
        this.startTimeSession1 = startTime1;
        this.endTimeSession1 = endTime1;
        this.startTimeSession2 = startTime2;
        this.endTimeSession2 = endTime2;
        this.startTimeSession3 = startTime3;
        this.endTimeSession3 = endTime3;
        this.startTimeSession4 = startTime4;
        this.endTimeSession4 = endTime4;
        this.startTimeDisplaySession1 = startTimeDisplaySession1;
        this.startTimeDisplaySession2 = startTimeDisplaySession2;
        this.startTimeDisplaySession3 = startTimeDisplaySession3;
        this.startTimeDisplaySession4 = startTimeDisplaySession4;
        this.endTimeDisplaySession1 = endTimeDisplaySession1;
        this.endTimeDisplaySession2 = endTimeDisplaySession2;
        this.endTimeDisplaySession3 = endTimeDisplaySession3;
        this.endTimeDisplaySession4 = endTimeDisplaySession4;
        this.duetsAllowed = duetsAllowed;
        scheduleData = new ScheduleData();
    }

    /* Constructor to instantiate existing audition without session times */
    public Audition(int auditionId, int districtId, int numJudges, string venue, string chairpersonId, string theoryTestSeries,
        DateTime auditionDate, DateTime freezeDate, bool duetsAllowed)
    {
        this.auditionId = auditionId;
        this.districtId = districtId;
        this.numJudges = numJudges;
        this.venue = venue;
        this.chairpersonId = chairpersonId;
        this.theoryTestSeries = theoryTestSeries;
        this.auditionDate = auditionDate;
        this.freezeDate = freezeDate;
        this.duetsAllowed = duetsAllowed;
        scheduleData = new ScheduleData();
    }

    /*
     * Pre:  All attributes must have values
     * Post: A new audition is added to the database 
     */
    private void addNewAudition()
    {
        DbInterfaceAudition.AddNewAudition(this);
    }

    /*
     * Pre:  The audition must already exist in the database
     * Post: The audition's information is edited in the database
     * @returns true if the update is successful and false otherwise
     */
    public bool updateInDatabase()
    {
        return DbInterfaceAudition.EditAudition(this);
    }

    /*
     * Pre:
     * Post: The audition's scheduling data is updated in the database
     */
    public bool SaveScheduleData()
    {
        return scheduleData.Save(auditionId);
    }

    /*
     * Pre:
     * Post: Returns the audition type based on the input district id.
     *       15 = State Keyboard
     *       16 = State Vocal/Instrumental
     *       All others = District
     * @returns the audition type
     */
    public string getAuditionType()
    {
        string type = "District";

        if (districtId == 15)
            type = "State Keyboard";
        else if (districtId == 16 || districtId == 20 || districtId == 21 || districtId == 22)
            type = "State Vocal/Instrumental";
        else if (districtId == 23)
            type = "HS Virtuoso";
        else if (districtId == 24)
            type = "Composition";

        return type;
    }

    /*
     * Pre:
     * Post: Returns the list of rooms for the audition
     * @param refresh is an optional parameter to force a refresh of the list of rooms
     * @returns the list of rooms
     */
    public List<string> GetRooms(bool refresh = false)
    {
        return scheduleData.GetRooms(auditionId, refresh);
    }

    /*
     * Pre:
     * Post: Returns the list of theory test rooms for the audition
     * @param refresh is an optional parameter to force a refresh of the list of rooms
     * @returns the list of rooms
     */
    public List<Tuple<string, string>> GetTheoryRooms(bool refresh = false)
    {
        return scheduleData.GetTheoryRooms(auditionId, refresh);
    }

    /*
     * Pre:
     * Post: Returns the list of judges for the audition's district
     * @param refresh is an optional parameter to force a refresh of the list of judges
     * @returns the list of judges
     */
    public List<Judge> GetAvailableJudges(bool refresh = false)
    {
        return scheduleData.GetAvailableJudges(auditionId, refresh);
    }

    /*
     * Pre:
     * Post: Returns the list of judges scheduled for the audition
     * @param refresh is an optional parameter to force a refresh of the list of judges
     * @returns the list of judges
     */
    public List<Judge> GetEventJudges(bool refresh = false)
    {
        return scheduleData.GetEventJudges(auditionId, refresh);
    }

    /*
     * Pre:
     * Post: Returns the list of judge room assignments scheduled for the audition
     * @param refresh is an optional parameter to force a refresh of the list of judge room assignments
     * @returns the list of judge room assignments
     */
    public List<JudgeRoomAssignment> GetEventJudgeRoomAssignments(bool refresh = false)
    {
        return scheduleData.GetEventJudgeRoomAssignments(auditionId, refresh);
    }

    /*
     * Pre:
     * Post: Add a new room to use for the audition
     */
    public void AddRoom(string room) 
    {
        scheduleData.AddRoom(room);
    }

    /*
     * Pre:
     * Post: Add a new room to use for theory tests at the audition
     */
    public void AddTheoryRoom(string test, string room)
    {
        scheduleData.AddTheoryRoom(test, room);
    }

    /*
     * Pre:
     * Post: Add a new judge to use for the audition
     */
    public void AddJudge(int contactId)
    {
        scheduleData.AddJudge(new Judge(contactId));
    }

    /*
     * Pre:
     * Post: Add a new judge/room assignment to the audition
     */
    public void AddJudgeRoom(int contactId, string room, List<Tuple<int, string>> times, int scheduleOrder)
    {
        scheduleData.AddJudgeRoom(new Judge(contactId), room, times, scheduleOrder);
    }

    /*
     * Pre:
     * Post: Removes an existing room for the audition
     */
    public void RemoveRoom(string room)
    {
        scheduleData.RemoveRoom(room);
    }

    /*
     * Pre:
     * Post: Removes an existing theory room for the audition
     */
    public void RemoveTheoryRoom(string test, string room)
    {
        scheduleData.RemoveTheoryRoom(test, room);
    }

    /*
     * Pre:
     * Post: Remove a judge from the audition
     */
    public void RemoveJudge(int contactId)
    {
        scheduleData.RemoveJudge(new Judge(contactId));
    }

    /*
     * Pre:
     * Post:Removes a judge/room assignment from the audition
     */
    public void RemoveJudgeRoom(int contactId, string room, List<Tuple<int, string>> times, int scheduleOrder)
    {
        scheduleData.RemoveJudgeRoom(new Judge(contactId), room, times, scheduleOrder);
    }
}
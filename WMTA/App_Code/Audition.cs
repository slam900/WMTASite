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
    public TimeSpan startTime { get; private set; }
    public TimeSpan endTime { get; private set; }
    public bool duetsAllowed { get; private set; }  /*Used for State auditions only - should only have one Badger Keyboard site per year where duets are allowed */
    private ScheduleData scheduleData { get; set; }

    /* Constructor to instantiate audition as well as create it in the database */
	public Audition(int districtId, int numJudges, string venue, string chairpersonId, 
                    string theoryTestSeries, DateTime auditionDate, DateTime freezeDate,
                    TimeSpan startTime, TimeSpan endTime, bool duetsAllowed)
	{
        this.districtId = districtId;
        this.numJudges = numJudges;
        this.venue = venue;
        this.chairpersonId = chairpersonId;
        this.theoryTestSeries = theoryTestSeries;
        this.auditionDate = auditionDate;
        this.freezeDate = freezeDate;
        this.startTime = startTime;
        this.endTime = endTime;
        this.duetsAllowed = duetsAllowed;
        scheduleData = new ScheduleData();

        addNewAudition();
	}

    /* Constructor to instantiate existing audition */
    public Audition(int auditionId, int districtId, int numJudges, string venue, 
                    string chairpersonId, string theoryTestSeries, DateTime auditionDate, 
                    DateTime freezeDate, TimeSpan startTime, TimeSpan endTime, bool duetsAllowed)
    {
        this.auditionId = auditionId;
        this.districtId = districtId;
        this.numJudges = numJudges;
        this.venue = venue;
        this.chairpersonId = chairpersonId;
        this.theoryTestSeries = theoryTestSeries;
        this.auditionDate = auditionDate;
        this.freezeDate = freezeDate;
        this.startTime = startTime;
        this.endTime = endTime;
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
     *       16 = State Non-Keyboard
     *       All others = District
     * @returns the audition type
     */
    public string getAuditionType()
    {
        string type = "District";

        if (districtId == 15)
            type = "State Keyboard";
        else if (districtId == 16 || districtId == 20 || districtId == 21 || districtId == 22)
            type = "State Non-Keyboard";
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
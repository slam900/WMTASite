using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Miller
 * Date:   10/15/2014
 * This class holds the data needed to prepare for scheduling an audition
 */
public class ScheduleData
{
    public List<string> rooms { get; set; }
    public List<Tuple<string, string>> theoryRooms { get; set; } //item 1 = test, item 2 = room
    public List<Judge> availableJudges { get; set; }
    public List<Judge> scheduledJudges { get; set; }
    public List<JudgeRoomAssignment> judgeRooms { get; set; }

    // Items that have been removed from schedule
    public List<string> roomsToRemove { get; set; }
    public List<Tuple<string, string>> theoryRoomsToRemove { get; set; }
    public List<Judge> scheduledJudgesToRemove { get; set; }
    public List<JudgeRoomAssignment> judgeRoomsToRemove { get; set; }

    public ScheduleData()
    {
        rooms = new List<string>();
        theoryRooms = new List<Tuple<string, string>>();
        availableJudges = new List<Judge>();
        scheduledJudges = new List<Judge>();
        judgeRooms = new List<JudgeRoomAssignment>();
        roomsToRemove = new List<string>();
        theoryRoomsToRemove = new List<Tuple<string, string>>();
        scheduledJudgesToRemove = new List<Judge>();
        judgeRoomsToRemove = new List<JudgeRoomAssignment>();
    }

    /*
     * Pre:
     * Post: The scheduling data is updated in the database for the input audition id
     */
    public bool Save(int auditionId)
    {
        bool success = true;

        // Delete items in delete lists
        foreach (string room in roomsToRemove)
            success = success && DbInterfaceScheduling.DeleteRoom(auditionId, room);
        foreach (Tuple<string, string> theoryRoom in theoryRoomsToRemove)
            success = success && DbInterfaceScheduling.DeleteTheoryRoom(auditionId, theoryRoom.Item1, theoryRoom.Item2);
        foreach (Judge judge in scheduledJudgesToRemove)
            success = success && DbInterfaceScheduling.DeleteJudge(auditionId, judge.id);

        // Update
        foreach (string room in rooms)
            success = success && DbInterfaceScheduling.AddRoom(auditionId, room);
        foreach (Tuple<string, string> theoryRoom in theoryRooms)
            success = success && DbInterfaceScheduling.AddTheoryRoom(auditionId, theoryRoom.Item1, theoryRoom.Item2);
        foreach (Judge judge in scheduledJudges)
            success = success && DbInterfaceScheduling.AddJudge(auditionId, judge.id, -1);

        // TODO - still need to figure out if room assignment object needs to be saved

        return success;
    }

    /*
     * Pre:
     * Post: Returns the list of rooms for the audition
     * @param refresh is an optional parameter to force a refresh of the list of rooms
     * @returns the list of rooms
     */
    public List<string> GetRooms(int auditionId, bool refresh)
    {
        if (rooms == null || refresh)
        {
            rooms = DbInterfaceScheduling.GetAuditionRooms(auditionId);
        }

        return rooms;
    }

    /*
     * Pre:
     * Post: Returns the list of theory test rooms for the audition
     * @param refresh is an optional parameter to force a refresh of the list of rooms
     * @returns the list of rooms
     */
    public List<Tuple<string, string>> GetTheoryRooms(int auditionId, bool refresh)
    {
        if (theoryRooms == null || refresh)
        {
            theoryRooms = DbInterfaceScheduling.GetAuditionTheoryRooms(auditionId);
        }

        return theoryRooms;
    }

    /*
     * Pre:
     * Post: Returns the list of judges for the audition's district
     * @param refresh is an optional parameter to force a refresh of the list of judges
     * @returns the list of judges
     */
    public List<Judge> GetAvailableJudges(int auditionId, bool refresh)
    {
        if (availableJudges == null || refresh)
        {
            availableJudges = DbInterfaceScheduling.GetDistrictJudges(auditionId);
        }

        return availableJudges;
    }

    /*
     * Pre:
     * Post: Returns the list of judges scheduled for the audition
     * @param refresh is an optional parameter to force a refresh of the list of judges
     * @returns the list of judges
     */
    public List<Judge> GetEventJudges(int auditionId, bool refresh)
    {
        if (scheduledJudges == null || refresh)
        {
            scheduledJudges = DbInterfaceScheduling.GetAuditionJudges(auditionId);
        }

        return scheduledJudges;
    }

    /*
     * Pre:
     * Post: Add a new room for scheduling
     */
    public void AddRoom(string room)
    {
        if (!rooms.Contains(room))
        {
            rooms.Add(room);
        }
    }

    /*
     * Pre:
     * Post: Assign a theory test to a room
     */
    public void AddTheoryRoom(string test, string room)
    {
        Tuple<string, string> theoryRoom = new Tuple<string, string>(test, room);

        if (!theoryRooms.Contains(theoryRoom))
        {
            theoryRooms.Add(theoryRoom);
        }
    }

    /*
     * Pre:
     * Post: Add a judge to the audition
     */
    public void AddJudge(Judge judge)
    {
        if (!scheduledJudges.Contains(judge))
        {
            scheduledJudges.Add(judge);
        }
    }

    /*
     * Pre:
     * Post: Add a judge/room assignment to the audition
     */
    public void AddJudgeRoom(Judge judge, string room, List<Tuple<int, string>> times, int scheduleOrder)
    {
        JudgeRoomAssignment assignment = new JudgeRoomAssignment(judge, room, times, scheduleOrder);

        if (!judgeRooms.Contains(assignment))
        {
            judgeRooms.Add(assignment);
        }
        else // Update, if it already exists
        {
            judgeRooms.Remove(assignment);
            judgeRooms.Add(assignment);
        }
    }

    /*
     * Pre:
     * Post: Removes the room from the list of active rooms
     *       and adds it to the list of rooms to remove
     */
    public void RemoveRoom(string room)
    {
        rooms.Remove(room);
        roomsToRemove.Add(room);
    }

    /*
     * Pre:
     * Post: Removes the theory room from the list of active rooms
     *       and adds it to the list of theory rooms to remove
     */
    public void RemoveTheoryRoom(string test, string room)
    {
        Tuple<string, string> testRoom = new Tuple<string, string>(test, room);
        theoryRooms.Remove(testRoom);
        theoryRoomsToRemove.Add(testRoom);
    }

    /*
     * Pre:
     * Post: Removes the judge from the list of active judges
     *       and adds them to the list of judges to remove
     */
    public void RemoveJudge(Judge judge)
    {
        scheduledJudges.Remove(judge);
        scheduledJudgesToRemove.Add(judge);
    }

    /*
     * Pre:
     * Post: Remove a judge/room assignment from the audition
     */
    public void RemoveJudgeRoom(Judge judge, string room, List<Tuple<int, string>> times, int scheduleOrder)
    {
        JudgeRoomAssignment assignment = new JudgeRoomAssignment(judge, room, times, scheduleOrder);

        judgeRooms.Remove(assignment);
        judgeRoomsToRemove.Add(assignment);
    }
}
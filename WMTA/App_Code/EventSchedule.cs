using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Miller
 * Date:   12/20/14
 * This class represents a schedule for an event
 */
public class EventSchedule
{
    public List<ScheduleSlot> ScheduleSlots { get; set; }

    public EventSchedule()
    {
        ScheduleSlots = new List<ScheduleSlot>();
    }

    public void Add(int auditionId, int judgeId, string judgeName, int minutes, TimeSpan startTime, 
        string timePref, string grade, string audType, string audTrack, string studentName, int studentId,
        string instrument)
    {
        ScheduleSlot slot = new ScheduleSlot(auditionId, judgeId, judgeName, minutes, startTime, timePref, 
            grade, audType, audTrack, studentName, studentId, instrument);

        ScheduleSlots.Add(slot);
    }

    /*
     * Pre:  All audition schedule slots must exist in the database
     * Post: Commit the updated schedule to the database
     */
    public bool UpdateSchedule()
    {
        return DbInterfaceScheduling.UpdateSchedule(this);
    }

    public void MoveAudition(int idToSwitch, int idToSwitchWith, Audition audition)
    {
        ScheduleSlot auditionToSwitch = ScheduleSlots.Where(s => s.AuditionId == idToSwitch).FirstOrDefault();
        ScheduleSlot auditionToSwitchWith = ScheduleSlots.Where(s => s.AuditionId == idToSwitchWith).FirstOrDefault();

        if (auditionToSwitch != null && auditionToSwitchWith != null)
        {

            if (auditionToSwitch.JudgeId == auditionToSwitchWith.JudgeId && auditionToSwitch.StartTime < auditionToSwitchWith.StartTime) // Same judge, shifting later
            {
                MoveAuditionLaterWithSameJudge(auditionToSwitch, auditionToSwitchWith, audition);
            }
            else if (auditionToSwitch.JudgeId == auditionToSwitchWith.JudgeId && auditionToSwitch.StartTime > auditionToSwitchWith.StartTime) // Same judge, shifting earlier
            {
                MoveAuditionEarlierWithSameJudge(auditionToSwitch, auditionToSwitchWith, audition);
            }
            else if (auditionToSwitch.JudgeId != auditionToSwitchWith.JudgeId)
            {
                MoveAuditionWithDifferentJudge(auditionToSwitch, auditionToSwitchWith, audition);
            }
        }
    }

    /*
     * Pre:
     * Post: Move the first audition to the slot of the second.  To do this, move all slots from after auditionToSwitch
     *       to auditionToSwitch with one slot earlier and insert auditionToSwitch into the spot created, previously
     *       occupied by auditionToSwitchWith. 
     */
    private void MoveAuditionLaterWithSameJudge(ScheduleSlot auditionToSwitch, ScheduleSlot auditionToSwitchWith, Audition audition)
    {
        TimeSpan newTime = auditionToSwitchWith.StartTime;

        // Shift auditions to make room for switch
        ShiftAuditionsEarlier(auditionToSwitch.StartTime.Add(TimeSpan.FromMinutes(1)), auditionToSwitchWith.StartTime, auditionToSwitch.Minutes, audition, auditionToSwitchWith.JudgeId);

        // move audition to it's new spot
        ScheduleSlots.Where(s => s.AuditionId == auditionToSwitch.AuditionId).FirstOrDefault().StartTime = newTime;
    }

    private void MoveAuditionEarlierWithSameJudge(ScheduleSlot auditionToSwitch, ScheduleSlot auditionToSwitchWith, Audition audition)
    {
        TimeSpan newTime = auditionToSwitchWith.StartTime;

        //Shift auditions to make room for switch, if needed.  Otherwise just change the time
        int session = GetNewAuditionSession(audition, auditionToSwitchWith.StartTime, auditionToSwitchWith.StartTime.Add(TimeSpan.FromMinutes(auditionToSwitchWith.Minutes)));
        if (FitsInNewSessionWithoutShift(audition, session, auditionToSwitch.Minutes, auditionToSwitchWith.JudgeId))
            newTime = auditionToSwitchWith.StartTime.Add(TimeSpan.FromMinutes(auditionToSwitchWith.Minutes));
        else
            ShiftAuditionsLater(auditionToSwitchWith.StartTime, auditionToSwitch.StartTime.Subtract(TimeSpan.FromMinutes(1)), auditionToSwitch.Minutes, audition, auditionToSwitchWith.JudgeId);

        //move audition to it's new spot
        ScheduleSlots.Where(s => s.AuditionId == auditionToSwitch.AuditionId).FirstOrDefault().StartTime = newTime;
    }

    private void MoveAuditionWithDifferentJudge(ScheduleSlot auditionToSwitch, ScheduleSlot auditionToSwitchWith, Audition audition)
    {
        TimeSpan newTime = auditionToSwitchWith.StartTime;

        // Shift all auditions in session/judge of slot being moved up to fill gap
        int session = GetNewAuditionSession(audition, auditionToSwitch.StartTime, auditionToSwitch.StartTime.Add(TimeSpan.FromMinutes(auditionToSwitch.Minutes)));
        TimeSpan endTime = audition.endTimeSession1;
        if (session == 2)
            endTime = audition.endTimeSession2;
        else if (session == 3)
            endTime = audition.endTimeSession3;
        else if (session == 4)
            endTime = audition.endTimeSession4;

        ShiftAuditionsEarlier(auditionToSwitch.StartTime, endTime, auditionToSwitch.Minutes, auditionToSwitch.JudgeId);        

        // If there is room left in the session, just put it in there w/o shifting
        session = GetNewAuditionSession(audition, auditionToSwitchWith.StartTime, auditionToSwitchWith.StartTime.Add(TimeSpan.FromMinutes(auditionToSwitchWith.Minutes)));
        if (FitsInNewSessionWithoutShift(audition, session, auditionToSwitch.Minutes, auditionToSwitchWith.JudgeId))
            newTime = auditionToSwitchWith.StartTime.Add(TimeSpan.FromMinutes(auditionToSwitchWith.Minutes));
        // Shift all auditions in session/judge of slot to switch with with time greater than or equal to the audition later
        else
            ShiftAuditionsLater(auditionToSwitchWith.StartTime, TimeSpan.MaxValue, auditionToSwitch.Minutes, audition, auditionToSwitchWith.JudgeId);
            //ShiftAuditionsLater(auditionToSwitchWith.StartTime, TimeSpan.MaxValue, auditionToSwitch.Minutes, auditionToSwitchWith.JudgeId);

        // Change audition time and judge
        ScheduleSlots.Where(s => s.AuditionId == auditionToSwitch.AuditionId).First().StartTime = newTime;
        ScheduleSlots.Where(s => s.AuditionId == auditionToSwitch.AuditionId).First().JudgeId = auditionToSwitchWith.JudgeId;
        ScheduleSlots.Where(s => s.AuditionId == auditionToSwitch.AuditionId).First().JudgeName = auditionToSwitchWith.JudgeName;
    }

    /*
     * Pre:
     * Post: Shift each audition from startTime to endTime the input number of minutes earlier
     */
    private void ShiftAuditionsEarlier(TimeSpan startTime, TimeSpan endTime, int numberMinutesToShift, Audition audition, int judgeId)
    {
        List<ScheduleSlot> slotsToConsider = ScheduleSlots.Where(s => s.JudgeId == judgeId).ToList();
        bool doneShifting = false;

        for (int i = 0; !doneShifting && i < slotsToConsider.Count(); i++)
        {
            ScheduleSlot slot = slotsToConsider.ElementAt(i);

            // Check if we're done before changing the audition's start time so we don't shift more than we should
            if (slot.StartTime >= endTime) doneShifting = true;

            if (slot.StartTime >= startTime && slot.StartTime <= endTime && !DuringBreak(audition, slot.StartTime.Subtract(TimeSpan.FromMinutes(numberMinutesToShift)), slot.Minutes))
            {
                ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).FirstOrDefault().StartTime = slot.StartTime.Subtract(TimeSpan.FromMinutes(numberMinutesToShift));
            }
            else if (slot.StartTime >= startTime && slot.StartTime <= endTime)
            {
                // The shift will move the audition into the break, so leave it where it is and shift other auditions out
                int newSession = GetNewAuditionSession(audition, slot.StartTime, slot.StartTime.Add(TimeSpan.FromMinutes(slot.Minutes))); 
              
                // If new session is 4, shift any audition's after it out by the length of the audition being moved
                if (newSession == 4)
                {
                    ShiftAuditionsLater(endTime, TimeSpan.MaxValue, numberMinutesToShift, judgeId);
                }
                // If session is not 4, need to shift each session out by the number of minutes of the last audition of the previous session
                else if (newSession == 3)
                {
                    // Get last session 3 audition slot
                    ScheduleSlot lastSessionSlot = GetLastSessionAudition(audition.endTimeSession3, judgeId);

                    //Shift session 4 auditions later by # of minutes of last session 3 audition
                    ShiftAuditionsLater(audition.startTimeSession4, TimeSpan.MaxValue, lastSessionSlot.Minutes, judgeId);

                    //Move last session 3 to first session 4 slot
                    ScheduleSlots.Where(s => s.AuditionId == lastSessionSlot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession4;

                    //Shift session 3 as needed to insert new audition
                    ShiftAuditionsLater(endTime, audition.endTimeSession3, numberMinutesToShift, judgeId);
                }
                else if (newSession == 2) 
                {
                    // Get last session 3 audition slot
                    ScheduleSlot lastSessionSlot = GetLastSessionAudition(audition.endTimeSession3, judgeId);

                    //Shift session 4 auditions later by # of minutes of last session 3 audition
                    ShiftAuditionsLater(audition.startTimeSession4, TimeSpan.MaxValue, lastSessionSlot.Minutes, judgeId);

                    //Move last session 3 to first session 4 slot
                    ScheduleSlots.Where(s => s.AuditionId == lastSessionSlot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession4;

                    //Shfit session 3 auditions later by # of minutes of last session 2 audition
                    lastSessionSlot = GetLastSessionAudition(audition.endTimeSession2, judgeId);
                    ShiftAuditionsLater(audition.startTimeSession3, audition.endTimeSession3, lastSessionSlot.Minutes, judgeId);

                    //Move last session 2 to first session 3 slot
                    ScheduleSlots.Where(s => s.AuditionId == lastSessionSlot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession3;

                    //Shift session 2 as needed to insert new audition
                    ShiftAuditionsLater(endTime, audition.endTimeSession2, numberMinutesToShift, judgeId);
                }
            }
        }
    }

    private void ShiftAuditionsLater(TimeSpan startTime, TimeSpan endTime, int numberMinutesToShift, Audition audition, int judgeId)
    {
        List<ScheduleSlot> slotsToConsider = ScheduleSlots.Where(s => s.JudgeId == judgeId).ToList();
        bool doneShifting = false;

        for (int i = 0; !doneShifting && i < slotsToConsider.Count(); i++)
        {
            ScheduleSlot slot = slotsToConsider.ElementAt(i);

            // Check if we're done before changing the audition's start time so we don't shift more than we should
            if (slot.StartTime >= endTime) doneShifting = true;

            if (slot.StartTime >= startTime && slot.StartTime <= endTime && !DuringBreak(audition, slot.StartTime.Add(TimeSpan.FromMinutes(numberMinutesToShift)), slot.Minutes))
            {
                ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).FirstOrDefault().StartTime = slot.StartTime.Add(TimeSpan.FromMinutes(numberMinutesToShift));
            }
            else if (slot.StartTime >= startTime && slot.StartTime <= endTime)
            {
                // The shift will move the audition into the break, so leave it where it is and shift other auditions out
                int newSession = GetNewAuditionSession(audition, slot.StartTime.Add(TimeSpan.FromMinutes(numberMinutesToShift)), slot.StartTime.Add(TimeSpan.FromMinutes(numberMinutesToShift + slot.Minutes)));

                // If new session is 4, shift any audition's after it out by the length of the audition being moved
                if (newSession == 4)
                {
                    ShiftAuditionsLater(audition.startTimeSession4, TimeSpan.MaxValue, slot.Minutes, judgeId);
                    ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession4;
                }
                // If session is not 4, need to shift each session out by the number of minutes of the last audition of the previous session
                else if (newSession == 3)
                {
                    // Get last session 3 audition slot
                    ScheduleSlot lastSessionSlot = GetLastSessionAudition(audition.endTimeSession3, judgeId);

                    //Shift session 4 auditions later by # of minutes of last session 3 audition
                    ShiftAuditionsLater(audition.startTimeSession4, TimeSpan.MaxValue, lastSessionSlot.Minutes, judgeId);

                    //Move last session 3 to first session 4 slot
                    ScheduleSlots.Where(s => s.AuditionId == lastSessionSlot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession4;

                    //Shift session 3 as needed to insert new audition - new - edited first parameter from endTime
                    ShiftAuditionsLater(audition.startTimeSession3, audition.endTimeSession3, slot.Minutes, judgeId);
                    // New - insert current slot into first session 3 spot
                    ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession3;

                }
                else if (newSession == 2) 
                {
                    // Get last session 3 audition slot
                    ScheduleSlot lastSessionSlot = GetLastSessionAudition(audition.endTimeSession3, judgeId);

                    //Shift session 4 auditions later by # of minutes of last session 3 audition
                    ShiftAuditionsLater(audition.startTimeSession4, TimeSpan.MaxValue, lastSessionSlot.Minutes, judgeId);

                    //Move last session 3 to first session 4 slot
                    ScheduleSlots.Where(s => s.AuditionId == lastSessionSlot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession4;

                    //Shfit session 3 auditions later by # of minutes of last session 2 audition
                    lastSessionSlot = GetLastSessionAudition(audition.endTimeSession2, judgeId);
                    ShiftAuditionsLater(audition.startTimeSession3, audition.endTimeSession3, lastSessionSlot.Minutes, judgeId);

                    //Move last session 2 to first session 3 slot
                    ScheduleSlots.Where(s => s.AuditionId == lastSessionSlot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession3;

                    //Shift session 2 as needed to insert new audition - new - edited first parameter from endTime
                    ShiftAuditionsLater(audition.startTimeSession2, audition.endTimeSession2, slot.Minutes, judgeId);

                    // New - insert current slot into first session 2 spot
                    ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).FirstOrDefault().StartTime = audition.startTimeSession2;
                }
            }
        }
    }

    /*
     * Pre:
     * Post: Shift each audition from startTime to endTime the input number of minutes earlier
     */
    private void ShiftAuditionsEarlier(TimeSpan startTime, TimeSpan endTime, int numberMinutesToShift, int judgeId)
    {
        List<ScheduleSlot> relevantSlots = ScheduleSlots.Where(s => s.JudgeId == judgeId && s.StartTime >= startTime && s.StartTime <= endTime).ToList();

        for (int i = 0; i < relevantSlots.Count(); i++)
        {
            ScheduleSlot slot = relevantSlots.ElementAt(i);
            ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).First().StartTime = slot.StartTime.Subtract(TimeSpan.FromMinutes(numberMinutesToShift));
        }
    }

    /*
     * Pre:
     * Post: Shift each audition from startTime to endTime the input number of minutes later
     */
    private void ShiftAuditionsLater(TimeSpan startTime, TimeSpan endTime, int numberMinutesToShift, int judgeId)
    {
        List<ScheduleSlot> relevantSlots = ScheduleSlots.Where(s => s.JudgeId == judgeId && s.StartTime >= startTime && s.StartTime <= endTime).ToList();

        for (int i = 0; i < relevantSlots.Count(); i++)
        {
            ScheduleSlot slot = relevantSlots.ElementAt(i);
            ScheduleSlots.Where(s => s.AuditionId == slot.AuditionId).First().StartTime = slot.StartTime.Add(TimeSpan.FromMinutes(numberMinutesToShift));
        }
    }

    /*
     * Pre:
     * Post: Determines whether or not the given audition start time and length would cause the audition to be during one of the event's breaks
     */
    private bool DuringBreak(Audition audition, TimeSpan startTime, int length)
    {
        TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(length));

        bool inFirstBreak = (startTime >= audition.endTimeSession1 && startTime < audition.startTimeSession2) || (endTime > audition.endTimeSession1 && endTime <= audition.startTimeSession2);
        bool inSecondBreak = (startTime >= audition.endTimeSession2 && startTime < audition.startTimeSession3) || (endTime > audition.endTimeSession2 && endTime <= audition.startTimeSession3);
        bool inThirdBreak = (startTime >= audition.endTimeSession3 && startTime < audition.startTimeSession4) || (endTime > audition.endTimeSession3 && endTime <= audition.startTimeSession4);

        return inFirstBreak || inSecondBreak || inThirdBreak;
    }

    /*
     * Pre:
     * Post: Determines the session number that the audition will be moved to
     */
    private int GetNewAuditionSession(Audition audition, TimeSpan newStartTime, TimeSpan newEndTime)
    {
        int session = -1;

        if (newStartTime >= audition.endTimeSession3 || newEndTime >= audition.endTimeSession3)
            session = 4;
        else if ((newStartTime >= audition.endTimeSession2 && newStartTime <= audition.endTimeSession3) || (newEndTime >= audition.endTimeSession2 && newEndTime <= audition.endTimeSession3))
            session = 3;
        else if ((newStartTime >= audition.endTimeSession1 && newStartTime <= audition.endTimeSession2) || (newEndTime >= audition.endTimeSession1 && newEndTime <= audition.endTimeSession2))
            session = 2;
        else if ((newStartTime >= audition.startTimeSession1 && newStartTime <= audition.endTimeSession1) || (newEndTime >= audition.startTimeSession1 && newEndTime <= audition.endTimeSession1))
            session = 1;

        return session;
    }

    /*
     * Pre:
     * Post: Returns the last audition that occurs before the input session end time
     */
    private ScheduleSlot GetLastSessionAudition(TimeSpan endTimeOfSession, int judgeId)
    {
        TimeSpan timeClosestToSessionEnd = TimeSpan.MinValue;
        ScheduleSlot lastSlot = null;

        foreach (ScheduleSlot slot in ScheduleSlots.Where(s => s.JudgeId == judgeId))
        {
            if (slot.StartTime < endTimeOfSession && slot.StartTime > timeClosestToSessionEnd)
            {
                lastSlot = slot;
                timeClosestToSessionEnd = slot.StartTime;
            }
        }

        return lastSlot;
    }

    /*
     * Pre:
     * Post: Determine whether or not an audition with the input length will fit at the end of a session
     */
    private bool FitsInNewSessionWithoutShift(Audition audition, int newSession, int auditionLength, int judgeId)
    {
        TimeSpan sessionEndTime = audition.endTimeSession1;
        bool fits = false;

        if (newSession == 2)
            sessionEndTime = audition.endTimeSession2;
        else if (newSession == 3)
            sessionEndTime = audition.endTimeSession3;
        else if (newSession == 4)
            sessionEndTime = audition.endTimeSession4;

        // Get current last audition in the session
        ScheduleSlot lastSlot = GetLastSessionAudition(sessionEndTime, judgeId);
        if (lastSlot != null)
        {
            fits = lastSlot.StartTime.Add(TimeSpan.FromMinutes(lastSlot.Minutes + auditionLength)) <= sessionEndTime;
        }

        return fits;
    }
}
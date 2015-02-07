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

    public void Add(int auditionId, int judgeId, string judgeName, int minutes, TimeSpan startTime)
    {
        ScheduleSlot slot = new ScheduleSlot(auditionId, judgeId, judgeName, minutes, startTime);

        ScheduleSlots.Add(slot);
    }

    public void MoveAudition(int idToSwitch, int idToSwitchWith, int judgeId)
    {
        ScheduleSlot auditionToSwitch = ScheduleSlots.Where(s => s.AuditionId == idToSwitch).FirstOrDefault();
        ScheduleSlot auditionToSwitchWith = ScheduleSlots.Where(s => s.AuditionId == idToSwitchWith).FirstOrDefault();

        if (auditionToSwitch != null && auditionToSwitchWith != null)
        {
            // If the judge is the same and the time of auditionToSwitch is earlier than auditionToSwitchWith, do first algorithm
            if (auditionToSwitch.JudgeId == judgeId && auditionToSwitch.StartTime < auditionToSwitchWith.StartTime)
            {
                MoveAuditionLaterWithSameJudge(auditionToSwitch, auditionToSwitchWith);
            }
            else if (auditionToSwitchWith.JudgeId == judgeId && auditionToSwitch.StartTime > auditionToSwitchWith.StartTime)
            {

            }

            //cases where judges are different



        }
    }

    /*
     * Pre:
     * Post: Move the first audition to the slot of the second.  To do this, move all slots from after auditionToSwitch
     *       to auditionToSwitch with one slot earlier and insert auditionToSwitch into the spot created, previously
     *       occupied by auditionToSwitchWith. 
     */
    private void MoveAuditionLaterWithSameJudge(ScheduleSlot auditionToSwitch, ScheduleSlot auditionToSwitchWith)
    {
        ShiftAuditionsEarlier(auditionToSwitch.StartTime, auditionToSwitchWith.StartTime, auditionToSwitch.Minutes);

        // move audition to it's new spot
        ScheduleSlots.Where(s => s.AuditionId == auditionToSwitch.AuditionId).FirstOrDefault().StartTime = auditionToSwitchWith.StartTime;

        // TODO - need to take breaks in the schedule into account
    }

    /*
     * Pre:
     * Post: Shift each audition from startTime to endTime the input number of minutes earlier
     */
    private void ShiftAuditionsEarlier(TimeSpan startTime, TimeSpan endTime, int numberMinutesToShift)
    {
        bool doneShifting = false;

        for (int i = 0; !doneShifting && i < ScheduleSlots.Count(); i++)
        {
            ScheduleSlot slot = ScheduleSlots.ElementAt(i);

            // Check if we're done before changing the audition's start time so we don't shift more than we should
            if (slot.StartTime >= endTime) doneShifting = true;

            if (slot.StartTime >= startTime && slot.StartTime <= endTime)
            {
                slot.StartTime = slot.StartTime.Subtract(TimeSpan.FromMinutes(numberMinutesToShift));
            }
        }
    }

    /*
     * Pre:
     * Post: Shift each audition from startTime to endTime the input number of minutes later
     */
    private void ShiftAuditionsLater(TimeSpan startTime, TimeSpan endTime, int numberMinutesToShift)
    {
        bool doneShifting = false;

        for (int i = 0; !doneShifting && i < ScheduleSlots.Count(); i++)
        {
            ScheduleSlot slot = ScheduleSlots.ElementAt(i);

            // Check if we're done before changing the audition's start time so we don't shift more than we should
            if (slot.StartTime >= endTime) doneShifting = true;

            if (slot.StartTime >= startTime && slot.StartTime <= endTime)
            {
                slot.StartTime = slot.StartTime.Add(TimeSpan.FromMinutes(numberMinutesToShift));
            }
        }
    }
}
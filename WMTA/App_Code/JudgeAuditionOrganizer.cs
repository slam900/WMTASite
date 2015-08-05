using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AuditionSlot
{
    public int Slot { get; set; }
    public int AuditionId { get; set; }
    public int Length { get; set; }
    public bool IsDuet { get; set; }
    public TimeSpan StartTime { get; set; }
}

public class TimeSlot
{
    public int Order { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}


public class JudgeAuditionOrganizer
{
    public Dictionary<int, List<AuditionSlot>> JudgeSlots { get; set; }  // Map of judge id to list of ordered auditions
    public Dictionary<int, List<TimeSlot>> JudgeTimeSlots { get; set; }  // Map of judge id to available times
    public List<TimeSlot> AuditionTimeSlots { get; set; }

    public JudgeAuditionOrganizer(Dictionary<int, List<AuditionSlot>> judgeSlots, int auditionOrgId)
    {
        JudgeSlots = judgeSlots;
        JudgeTimeSlots = DbInterfaceJudge.LoadAuditionJudgesTimeSlots(auditionOrgId);
        AuditionTimeSlots = DbInterfaceAudition.GetAuditionTimeSlots(auditionOrgId);
    }

    /*
     * Assign times to the auditions.  Use judge overbooked to pass back whether or not any judges had to be over-scheduled
     * Return whether the scheduling was successful
     */
    public bool SetTimes(bool judgeOverbooked = false)
    {
        bool success = true;

        foreach (int judgeId in JudgeSlots.Keys)
            success = success && SetJudgeTimes(judgeId, judgeOverbooked);

        return success && SaveTimes();
    }

    /*
     * Assign times to each of the judge's audition slots.
     * Start with the earliest available time and continue adding to that session until it is full
     * When that fills up move on to the next one and decrease the audition slot index to attempt to reschedule the audition that didn't fit in the previous session
     * Return whether all auditions fit with the judge
     */
    private bool SetJudgeTimes(int judgeId, bool judgeOverbooked)
    {
        List<AuditionSlot> judgeSlots = JudgeSlots[judgeId];
        List<TimeSlot> judgeTimes = JudgeTimeSlots[judgeId].OrderBy(s => s.Order).ToList();
        bool roomAvailable = true, allowForDuet = true;
        TimeSpan nextStartTime = judgeTimes[0].StartTime;
        int currentJudgeTime = 0;

        for (int i = 0; roomAvailable && i < judgeSlots.Count; i++)
        {
            AuditionSlot slot = judgeSlots[i];

            if (nextStartTime.Add(TimeSpan.FromMinutes(slot.Length)) <= judgeTimes[currentJudgeTime].EndTime || judgeTimes[currentJudgeTime].EndTime == AuditionTimeSlots[AuditionTimeSlots.Count - 1].EndTime) // Allow a judge's last session to be overbooked
            {
                slot.StartTime = nextStartTime;

                // Allow judges to be overbooked in the last slot
                if (nextStartTime.Add(TimeSpan.FromMinutes(slot.Length)) > judgeTimes[currentJudgeTime].EndTime)
                    judgeOverbooked = true;

                if (!slot.IsDuet || (slot.IsDuet && !allowForDuet))
                {
                    nextStartTime = nextStartTime.Add(TimeSpan.FromMinutes(slot.Length));
                    allowForDuet = true;
                }
                else if (slot.IsDuet) // If we have a duet and the duet partner hasn't gone through yet, don't increase the time
                    allowForDuet = false;
            }
            else // Move to the next session and try scheduling the audition again
            {
                currentJudgeTime++;
                i--;

                // Stop looping if the judge is out of room
                if (currentJudgeTime >= judgeTimes.Count)
                    roomAvailable = false;
                // Set the start time to the beginning of the new session
                else
                    nextStartTime = judgeTimes[currentJudgeTime].StartTime;
            }
        }

        return roomAvailable; 
    }

    /*
     * Pre:
     * Post: Save the new times to the temporary schedule table
     */
    private bool SaveTimes()
    {
        foreach (int judgeId in JudgeSlots.Keys)
        {
            foreach (AuditionSlot slot in JudgeSlots[judgeId])
            {
                // Update database
            }
        }
    }
}
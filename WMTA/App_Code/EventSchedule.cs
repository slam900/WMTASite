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

    public void Add(int judgeId, string judgeName, int minutes, TimeSpan startTime)
    {
        ScheduleSlot slot = new ScheduleSlot(judgeId, judgeName, minutes, startTime);

        ScheduleSlots.Add(slot);
    }
}
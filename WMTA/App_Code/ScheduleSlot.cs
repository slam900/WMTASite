using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ScheduleSlot
{
    public int AuditionId { get; set; }
    public int JudgeId { get; set; }
    public string JudgeName { get; set; } // If need more judge data can change this to a Judge object
    public int Minutes { get; set; }
    public TimeSpan StartTime { get; set; }
    public string StudentName { get; set; } // optional

    public ScheduleSlot(int auditionId, int judgeId, string judgeName, int minutes, TimeSpan startTime)
    {
        AuditionId = auditionId;
        JudgeId = judgeId;
        JudgeName = judgeName;
        Minutes = minutes;
        StartTime = startTime;
    }
}
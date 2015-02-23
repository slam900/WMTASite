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
    public int StudentId { get; set; }
    public string StudentName { get; set; }     // optional
    public string TimePreference { get; set; }  // optional
    public string Grade { get; set; } // optional
    public string AuditionType { get; set; } //optional
    public string AuditionTrack { get; set; } //optional

    public ScheduleSlot(int auditionId, int judgeId, string judgeName, int minutes, TimeSpan startTime, string timePref, 
        string grade, string audType, string audTrack, string studentName, int studentId)
    {
        AuditionId = auditionId;
        JudgeId = judgeId;
        JudgeName = judgeName;
        Minutes = minutes;
        StartTime = startTime;
        TimePreference = timePref;
        Grade = grade;
        AuditionType = audType;
        AuditionTrack = audTrack;
        StudentName = studentName;
        StudentId = studentId;
    }

    /*
     * Pre:
     * Post: Two schedule slots are equal if they have the same audition id...in theory
     */
    public override bool Equals(object obj)
    {
        return AuditionId == ((ScheduleSlot)obj).AuditionId;
    }

    public override int GetHashCode()
    {
        return AuditionId.GetHashCode();
    }
}
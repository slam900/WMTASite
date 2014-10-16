using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Miller
 * Date:   10/15/2014
 * This class holds the data needed to assign a judge to a room
 */
public class JudgeRoomAssignment
{
    public Judge judge { get; set; }
    public string room { get; set; }
    public List<Tuple<int, string>> times { get; set; } // item 1 = time id, item 2 = time string
    public int scheduleOrder { get; set; }

    public JudgeRoomAssignment(Judge judge, string room, List<Tuple<int, string>> times, int scheduleOrder)
    {
        this.judge = judge;
        this.room = room;
        this.times = times;
        this.scheduleOrder = scheduleOrder;
    }

    public override bool Equals(object obj)
    {
        JudgeRoomAssignment other = (JudgeRoomAssignment)obj;
        return other.judge.Equals(judge) && other.room.Equals(room);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   04/3/13
 * This class is a simplified version of the StudentCoordinate class 
 * in that it only keeps track of the coordinate student's audition
 * id and the reason for the coordinate since the year id and all of the
 * student's audition ids are not always necessary
 */
public class StudentCoordinateSimple
{
    public int auditionId { get; private set; }
    public string reason { get; private set; }
    public string coordinateName { get; set; }

	public StudentCoordinateSimple(int auditionId, string reason)
	{
        this.auditionId = auditionId;
        this.reason = reason;
	}

    public StudentCoordinateSimple(string coordinateName, string reason)
    {
        this.coordinateName = coordinateName;
        this.reason = reason;
    }
}
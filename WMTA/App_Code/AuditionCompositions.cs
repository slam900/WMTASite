using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   6/7/2013
 * This class stores a composition and the student's points associated
 * with the composition
 */
public class AuditionCompositions
{
    public Composition composition { get; private set; }
    public int points { get; set; }

	public AuditionCompositions(Composition composition, int points)
	{
        this.composition = composition;
        this.points = points;
	}

    /*
     * Pre:
     * Post: Returns true if the input object has the same composition and 
     *       point value as the current one
     */
    public override bool Equals(object obj)
    {
        AuditionCompositions other = (AuditionCompositions)obj;

        return other.composition.compositionId == composition.compositionId && other.points == points;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
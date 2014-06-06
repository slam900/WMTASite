using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   September 24, 2013
 * This class tracks a single preference of a judge.  A preference can be a
 * preferred audition type, audition level, composition level, instrument, or
 * time.
 */
public class JudgePreference
{
    public int preferenceId { get; private set; }
    public Utility.JudgePreferences preferenceType { get; private set; }
    public string preference { get; private set; }

    public JudgePreference(int preferenceId, Utility.JudgePreferences preferenceType, string preference)
	{
        this.preferenceId = preferenceId;
        this.preferenceType = preferenceType;
        this.preference = preference;
	}

    /*
     * Override Equals method
     * Two preferences are equal if they are of the same type and have the
     * same preference value
     */
    public override bool Equals(object obj)
    {
        return ((JudgePreference)obj).preferenceType == preferenceType &&
               ((JudgePreference)obj).preference.ToUpper().Equals(preference.ToUpper());
    }

    public override int GetHashCode()
    {
        return preferenceType.GetHashCode() * preference.GetHashCode();
    }
}
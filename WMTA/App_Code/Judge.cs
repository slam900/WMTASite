using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   October 2013
 * This class represents a judge in the system
 */
public class Judge : Contact
{
    public List<JudgePreference> preferences { get; private set; }

    /*
     * Constructor for creating a new judge
     */
    public Judge(string firstName, string mi, string lastName, string email, string phone,
                 int districtId, string contactTypeId) 
               : base(firstName, mi, lastName, email, phone, districtId, contactTypeId)
	{
        preferences = new List<JudgePreference>();
	}

    /*
     * Constructor for an existing judge
     * @param loadPreferences should be true if the judge's preferences need to be
     *        loaded from the database and false otherwise
     */
    public Judge(int id, string firstName, string mi, string lastName, string email,
                 string phone, int districtId, string contactTypeId, 
                 List<JudgePreference> preferences, bool loadPreferences)
               : base(id, firstName, mi, lastName, email, phone, districtId, contactTypeId)
    {
        if (!loadPreferences)
            this.preferences = preferences;
        else
            this.preferences = DbInterfaceContact.GetJudgePreferences(id);
    }

    /*
     * Constructor for loading the data of an existing judge
     */
    public Judge(int id)
        : base(id, true)
    {
        preferences = DbInterfaceContact.GetJudgePreferences(id);
    }

    /*
     *Pre:
     *Post:  The new preference is added to the judge's list of preferences
     *@param preferenceType is the type of the new preference
     *@param preference is the specific preference
     *@returns true if the preference was successfully added and false otherwise
     */
    public bool addPreference(Utility.JudgePreferences preferenceType, string preference)
    {
        bool result = true;
        JudgePreference newPref = DbInterfaceContact.AddJudgePreference(id, preferenceType, preference);

        if (preferences == null) preferences = new List<JudgePreference>();

        if (newPref != null)
            preferences.Add(newPref);
        else
            result = false;

        return result;
    }

    /*
     * Pre:
     * Post: The input preference is deleted front the judge's list of preferences
     * @param preferenceType is the type of the preference to delete
     * @param preference is the specific preference to delete
     * @returns true if the preference is successfully deleted and false otherwise
     */
    public bool deletePreference(Utility.JudgePreferences preferenceType, string preference)
    {
        bool result;
        JudgePreference prefToDelete = new JudgePreference(-1, preferenceType, preference);

        result = DbInterfaceContact.DeleteJudgePreference(id, preferenceType, preference);

        if (result)
            preferences.Remove(prefToDelete);

        return result;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   February 2013
 * This class represents a composition in the system
 */
public class Composition
{
    public int compositionId { get; private set; }
    public string title { get; private set; }
    public string composer { get; private set; }
    public string style { get; private set; }
    public string compLevel { get; private set; }
    public double playingTime { get; private set; }

    /* Constructor for existing compositions */
    public Composition(int id, string title, string composer, string style, string level, double time)
	{
		this.compositionId = id;
        this.title = title;
        this.composer = composer;
        this.style = style;
        this.compLevel = level;
        this.playingTime = time;
	}

    /* Constructor for new compositions */
    public Composition(string title, string composer, string style, string level, double time)
    {
        this.title = title;
        this.composer = composer;
        this.style = style;
        this.compLevel = level;
        this.playingTime = time;

        addToDatabase();
    }

    /* Constructor to retrieve id of an existing audition */
    public Composition(int id)
    {
        Composition comp = DbInterfaceComposition.GetComposition(id);

        if (comp != null)
        {
            this.compositionId = comp.compositionId;
            this.title = comp.title;
            this.composer = comp.composer;
            this.style = comp.style;
            this.compLevel = comp.compLevel;
            this.playingTime = comp.playingTime;
        }
        else 
        { 
            this.compositionId = -1; 
        }
    }

    /*
     * Pre:
     * Post: Add the new composition to the database 
     */
    private void addToDatabase()
    {
        compositionId = DbInterfaceComposition.AddComposition(title, composer, style, playingTime, compLevel);
    }

    /*
     * Pre:
     * Post: Update the information of the composition in the database
     */
    public bool updateInDatabase()
    {
        return DbInterfaceComposition.EditComposition(compositionId, title, composer, style, playingTime, compLevel);
    }

    /*
     * Pre:
     * Post: Determines whether the current composition has been used in an audition
     * @returns true if it has been used and false otherwise
     */
    public int getTimesUsedCount() 
    {
        return DbInterfaceComposition.GetCompositionUsageCount(compositionId);
    }
}
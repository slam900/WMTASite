using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Feedback
{
    public int id { get; private set; }
    public string name { get; set; }
    public string email { get; set; }
    public string feedbackType { get; set; }
    public Utility.Importance importance { get; set; }
    public string functionality { get; set; }
    public string description { get; set; }
    public string assignedTo { get; set; }
    public bool completed { get; private set; }
    public DateTime dateEntered { get; private set; }
    public DateTime dateComplete { get; private set; }
    public string versionCompleted { get; set; }

    public Feedback(string name, string email, string feedbackType, Utility.Importance importance,
                    string functionality, string description)
    {
        this.name = name;
        this.email = email;
        this.feedbackType = feedbackType;
        this.importance = importance;
        this.functionality = functionality;
        this.description = description;
        this.dateEntered = DateTime.Today;
    }

    /*
     * Pre:
     * Post: Marks the issue as being complete
     */
    public void Complete(string version)
    {
        completed = true;
        dateComplete = DateTime.Today;
        versionCompleted = version;
    }

    /*
     * Pre:
     * Post: Re-opens an issue that was previously marked as being completed
     */
    public void ReopenIssue()
    {
        completed = false;
        versionCompleted = "";
    }

    public void AddToDatabase()
    {
        DbInterfaceFeedback.AddFeedback(name, email, feedbackType, importance, functionality, description);
    }
}
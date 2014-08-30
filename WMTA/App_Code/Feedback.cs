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
    public string importance { get; set; }
    public string functionality { get; set; }
    public string description { get; set; }
    public string assignedTo { get; set; }
    public bool completed { get; private set; }
    public DateTime dateEntered { get; private set; }
    public DateTime dateComplete { get; private set; }
    public string versionCompleted { get; set; }

    public Feedback(string name, string email, string feedbackType, string importance,
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

    public Feedback(string name, string email, string feedbackType, string importance,
                    string functionality, string description, string assignedTo, bool completed,
                    DateTime dateEntered, DateTime dateComplete)
    {

        this.name = name;
        this.email = email;
        this.feedbackType = feedbackType;
        this.importance = importance;
        this.functionality = functionality;
        this.description = description;
        this.assignedTo = assignedTo;
        this.completed = completed;
        this.dateEntered = dateEntered;
        this.dateComplete = dateComplete;
    }

    /*
     * Pre:  Feedback with input id must exist in the system
     * Post: Feedback is loaded
     */
    public Feedback(int id)
    {
        this.id = id;

        LoadFeedback();
    }

    /*
     * Pre:
     * Post: Update the complete status of the feedback
     */
    public void SetComplete(bool complete)
    {
        if (complete)
            Complete();
        else
            ReopenIssue();
    }

    /*
     * Pre:
     * Post: Marks the issue as being complete
     */
    private void Complete()
    {
        completed = true;
        dateComplete = DateTime.Today;
        versionCompleted = Utility.version;
    }

    /*
     * Pre:
     * Post: Re-opens an issue that was previously marked as being completed
     */
    private void ReopenIssue()
    {
        completed = false;
        dateComplete = DateTime.MinValue;
        versionCompleted = "";
    }

    /*
     * Pre:  Feedback with current id must exist in the system
     * Post: Feedback is loaded
     */
    public void LoadFeedback()
    {
        Feedback temp = DbInterfaceFeedback.LoadFeedback(this.id);

        if (temp != null)
        {
            this.name = temp.name;
            this.email = temp.email;
            this.feedbackType = temp.feedbackType;
            this.importance = temp.importance;
            this.functionality = temp.functionality;
            this.description = temp.description;
            this.assignedTo = temp.assignedTo;
            this.completed = temp.completed;
            this.dateEntered = temp.dateEntered;
            this.dateComplete = temp.dateComplete;
        }
        else
        {
            this.id = -1;
        }
    }

    /*
     * Pre:
     * Post: Add feedback data to database
     * @returns true if successful and false otherwise
     */
    public bool AddToDatabase()
    {
        return DbInterfaceFeedback.AddFeedback(name, email, feedbackType, importance, functionality, description);
    }

    /*
     * Pre:
     * Post: Update the feedback in the database
     * @returns true if successful and false otherwise
     */
    public bool Update()
    {
        return DbInterfaceFeedback.UpdateFeedback(this);
    }
}
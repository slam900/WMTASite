using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Resources
{
    public partial class ViewHelpRequests : System.Web.UI.Page
    {
        private Feedback feedback;
        private string feedbackSession = "Feedback";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            if (!Page.IsPostBack)
            {
                Session[feedbackSession] = null;

                FilterRecords();
            }
        }

        /*
         * Pre:
         * Post: If the user is not logged in they will be redirected to the welcome screen.
         *       Show only composition requests if user has composition permission and not
         *       admin permission
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to welcome screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("../Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("A") || user.permissionLevel.Contains("C")))
                {
                    Response.Redirect("../Default.aspx");
                }
            }
        }

        /*
         * Pre:
         * Post: Filter based on user type and whether the user would like to view
         *       all items or just open items
         */
        protected void rblFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterRecords();
        }

        /*
         * Pre:
         * Post: Filter based on user type and whether the user would like to view
         *       all items or just open items
         */
        private void FilterRecords()
        {
            User user = (User)Session[Utility.userRole];

            //show either all items or all open items
            if (rblFilter.SelectedIndex == 0 && user.permissionLevel.Contains("A"))
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM [Feedback] WHERE [Completed] = 0 ORDER BY [Id]";
            }
            else if (rblFilter.SelectedIndex == 0 && user.permissionLevel.Contains("C"))
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM [Feedback] WHERE [Completed] = 0 AND [FeedbackType] = 'Composition Question' ORDER BY [Id]";
            }
            else if (rblFilter.SelectedIndex == 1 && user.permissionLevel.Contains("A"))
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM [Feedback] ORDER BY [Completed], [Id]";
            }
            else if (rblFilter.SelectedIndex == 1 && user.permissionLevel.Contains("C"))
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM [Feedback] WHERE [FeedbackType] = 'Composition Question' ORDER BY [Completed], [Id]";
            }

            gvRequests.DataBind();
        }

        /*
         * Pre:
         * Post: Load the selected request to the edit form
         */
        protected void gvRequests_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedRow = gvRequests.SelectedIndex;
            ClearForm();

            if (selectedRow >= 0)
            {
                int id = Convert.ToInt32(gvRequests.Rows[selectedRow].Cells[1].Text);
                feedback = new Feedback(id);

                Session[feedbackSession] = feedback;

                //load to edit form
                LoadFeedback(feedback);
                upEdit.Visible = true;

                //scroll to edit form
                btnSubmit.Focus();
            }
            else
            {
                upEdit.Visible = false;
            }
        }

        /*
         * Pre:
         * Post: Load the input feedback data to the edit form
         */
        private void LoadFeedback(Feedback feedback)
        {
            //load feedback type
            ListItem currItem = rblFeedbackType.Items.FindByValue(feedback.feedbackType);
            if (currItem != null)
                rblFeedbackType.SelectedIndex = rblFeedbackType.Items.IndexOf(currItem);

            //load importance
            currItem = rblImportance.Items.FindByValue(feedback.importance);
            if (currItem != null)
                rblImportance.SelectedIndex = rblImportance.Items.IndexOf(currItem);

            //load text fields
            txtFunctionality.Text = feedback.functionality;
            txtDescription.Text = feedback.description;
            txtAssignedTo.Text = feedback.assignedTo;
            chkComplete.Checked = feedback.completed;
        }

        /*
         * Pre:
         * Post: Submit updates to selected item and refresh the gridview
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (gvRequests.SelectedIndex >= 0)
            {
                if (feedback == null)
                {
                    feedback = (Feedback)Session[feedbackSession];
                }

                feedback.feedbackType = rblFeedbackType.SelectedValue.ToString();
                feedback.importance = rblImportance.SelectedValue.ToString();
                feedback.functionality = txtFunctionality.Text;
                feedback.description = txtDescription.Text;
                feedback.assignedTo = txtAssignedTo.Text;
                feedback.SetComplete(chkComplete.Checked);

                if (feedback.Update())
                {
                    FilterRecords();
                    gvRequests.DataBind();

                    upEdit.Visible = false;
                    ClearForm();
                    
                    showSuccessMessage("The request was successfully updated.");
                }
                else
                {
                    showErrorMessage("Error: There was an error updating the request.");
                }
            }
            else
            {
                showWarningMessage("Please select a request to edit.");   
            }
        }

        /*
         * Pre:
         * Post: Clear the edit form
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();

            Session[feedbackSession] = null;
        }

        /*
         * Pre:
         * Post: Clear the edit form
         */
        private void ClearForm()
        {
            rblFeedbackType.SelectedIndex = -1;
            rblImportance.SelectedIndex = -1;
            txtFunctionality.Text = "";
            txtDescription.Text = "";
            txtAssignedTo.Text = "";
            chkComplete.Checked = false;
        }

        /*
        * Pre:
        * Post: Displays the input error message in the top-left corner of the screen
        * @param message is the message text to be displayed
        */
        private void showErrorMessage(string message)
        {
            lblErrorMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowError", "showMainError()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input warning message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showWarningMessage(string message)
        {
            lblWarningMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowWarning", "showWarningMessage()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input success message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showSuccessMessage(string message)
        {
            lblSuccessMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowSuccess", "showSuccessMessage()", true);
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("ViewHelpRequests", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
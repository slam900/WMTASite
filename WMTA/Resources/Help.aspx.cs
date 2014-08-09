using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Resources
{
    public partial class Help : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (verifyEmailAddress())
                {
                    Utility.Importance importance = Utility.Importance.Low;
                    if (rblImportance.SelectedIndex == 1)
                        importance = Utility.Importance.Medium;
                    else if (rblImportance.SelectedIndex == 2)
                        importance = Utility.Importance.High;

                    Feedback feedback = new Feedback(txtName.Text, txtEmail.Text, rblFeedbackType.SelectedValue.ToString(),
                                                     importance, txtFunctionality.Text, txtDescription.Text);
                    feedback.AddToDatabase();
                    sendEmail(feedback);

                    showSuccessMessage("Your feedback has been sent successfully.");
                    clearPage();
                }
            }
            catch (Exception ex)
            {
                Utility.LogError("Help - Send Feedback", "btnSubmit", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
                showErrorMessage("Error: An error occurred - your feedback may not have been sent.");
            }
        }

        /*
         * Make sure email address includes @ and .
         */
        private bool verifyEmailAddress()
        {
            bool valid = true;
            char[] arr = txtEmail.Text.ToCharArray();
            int atPosition = 0, dotPosition = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals('@'))
                    atPosition = i;
                else if (arr[i].Equals('.'))
                    dotPosition = i;
            }

            if (atPosition == 0 || dotPosition == 0 || atPosition > dotPosition)
            {
                showWarningMessage("Please enter a valid email address.");
                valid = false;
            }

            return valid;
        }

        /*
         * Pre:
         * Post: Send email to admins with feedback details
         */
        private void sendEmail(Feedback feedback)
        {
            try
            {
                MailMessage message = new MailMessage();

                if (feedback.feedbackType.Contains("Composition"))
                {
                    message.To.Add(Utility.compositionEmail);

                    message.Subject = "Composition Change Request from Ovation";
                    message.Body = "This is an automated email from Ovation.  This issue has been logged in the database and can be viewed and modified at *some page I still need to create*\n\nName: " +
                                   feedback.name + "\nEmail: " + feedback.email + "\nFeedback Type: " + feedback.feedbackType + "\nImportance: " + feedback.importance + "\nFunctionality: " +
                                   feedback.functionality + "\nDescription: " + feedback.description;
                }
                else
                {
                    message.To.Add(Utility.ovationEmail);

                    message.Subject = "Feedback from Ovation - " + feedback.feedbackType;
                    message.Body = "This is an automated email from Ovation.  This issue has been logged in the database and can be viewed and modified at *some page I still need to create*\n\nName: " +
                                   feedback.name + "\nEmail: " + feedback.email + "\nFeedback Type: " + feedback.feedbackType + "\nImportance: " + feedback.importance + "\nFunctionality: " +
                                   feedback.functionality + "\nDescription: " + feedback.description;
                }
                
                SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587);
                mailer.Credentials = new NetworkCredential(Utility.ovationEmail, Utility.ovationPassword);
                mailer.EnableSsl = true;
                
                mailer.Send(message);
            }
            catch (Exception e)
            {
                Utility.LogError("Help - Send Feedback", "sendEmail", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Clear the page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: Clear all data on the page
         */
        private void clearPage()
        {
            txtName.Text = "";
            txtEmail.Text = "";
            rblFeedbackType.SelectedIndex = -1;
            rblImportance.SelectedIndex = 0;
            txtFunctionality.Text = "";
            txtDescription.Text = "";
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
            Utility.LogError("Help", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            showErrorMessage("Error: An error occurred.");

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
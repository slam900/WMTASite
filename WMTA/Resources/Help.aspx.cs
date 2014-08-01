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
            catch (Exception ex)
            {
                Utility.LogError("Help - Send Feedback", "btnSubmit", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);
                showErrorMessage("Error: An error occurred - your feedback may not have been sent.");
            }
        }

        /*
         * Pre:
         * Post: Send email to admins with feedback details
         */
        private void sendEmail(Feedback feedback)
        {
            try
            {
                MailMessage message = new MailMessage("schultz.kris@uwlax.edu", "schultz.kris@uwlax.edu");
                message.Subject = "Feedback from Ovation";
                message.Body = "This is an automated email from Ovation.  This issue has been logged in the database and can be viewed and modified at *some page I still need to create*\n\nName: " + 
                               feedback.name + "\nEmail: " + feedback.email + "\nFeedback Type: " + feedback.feedbackType + "\nImportance: " + feedback.importance + "\nFunctionality: " + 
                               feedback.functionality + "\nDescription: " + feedback.description;
                SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587);
                mailer.Credentials = new NetworkCredential("schultz.kris@uwlax.edu", "January182014!!!");
                //mailer.Credentials = new NetworkCredential("wmtaOvation@gmail.com", "wiMTA2013*");
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
            rblImportance.SelectedIndex = -1;
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
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowError", "showMainError(" + message + ")", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowMainError", "showMainError(" + message + ")", true);
            lblErrorMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowError", "showMainError()", true);
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
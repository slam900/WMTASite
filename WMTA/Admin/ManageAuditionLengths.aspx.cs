using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Admin
{
    public partial class ManageAuditionLengths : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
        }

        /*
         * Pre:
         * Post: If the user is not logged in or has invalid credentials they will be redirected to the welcome screen
         *       System administrators  can use this page
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!user.permissionLevel.Contains("A"))
                    Response.Redirect("/Default.aspx");
            }
        }

        /*
         * Update the min and max lengths
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int min = 0, max = 0;

            if (ddlCompLevel.SelectedIndex > 0 && Int32.TryParse(txtMinimum.Text, out min) && Int32.TryParse(txtMaximum.Text, out max) && min <= max)
            {
                if (DbInterfaceAdmin.UpdateLevelLengthLimits(ddlCompLevel.SelectedValue, min, max))
                {
                    ddlCompLevel.SelectedIndex = 0;
                    txtMinimum.Text = "";
                    txtMaximum.Text = "";
                    showSuccessMessage("The limits were successfully updated.");
                }
                else
                    showErrorMessage("The limits could not be updated.");
            }
            else
            {
                showWarningMessage("Please ensure all fields are filled in and that the minimum is less than or equal to the maximum.");
            }
        }

        /*
         * Load the min and max lengths
         */
        protected void ddlCompLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCompLevel.SelectedIndex > 0)
            {
                Tuple<int, int> limits = DbInterfaceAdmin.LoadLevelLengthLimits(ddlCompLevel.SelectedValue);

                txtMinimum.Text = limits.Item1.ToString();
                txtMaximum.Text = limits.Item2.ToString();
            }
            else
            {
                txtMinimum.Text = "";
                txtMaximum.Text = "";
            }
        }

        #region Messages

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
         * Post: Displays the input informational message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showInfoMessage(string message)
        {
            lblInfoMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowInfo", "showInfoMessage()", true);
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

        #endregion Messages

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Repertoire", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            showErrorMessage("Error: An error occurred.");

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
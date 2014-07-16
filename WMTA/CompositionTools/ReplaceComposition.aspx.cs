using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class ReplaceComposition : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
        }

        /*
         * Pre:
         * Post: If the user is not logged in or has invalid credentials they will be redirected to the welcome screen
         *       System administrators and those with composition rights can use this page
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("A") || user.permissionLevel.Contains("C")))
                {
                    Response.Redirect("/Default.aspx");
                }
            }
        }

        #region Find Usage

        /*
         * Pre:
         * Post: The selected composition to replace is replaced by the replacement composition.
         *       The composition to replace is then deleted from the system.
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                //replace first composition with second
                int idToReplace = Convert.ToInt32(ddlCompositionToReplace.SelectedValue);
                int replacementId = Convert.ToInt32(ddlReplacement.SelectedValue);
                bool success = DbInterfaceComposition.ReplaceComposition(idToReplace, replacementId);

                if (success)
                {
                    clearPage();
                    showSuccessMessage("The composition was successfully replaced.");
                }
                else
                {
                    showErrorMessage("Error: There was an error replacing the selected composition.");
                }
            }
            else //show error message if required data is missing
            {
                showWarningMessage("Please fill in all required fields.");
            }
        }

        #endregion Find Usage

        #region Clear Functions

        /*
         * Pre:
         * Post: All data on the page is cleared
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: All data on the page is cleared
         */
        private void clearPage()
        {
            ddlCompositionToReplace.SelectedIndex = -1;
            ddlReplacement.SelectedIndex = -1;
        }

        #endregion Clear Functions

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
         * Post: Displays the input success message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showSuccessMessage(string message)
        {
            lblSuccessMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowSuccess", "showSuccessMessage()", true);
        }

        #endregion Messages
    }
}
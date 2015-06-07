using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class ReplaceComposerName : System.Web.UI.Page
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

        #region Replace Composer

        /*
         * Pre:
         * Post: The selected composer name is changed to the input composer name
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                string currentName = ddlComposer.SelectedValue.ToString();
                string newName = getNewComposerName();

                bool success = DbInterfaceComposition.ReplaceComposer(currentName, newName);

                if (success)
                {
                    showSuccessMessage("All occurrences of " + currentName + " were successfully updated to " + newName);

                    clearPage();

                    //update dropdown list
                    ddlComposer.DataSource = null;
                    ddlComposer.DataBind();
                    ddlComposer.Items.Clear();

                    ddlComposer.Items.Add(new ListItem("", ""));
                    ddlComposer.DataSourceID = "WmtaDataSource5";
                    ddlComposer.DataBind();
                }
                else
                {
                    showErrorMessage("Error: An error occurred while updating the composer name.");
                }
            }
            else //show error message if required data is missing
            {
                showWarningMessage("Please fill in all required fields.");
            }
        }



        /*
         * Pre:
         * Post: Constructs the new composer name
         * @returns the new composer name
         */
        private string getNewComposerName()
        {
            //get new composer name
            string composer = txtComposerLast.Text.Trim();

            //get first and middle initials, if entered
            if (!txtComposerFI.Text.Equals(""))
            {
                composer = composer + ", " + txtComposerFI.Text.Trim() + ".";

                if (!txtComposerMI.Text.Equals(""))
                    composer = composer + txtComposerMI.Text.Trim() + ".";
            }
            else if (!txtComposerMI.Text.Equals(""))
            {
                composer = composer + ", " + txtComposerMI.Text.Trim() + ".";
            }

            return composer;
        }

        #endregion Replace Composer

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
            ddlComposer.SelectedIndex = 0;
            txtComposerLast.Text = "";
            txtComposerFI.Text = "";
            txtComposerMI.Text = "";
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
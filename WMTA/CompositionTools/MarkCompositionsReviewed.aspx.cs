using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class MarkCompositionsReviewed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
            gvCompositions.DataBind();
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int fromId, toId;

            // Just search single id if a range isn't entered
            if (txtToId.Text.Equals(""))
                txtToId.Text = txtFromId.Text;

            if (Int32.TryParse(txtFromId.Text, out fromId) && Int32.TryParse(txtToId.Text, out toId))
            {
                if (DbInterfaceComposition.MarkCompositionsReviewed(fromId, toId))
                {
                    showSuccessMessage("The compositions were successfully marked as being reviewed.");
                    txtFromId.Text = "";
                    txtToId.Text = "";
                    gvCompositions.DataBind();
                }
                else
                    showErrorMessage("The compositions could not be marked as being reviewed.");
            }
            else
            {
                showWarningMessage("Please enter a range of composition ids to mark as reviewed.");
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

        protected void gvCompositions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompositions.PageIndex = e.NewPageIndex;
        }

        protected void gvCompositions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvCompositions, e);
        }

        /*
         * Pre:  The input must be a gridview that exists on the current page
         * Post: The background of the header row is set
         * @param gv is the gridView that will have its header row color changed
         * @param e are the event args for the event fired by the row being bound to data
         */
        private void setHeaderRowColor(GridView gv, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in gv.HeaderRow.Cells)
                {
                    cell.BackColor = Color.Black;
                    cell.ForeColor = Color.White;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Admin
{
    public partial class ManageAuditionTrackFees : System.Web.UI.Page
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
         * Load a track to be updated
         */
        protected void gvFees_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvFees.SelectedIndex;

            if (index >= 0 && index< gvFees.Rows.Count)
            {
                string track = gvFees.Rows[index].Cells[1].Text;
                string region = gvFees.Rows[index].Cells[2].Text;
                string type = gvFees.Rows[index].Cells[3].Text;
                string fee = gvFees.Rows[index].Cells[4].Text;

                lblTrack.Text = track + " " + region + " " + type;
                txtFee.Text = fee;
            }
            else
            {
                lblTrack.Text = "";
                txtFee.Text = "";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (DataIsValid())
            {
                string[] info = lblTrack.Text.Split(' ');
                string track = info[0];
                string region = info[1];
                string type = info[2];

                if (DbInterfaceAdmin.UpdateTrackFees(type, track, region, Decimal.Parse(txtFee.Text)))
                {
                    txtFee.Text = "";
                    lblTrack.Text = "";
                    gvFees.DataBind();
                    showSuccessMessage("The fee was successfully updated.");
                }
                else
                    showErrorMessage("The fee could not be updated.");
            }
        }

        /*
         * Determine whether the data is filled in and valid
         */ 
        private bool DataIsValid()
        {
            decimal fee;
            bool valid = true;

            if (lblTrack.Text.Equals("") || txtFee.Text.Equals(""))
            {
                showWarningMessage("Please select a fee type to edit.");
                valid = false;
            }
            else if (!Decimal.TryParse(txtFee.Text, out fee))
            {
                showWarningMessage("Please enter a valid fee.");
                valid = false;
            }
            else if (fee < 0)
            {
                showWarningMessage("Fees must be greater than or equal to 0.");
                valid = false;
            }

            return valid;
        }

        protected void gvFees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFees.PageIndex = e.NewPageIndex;
        }

        protected void gvFees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvFees, e);
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
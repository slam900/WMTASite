using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //clear session variable
            if (!Page.IsPostBack)
                Session[Utility.userRole] = null;
        }

        /*
         * Pre:
         * Post: The system attempts to log the user into the system using the entered information which
         *       should be first initial and last name as the user id and MTNA id as password.  The login
         *       attempt will fail if the user has not been registered for the current year
         */
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Value.Length > 1)
            {
                User currUser = new User(txtUsername.Value.Substring(0, 1), txtUsername.Value.Substring(1), txtPassword.Text);
                Session[Utility.userRole] = currUser;

                if (!currUser.permissionLevel.Equals(""))
                    Response.Redirect("/WelcomeScreen.aspx");
                else
                {
                    lblError.Visible = true;
                    Session[Utility.userRole] = null;
                }
            }
            else
            {
                lblError.Visible = true;
                Session[Utility.userRole] = null;
            }
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Default", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            //lblError.Text = "An error occurred";
            //lblError.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
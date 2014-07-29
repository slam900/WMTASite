using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class Login : System.Web.UI.Page
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
            User currUser = new User(txtUsername.Text.Substring(0, 1), txtUsername.Text.Substring(1), txtPassword.Text);
            Session[Utility.userRole] = currUser;

            if (currUser.permissionLevel != null && !currUser.permissionLevel.Equals(""))
            {
                //redirect to appropriate menu
                if (currUser.permissionLevel.Contains("A"))
                    Response.Redirect("~/Account/SystemAdminMenu.aspx");
                else if (currUser.permissionLevel.Contains("D"))
                    Response.Redirect("~/Account/DistrictChairMenu.aspx");
                else if (currUser.permissionLevel.Equals("T") || currUser.permissionLevel.Equals("TJ"))
                    Response.Redirect("~/Account/TeacherMenu.aspx");
                else
                    Response.Redirect("~/WelcomeScreen.aspx");
            }
            else
            {
                FailureText.Text = "Invalid username or password";
                ErrorMessage.Visible = true;
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
            Utility.LogError("Login", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Response.Redirect("~/ErrorPage.aspx");
        }
    }
}
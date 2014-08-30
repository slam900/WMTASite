using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Account
{
    public partial class TeacherMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
        }

        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                //make sure the user has sufficient permissions
                if (!user.permissionLevel.Contains("T"))
                    Response.Redirect("/Default.aspx");

                //show composition tools if user has C permissions
                if (user.permissionLevel.Contains("C"))
                {
                    pnlCompositionPermissions.Visible = true;
                    pnlNoCompositionPermissions.Visible = false;
                    pnlHelp.Visible = true;
                }
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
            Utility.LogError("TeacherMenu", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}
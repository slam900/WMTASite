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
            //Server.Transfer("ErrorPage.aspx", true);
            Response.Redirect("../ErrorPage.aspx");
        }
    }
}
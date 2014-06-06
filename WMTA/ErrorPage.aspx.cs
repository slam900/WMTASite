using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//code modified from http://msdn.microsoft.com/en-us/library/bb397417(v=vs.100).aspx

namespace WMTA
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the last error from the server
            Exception ex = Server.GetLastError();

            // Create a safe message
            string safeMsg = "A problem has occurred in the web site. ";

            // Show Inner Exception fields for local access
            if (ex.InnerException != null)
            {
                innerTrace.Text = ex.InnerException.StackTrace;
                InnerErrorPanel.Visible = Request.IsLocal;
                innerMessage.Text = ex.InnerException.Message;
            }

            // Show Trace for local access
            if (Request.IsLocal)
                exTrace.Visible = true;
            else
                ex = new Exception(safeMsg, ex);

            // Fill the page fields
            exMessage.Text = ex.Message;
            exTrace.Text = ex.StackTrace;

            // Log the exception 
            Utility.LogError("Error Page", "", "", "Message: " + ex.Message + "   Stack Trace: " + ex.StackTrace, -1);

            // Clear the error from the server
            Server.ClearError();
        }
    }
}
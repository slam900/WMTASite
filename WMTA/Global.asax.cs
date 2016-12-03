using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WMTA
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //code that runs when an unhandled error occurs
        void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Handle HTTP errors
            if (exc != null && exc.GetType() == typeof(HttpException))
            {
                //Redirect HTTP errors to HttpError page
                Server.Transfer("HttpErrorPage.aspx");
            }

            String msg = (exc == null)
                ? "Unkown Error"
                : exc.Message;

            // For other kinds of errors give the user some information
            //   but stay on the default page
            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write(
                "<p>" + msg + "</p>\n");
            Response.Write("Return to the <a href='~/WelcomeScreen.aspx'>" +
                "Home Page</a>\n");

            // Clear the error from the server
            Server.ClearError();
        }
    }
}
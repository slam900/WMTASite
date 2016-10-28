using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Contacts
{
    public partial class ViewRegisteredContacts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                checkPermissions();
            }
        }

        /*
         * Pre:
         * Post: If the user is not logged in they will be redirected to the welcome screen
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
                {
                    Response.Redirect("/Default.aspx");
                }
            }
        }

        protected void gvContacts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvContacts.PageIndex = e.NewPageIndex;
        }

        protected void gvContacts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvContacts, e);
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
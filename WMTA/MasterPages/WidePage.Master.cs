using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.MasterPages
{
    public partial class WidePage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Utility.userRole] == null || ((User)Session[Utility.userRole]).permissionLevel == null)
            {
                ulSystemAdmin.Style["display"] = "none";
                ulTeacher.Style["display"] = "none";
                ulDistrictChair.Style["display"] = "none";
                ulStateAdmin.Style["display"] = "none";
            }
            //system admin
            else if (((User)Session[Utility.userRole]).permissionLevel.Contains("A"))
            {
                ulNotLoggedIn.Style["display"] = "none";
                ulTeacher.Style["display"] = "none";
                ulDistrictChair.Style["display"] = "none";
                ulStateAdmin.Style["display"] = "none";
            }
            //state admin
            else if (((User)Session[Utility.userRole]).permissionLevel.Contains("S"))
            {
                ulNotLoggedIn.Style["display"] = "none";
                ulTeacher.Style["display"] = "none";
                ulDistrictChair.Style["display"] = "none";
                ulSystemAdmin.Style["display"] = "none";
            }
            //district chair
            else if (((User)Session[Utility.userRole]).permissionLevel.Contains("D"))
            {
                ulSystemAdmin.Style["display"] = "none";
                ulNotLoggedIn.Style["display"] = "none";
                ulTeacher.Style["display"] = "none";
                ulStateAdmin.Style["display"] = "none";
            }
            //teacher
            else if (((User)Session[Utility.userRole]).permissionLevel.Contains("T"))
            {
                ulSystemAdmin.Style["display"] = "none";
                ulNotLoggedIn.Style["display"] = "none";
                ulDistrictChair.Style["display"] = "none";
                ulStateAdmin.Style["display"] = "none";
            }
        }

        protected void LogOut(object sender, EventArgs e)
        {
            Session[Utility.userRole] = null;
            Response.Redirect("~/");
        }
    }
}
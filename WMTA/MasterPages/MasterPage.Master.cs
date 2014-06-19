using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.MasterPages
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Utility.userRole] == null || ((User)Session[Utility.userRole]).permissionLevel == null)
            {
                ulSystemAdmin.Style["display"] = "none";
                ulCustomer.Style["display"] = "none";
                ulStaff.Style["display"] = "none";
            } 
            else if (((User)Session[Utility.userRole]).permissionLevel.Contains("A")) 
            {
                ulNotLoggedIn.Style["display"] = "none";
                ulCustomer.Style["display"] = "none";
                ulStaff.Style["display"] = "none";
            }
            //else if ((UtilityClass.UserTypes)Session["userType"] == UtilityClass.UserTypes.Manager)
            //{
                //ulNotLoggedIn.Style["display"] = "none";
                //ulCustomer.Style["display"] = "none";
                //ulStaff.Style["display"] = "none";
            //}
            //else if ((UtilityClass.UserTypes)Session["userType"] == UtilityClass.UserTypes.Staff)
            //{
            //    ulNotLoggedIn.Style["display"] = "none";
            //    ulCustomer.Style["display"] = "none";
            //    ulManager.Style["display"] = "none";
            //}
            //else if ((UtilityClass.UserTypes)Session["userType"] == UtilityClass.UserTypes.Customer)
            //{
            //    ulNotLoggedIn.Style["display"] = "none";
            //    ulManager.Style["display"] = "none";
            //    ulStaff.Style["display"] = "none";
            //}
            //else
            //{
            //    ulManager.Style["display"] = "none";
            //    ulCustomer.Style["display"] = "none";
            //    ulStaff.Style["display"] = "none";
            //}
        }

        protected void LogOut(object sender, EventArgs e)
        {
            Session[Utility.userRole] = null;
            Response.Redirect("~/");
        }
    }
}
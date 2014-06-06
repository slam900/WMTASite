using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class WelcomeScreen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
        }

        /*
         * Pre:
         * Post: If the user is not logged in they will be redirected to the welcome screen,
         *       otherwise, links will be shown and hidden based on the user's permissions
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
                setValidActions();
        }

        /*
         * Pre:   The UserRole session variable must contain a User object
         * Post:  The links associated with the actions that should be available
         *        to the user are shown and the others are hidden
         */
        private void setValidActions()
        {
            User user = (User)Session[Utility.userRole];

            if (user.permissionLevel.Contains("A"))
                setSystemAdminActions();
            else if (user.permissionLevel.Contains("S"))
                setStateAdminActions();
            else if (user.permissionLevel.Contains("D"))
                setDistrictAdminActions();
            else if (user.permissionLevel.Contains("C"))
                setCompositionMngrActions();
            else if (user.permissionLevel.Contains("T"))
                setTeacherActions();
            else
                disableAll();
        }

        /*
         * Pre:
         * Post: Set system administrator available actions
         */
        private void setSystemAdminActions()
        {
            lnkDistrictAudition.Visible = true;
            lnkBadgerAudition.Visible = true;
            lnkDistrictRegistration.Visible = true;
            lnkBadgerRegistration.Visible = true;
            lnkCoordinateStudents.Visible = true;
            lnkDistrictPoints.Visible = true;
            lnkBadgerPoints.Visible = true;
            lnkEnterHsVPoints.Visible = true;
            lnkManageStudents.Visible = true;
            lnkManageContacts.Visible = true;
            lnkManageRepertoire.Visible = true;
            lnkRegisterContacts.Visible = true;
            lnkReports.Visible = true;
            lnkResources.Visible = true;
        }

        /*
         * Pre:
         * Post: Set state administrator available actions
         */
        private void setStateAdminActions()
        {
            lnkDistrictAudition.Visible = false;
            lnkBadgerAudition.Visible = true;
            lnkDistrictRegistration.Visible = true;
            lnkBadgerRegistration.Visible = true;
            lnkCoordinateStudents.Visible = true;
            lnkDistrictPoints.Visible = false;
            lnkBadgerPoints.Visible = true;
            lnkEnterHsVPoints.Visible = false;
            lnkManageStudents.Visible = true;
            lnkManageContacts.Visible = true;
            lnkManageRepertoire.Visible = true;
            lnkRegisterContacts.Visible = false;
            lnkReports.Visible = true;
            lnkResources.Visible = true;
        }

        /*
         * Pre:
         * Post: Set district administrator available actions
         */
        private void setDistrictAdminActions()
        {
            lnkDistrictAudition.Visible = true;
            lnkBadgerAudition.Visible = false;
            lnkDistrictRegistration.Visible = true;
            lnkBadgerRegistration.Visible = true;
            lnkCoordinateStudents.Visible = true;
            lnkDistrictPoints.Visible = true;
            lnkBadgerPoints.Visible = false;
            lnkEnterHsVPoints.Visible = true;
            lnkManageStudents.Visible = true;
            lnkManageContacts.Visible = true;
            lnkManageRepertoire.Visible = true;
            lnkRegisterContacts.Visible = false;
            lnkReports.Visible = true;
            lnkResources.Visible = true;
        }

        /*
         * Pre:
         * Post: Set composition manager available actions
         */
        private void setCompositionMngrActions()
        {
            lnkDistrictAudition.Visible = false;
            lnkBadgerAudition.Visible = false;
            lnkDistrictRegistration.Visible = true;
            lnkBadgerRegistration.Visible = true;
            lnkCoordinateStudents.Visible = true;
            lnkDistrictPoints.Visible = false;
            lnkBadgerPoints.Visible = false;
            lnkEnterHsVPoints.Visible = false;
            lnkManageStudents.Visible = true;
            lnkManageContacts.Visible = true;
            lnkManageRepertoire.Visible = true;
            lnkRegisterContacts.Visible = false;
            lnkReports.Visible = true;
            lnkResources.Visible = true;
        }

        /*
         * Pre:
         * Post: Set teacher available actions
         */
        private void setTeacherActions()
        {
            lnkDistrictAudition.Visible = false;
            lnkBadgerAudition.Visible = false;
            lnkDistrictRegistration.Visible = true;
            lnkBadgerRegistration.Visible = true;
            lnkCoordinateStudents.Visible = true;
            lnkDistrictPoints.Visible = false;
            lnkBadgerPoints.Visible = false;
            lnkEnterHsVPoints.Visible = false;
            lnkManageStudents.Visible = true;
            lnkManageContacts.Visible = true;
            lnkManageRepertoire.Visible = true;
            lnkRegisterContacts.Visible = false;
            lnkReports.Visible = true;
            lnkResources.Visible = true;
        }

        /*
         * Pre:
         * Post: Disables all actions
         */
        private void disableAll()
        {
            lnkDistrictAudition.Visible = false;
            lnkBadgerAudition.Visible = false;
            lnkDistrictRegistration.Visible = false;
            lnkBadgerRegistration.Visible = false;
            lnkCoordinateStudents.Visible = false;
            lnkDistrictPoints.Visible = false;
            lnkBadgerPoints.Visible = false;
            lnkEnterHsVPoints.Visible = false;
            lnkManageStudents.Visible = false;
            lnkManageContacts.Visible = false;
            lnkManageRepertoire.Visible = false;
            lnkRegisterContacts.Visible = false;
            lnkReports.Visible = false;
            lnkResources.Visible = false;
        }
    }
}
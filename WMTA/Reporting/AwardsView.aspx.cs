using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Events
{
    public partial class AwardsView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                checkPermissions();

                loadYearDropdown();
                loadDistrictDropdown();
                loadTeacherDropdown();
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
                Response.Redirect("../Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("A") || user.permissionLevel.Contains("T")))
                    Response.Redirect("../Default.aspx");
            }
        }

        /*
         * Pre:
         * Post: Loads the appropriate years in the dropdown
         */
        private void loadYearDropdown()
        {
            int firstYear = DbInterfaceStudentAudition.GetFirstAuditionYear();

            for (int i = DateTime.Now.Year + 1; i >= firstYear; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        /*
         * Pre:
         * Post:  If the current user is not an administrator, the district
         *        dropdowns are filtered to containing only the current
         *        user's district
         */
        private void loadDistrictDropdown()
        {
            User user = (User)Session[Utility.userRole];

            if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains('S'))) //if the user is a district admin, add only their district
            {
                //get own district dropdown info
                string districtName = DbInterfaceStudent.GetStudentDistrict(user.districtId);

                //add new item to dropdown and select it
                ddlDistrictSearch.Items.Add(new ListItem(districtName, user.districtId.ToString()));
                ddlDistrictSearch.SelectedIndex = 1;
            }
            else //if the user is an administrator, add all districts
            {
                ddlDistrictSearch.DataSource = DbInterfaceAudition.GetDistricts();

                ddlDistrictSearch.DataTextField = "GeoName";
                ddlDistrictSearch.DataValueField = "GeoId";

                ddlDistrictSearch.DataBind();
            }
        }

        /*
         * Pre:
         * Post: If the current user is a teacher, the teacher dropdown
         *       should only show the current user
         */
        private void loadTeacherDropdown()
        {
            if (HighestPermissionTeacher())
            {
                User user = (User)Session[Utility.userRole];
                Contact contact = DbInterfaceContact.GetContact(user.contactId);

                if (contact != null)
                {
                    ddlTeacher.Items.Add(new ListItem(contact.lastName + ", " + contact.firstName, user.contactId.ToString()));
                }
            }
        }

        /*
         * Pre:
         * Post: Determines whether or not the current user's highest permission level is Teacher
         * @returns true if the current user's highest permission level is Teacher and false otherwise
         */
        private bool HighestPermissionTeacher()
        {
            User user = (User)Session[Utility.userRole];
            bool teacherOnly = false;

            if (user.permissionLevel.Contains('T') && !(user.permissionLevel.Contains('D') || user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
            {
                teacherOnly = true;
            }

            return teacherOnly;
        }

        /*
         * Load the awards for the selected event
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlYear.SelectedIndex > 0 && ddlDistrictSearch.SelectedIndex > 0 && ddlAuditionType.SelectedIndex > 0)
            {
                int year = Convert.ToInt32(ddlYear.SelectedValue);
                int districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);
                int teacherId = ddlTeacher.SelectedIndex >= 0 && !ddlTeacher.SelectedValue.Equals("") ? teacherId = Convert.ToInt32(ddlTeacher.SelectedValue) : 0;

                // Update gridview
                DataTable table = DbInterfaceAwards.GetAwardData(year, districtId, teacherId, ddlAuditionType.SelectedValue.Equals("District"));

                if (table != null)
                {
                    gvAuditions.DataSource = table;
                    gvAuditions.DataBind();
                }
                else
                    showErrorMessage("Awards could not be retrieved.");
            }
            else
                showWarningMessage("Please select a year, district, and audition type.");
        }

        protected void ddlDistrictSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HighestPermissionTeacher())
                updateTeacherDropdown();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HighestPermissionTeacher())
                updateTeacherDropdown();
        }

        /*
         * Pre:
         * Post: Update the list of available teachers
         */
        private void updateTeacherDropdown()
        {
            ddlTeacher.DataSource = null;
            ddlTeacher.DataBind();
            ddlTeacher.Items.Clear();

            if (ddlDistrictSearch.SelectedIndex > 0)
            {
                int year = Convert.ToInt32(ddlYear.SelectedValue);
                int districtId = Convert.ToInt32(ddlDistrictSearch.SelectedValue);

                DataTable table = DbInterfaceContact.GetTeachersForEvent(districtId, year);

                if (table != null)
                {
                    //add empty item
                    ddlTeacher.Items.Add(new ListItem("", ""));

                    //add teachers from district
                    ddlTeacher.DataSource = table;

                    ddlTeacher.DataTextField = "ComboName";
                    ddlTeacher.DataValueField = "ContactId";

                    ddlTeacher.DataBind();
                }
                else
                {
                    showErrorMessage("Error: The teachers for the selected event could not be retrieved.");
                }
            }
        }

        #region Messages

        /*
         * Pre:
         * Post: Displays the input error message in the top-left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showErrorMessage(string message)
        {
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowError", "showMainError(" + message + ")", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowMainError", "showMainError(" + message + ")", true);
            lblErrorMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowError", "showMainError()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input warning message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showWarningMessage(string message)
        {
            lblWarningMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowWarning", "showWarningMessage()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input informational message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showInfoMessage(string message)
        {
            lblInfoMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowInfo", "showInfoMessage()", true);
        }

        #endregion
    }
}
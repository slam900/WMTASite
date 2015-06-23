using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace WMTA.Reporting
{
    public partial class TheoryTestReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                checkPermissions();

                loadYearDropdown();
                loadDistrictDropdown();
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
         * Post: If an event matching the search criteria is found, execute
         *       the reports for that audition
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string[] theoryTests = new string[] { "AA", "AB", "EA-alto", "EA-bass", "EA-keybrd", "EA-treble", "EB-alto", "EB-bass", "EB-keybrd", "EB-treble", "EC", "IA", "IB", "IC" };
            ReportViewer[] rptViewers = new ReportViewer[] { rptAA, rptAB, rptEAalto, rptEAbass, rptEAkeybrd, rptEAtreble, rptEBalto, rptEBbass, rptEBkeybrd, rptEBtreble, rptEC, rptIA, rptIB, rptIC };

            Tuple<int, string> orgIdAndSeries = DbInterfaceAudition.GetAuditionOrgIdAndTestSeries(Convert.ToInt32(ddlDistrictSearch.SelectedValue),
                                                                                                  Convert.ToInt32(ddlYear.SelectedValue));

            if (orgIdAndSeries.Item1 != -1 && !orgIdAndSeries.Item2.Equals(""))
            {
                int auditionOrgId = orgIdAndSeries.Item1;
                string testSeries = orgIdAndSeries.Item2;

                showInfoMessage("Please allow several minutes for your reports to generate.");

                for (int i = 0; i < theoryTests.Count(); i++)
                {
                    createReport("Theory_" + testSeries + "_" + theoryTests[i], rptViewers[i], auditionOrgId, testSeries, theoryTests[i]);
                }
            }
            else if (orgIdAndSeries.Item1 == -1)
            {
                showWarningMessage("No audition exists matching the input criteria.");
            }
            else if (orgIdAndSeries.Item2.Equals(""))
            {
                showWarningMessage("The selected audition does not have a theory test series specified.");
            }
        }

        /*
         * Pre:
         * Post: Create the input report in the specified report viewer
         */
        private void createReport(string rptName, ReportViewer rptViewer, int auditionOrgId, string theorySeries, string theoryTest)
        {
            try
            {
                rptViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                rptViewer.ToolBarItemBorderColor = System.Drawing.Color.Black;
                rptViewer.ToolBarItemBorderStyle = BorderStyle.Double;

                rptViewer.ServerReport.ReportServerCredentials = new ReportCredentials(Utility.ssrsUsername, Utility.ssrsPassword, Utility.ssrsDomain);

                rptViewer.ServerReport.ReportServerUrl = new Uri(Utility.ssrsUrl);
                rptViewer.ServerReport.ReportPath = "/wismusta/" + rptName + Utility.reportSuffix;

                //set parameters
                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("auditionOrgId", auditionOrgId.ToString()));
                parameters.Add(new ReportParameter("theorySeries", theorySeries)); 
                parameters.Add(new ReportParameter("theoryTest", theoryTest));

                rptViewer.ServerReport.SetParameters(parameters);

                rptViewer.AsyncRendering = true;
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred while generating reports.");

                Utility.LogError("TheoryTestReports", "createReport", "rptName: " + rptName +
                                 ", auditionOrgId: " + auditionOrgId, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
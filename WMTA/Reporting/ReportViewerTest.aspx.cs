﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace WMTA.Reporting
{
    public partial class ReportViewerTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string username = "wismusta_reportservr";
                string password = "33wi8mu8ta44";
                string domain = "sunflower.arvixe.com";

                rptViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                rptViewer.ToolBarItemBorderColor = System.Drawing.Color.Black;
                rptViewer.ToolBarItemBorderStyle = BorderStyle.Double;

                rptViewer.ServerReport.ReportServerCredentials = new ReportCredentials(username, password, domain);

                rptViewer.ServerReport.ReportServerUrl = new Uri("http://sunflower.arvixe.com/ReportServer_SQL_Service");
                rptViewer.ServerReport.ReportPath = "/wismusta/DistrictAuditionStatistics";

                rptViewer.ServerReport.SetParameters(new ReportParameter("auditionOrgId", "1036"));

                rptViewer.AsyncRendering = true;
            }
        }
    }
}
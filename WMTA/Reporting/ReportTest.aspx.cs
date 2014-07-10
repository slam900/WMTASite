using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMTA.com.arvixe.sunflower;

namespace WMTA.Reporting
{
    public partial class ReportTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportExecutionService rs = new ReportExecutionService();

            //set parameters and report settings
            string reportPath = "/StudentHistory";
            rs.Url = "http://sunflower.arvixe.com/ReportServer_SQL_Service/ReportExecution2005.asmx?wsdl";
            
            DataSourceCredentials credentials = new DataSourceCredentials();
            credentials.DataSourceName = "sunflower.arvixe.com";
            credentials.UserName = "wismusta";
            credentials.Password = "wiMTA2013*";

            rs.SetExecutionCredentials(new DataSourceCredentials[] { credentials });
 
            ExecutionInfo info = new ExecutionInfo();
            ExecutionHeader header = new ExecutionHeader();
            string historyId = "", extension = "", mimeType = "", encoding = "", devInfo = "False";
            string[] streamId;
            Warning[] warning;
            byte[] result;

            ParameterValue param = new ParameterValue();
            param.Name = "@auditionOrgId";
            param.Value = "1036";

            rs.ExecutionHeaderValue = header;
            info = rs.LoadReport(reportPath, historyId);
            rs.SetExecutionParameters(new ParameterValue[] { param }, "en-us");
            result = rs.Render("PDF", devInfo, out extension, out mimeType, out encoding, 
                               out warning, out streamId);

            Response.ClearContent();
            Response.AppendHeader("content-length", result.Length.ToString());
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(result);
            Response.End();
            Response.Flush();
        }
    }
}
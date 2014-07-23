using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Security.Principal;
using System.IO;
using System.Web.Configuration;
using WMTA.com.arvixe.sunflower;
using System.Diagnostics;

namespace WMTA.Reporting
{
    /// <summary>
    /// Summary description for Test
    /// </summary>
    public class Test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            ReportExecutionService rs = new ReportExecutionService();
            ExecutionInfo execInfo = new ExecutionInfo();
            ExecutionHeader execHeader = new ExecutionHeader();
            string historyId = null, encoding = "";
            string reportPath = "/wismusta/DistrictAuditionStatistics";
            //string extension = "PDF";
            //string mimeType = "pdf";
            string extension = "";
            string mimeType = "";
            string devInfo = "False";
            Warning[] warning = null;
            string[] streamId = null;
            byte[] result = null;

            rs.Url = "http://sunflower.arvixe.com/ReportServer_SQL_Service/ReportExecution2005.asmx?wsdl";

            //add credentials
            rs.Credentials = new NetworkCredential("wismusta_reportservr", "33wi8mu8ta44", "sunflower.arvixe.com");

            //add parameters
            ParameterValue[] parameters = new ParameterValue[1];
            parameters[0] = new ParameterValue();
            parameters[0].Name = "auditionOrgId";
            parameters[0].Value = "1036";            

            rs.ExecutionHeaderValue = execHeader;
            execInfo = rs.LoadReport(reportPath, historyId);
            rs.SetExecutionParameters(parameters, "en-us");
            result = rs.Render("PDF", devInfo, out extension, out mimeType, out encoding, out warning, out streamId);
            
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AppendHeader("content-length", result.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Flush();

            //HttpContext.Current.Response.ClearContent();
            //HttpContext.Current.Response.AddHeader("Pragma", "public");
            //HttpContext.Current.Response.AddHeader("Content-Description", "File Transfer");
            //HttpContext.Current.Response.AddHeader("X-Download-Options", "noopen");
            //HttpContext.Current.Response.AddHeader("X-Content-Type-Options", "nosniff");
            //HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
            //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Student_History_Rpt.pdf");
            //HttpContext.Current.Response.AppendHeader("content-length", result.Length.ToString());
            //HttpContext.Current.Response.ContentType("application/excel");
            //HttpContext.Current.Response.BinaryWrite(result);
            //HttpContext.Current.Response.End();
            //HttpContext.Current.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
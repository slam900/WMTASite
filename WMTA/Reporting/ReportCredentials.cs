using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;

namespace WMTA.Reporting
{
    public class ReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        public string username { get; private set; }
        public string password { get; private set; }
        public string domain { get; private set; }

        public ReportCredentials(string username, string password, string domain)
        {
            this.username = username;
            this.password = password;
            this.domain = domain;
        }

        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                return new NetworkCredential(username, password, domain);
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie,
                    out string userName, out string password,
                    out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;

            // Not using form credentials
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class DefaultRedirectErrorPage : System.Web.UI.Page
    {
        protected HttpException ex = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Log the exception 
            ex = new HttpException("defaultRedirect");

            Utility.LogError("DefaultRedirectErrorPage", "", "", ex.Message, -1);
        }
    }
}
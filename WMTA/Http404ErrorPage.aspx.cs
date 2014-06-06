using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class Http404ErrorPage : System.Web.UI.Page
    {
        protected HttpException ex = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Log the exception 
            ex = new HttpException("HTTP 404");
            Utility.LogError("Http404ErrorPage", "", "", ex.Message, -1);
        }
    }
}
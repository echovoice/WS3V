using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Demo;
using System.IO;

namespace Demo
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("http://ws3v.org/implementations");
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!File.Exists(HttpContext.Current.Server.MapPath("~") + Request.RawUrl.Substring(1).Replace("/", "\\")))
            {
                HttpContext.Current.Response.Redirect("http://ws3v.org/implementations");
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}

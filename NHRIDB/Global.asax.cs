using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NHRIDB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;
        }

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			HttpRuntimeSection section = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
			int maxFileSize = section.MaxRequestLength * 1024;
			if (Request.ContentLength > maxFileSize)
			{
				try
				{
					
					Response.Redirect("~/Basic/ErrorSize?size="+ maxFileSize.ToString());
				}
				catch (Exception ex)
				{

				}
			}
		}
	}
}

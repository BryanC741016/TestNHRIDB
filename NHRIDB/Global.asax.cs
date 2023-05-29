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
		void ErrorLog_Filtering(
		object sender,
		Elmah.ExceptionFilterEventArgs e)
		{
			if (e.Exception.GetBaseException() is HttpRequestValidationException) { e.Dismiss(); }
			var httpException = e.Exception as HttpException;
			var test = e.Exception as  System.Data.Entity.Validation.DbEntityValidationException;
			var errorMessage = string.Empty;
			if(test != null)
            {
				test.EntityValidationErrors.ToList().ForEach(e1 => {
					errorMessage = "Table:" + e1.Entry.Entity.GetType().Name;
					e1.ValidationErrors.ToList().ForEach(e2 =>
					{
						errorMessage += " Error:" + e2.ErrorMessage;
						Console.WriteLine(e2.ErrorMessage);
						System.Diagnostics.Debug.WriteLine(errorMessage);
					});
					});

				NHRIDB_DAL.DbModel.NHRIDBEntitiesDB _db = new NHRIDB_DAL.DbModel.NHRIDBEntitiesDB();
                NHRIDB_DAL.DAL.ErrorLogDA _ErrorLogDA = new NHRIDB_DAL.DAL.ErrorLogDA(_db);
				string id = DateTime.Now.ToString("yyyyMMddHHmmss") + Convert.ToString(HttpContext.Current.Session["name"]);
				_ErrorLogDA.Create(id: id, controller: string.Empty, action: string.Empty, message: e.Exception.Message + errorMessage, stacktrace: string.Empty);

			}

			if (httpException != null && httpException.GetHttpCode() == 404)
			{ e.Dismiss(); }
		}
	}
}

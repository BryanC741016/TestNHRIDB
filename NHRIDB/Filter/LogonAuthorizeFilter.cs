using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NHRIDB.Filter
{
    public class LogonAuthorizeFilter : ActionFilterAttribute
    {
        /*
         * 此類別為檢查有無登入
         * */

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext httpcontext = HttpContext.Current;
            // 確認目前要求的HttpSessionState


            //確認Session是否有資料
            if (httpcontext.Session == null || httpcontext.Session.Count == 0 || httpcontext.Session["uid"] == null || httpcontext.Session["hos"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 440;
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    //  RedirectToLoginPage(filterContext);
                    filterContext.HttpContext.Response.Redirect("~/Home/Index");
                }
            }
             

          
               
        

            base.OnActionExecuting(filterContext);

        }
 

      
    }
}
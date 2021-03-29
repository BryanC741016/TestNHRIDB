using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NHRIDB.Filter
{
    public class MvcAdminRightAuthorizeFilter : ActionFilterAttribute
    {
        /*
         * 此類別為檢查有無權限(能否可執行該Action)
         * */
        public char param { get; set; }

        public string exeController;

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //取得要判斷有無權限的action和controller
            if (exeController == null)
                exeController = filterContext.RouteData.Values["Controller"].ToString();

            //取得session
            HttpContext httpcontext = HttpContext.Current;
            dynamic ViewBag = filterContext.Controller.ViewBag;



            //是否可修改資料的預設值

            ViewBag.CanEdit = false;


            if (httpcontext.Session != null && httpcontext.User.Identity.IsAuthenticated)
            {
                List<PurviewModel> funcList = httpcontext.Session["funcList"] as List<PurviewModel>;
                if (funcList != null)
                {
                    if (funcList.Any(z => z.controllName!=null && z.controllName.ToLower().Equals(exeController.ToLower()) && z.purview == PermissionsKind.Write))
                    {

                        ViewBag.CanEdit = true;

                    }
                }



            }


            base.OnResultExecuting(filterContext);

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //取得要判斷有無權限的action和controller
            if (exeController == null)
                exeController = filterContext.RouteData.Values["Controller"].ToString();
        

            // 確認目前要求的HttpSessionState
            HttpContext httpcontext = HttpContext.Current;
                        
            if(httpcontext.Session == null || !httpcontext.User.Identity.IsAuthenticated)
            {
                Logout(filterContext);
            }
            else
            {
                ////依acion向session查詢是否有權限
                List<PurviewModel> funcList = httpcontext.Session["funcList"] as List<PurviewModel>;
                if (funcList == null)
                {
                    Logout(filterContext);
                }
                else {
                    if (!funcList.Any(z => z.controllName.ToLower().Equals(exeController.ToLower())))
                    {
                        ShowRightRs(filterContext);
                    }
                    else {
                        int purview = 0;
                        switch (param) {
                            case 'r':
                                purview = 1;
                                    break;
                            case 'w':
                                purview = 2;
                                break;

                        }//end switch

                        if (!funcList.Any(z => z.controllName.ToLower().Equals(exeController.ToLower()) && ((int)z.purview) >= purview))
                        {
                            ShowRightRs(filterContext);
                        }
                    }

               
                }

             
            }


            base.OnActionExecuting(filterContext);

        }

        //權限不符時
        private void ShowRightRs(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData["message"] = "無權限";
            filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary
                     {
                           { "controller", "Account" },
                           { "action", "NoRight" },
                          { "id", UrlParameter.Optional }
                     });

        }

        private void Logout(ActionExecutingContext filterContext) {
            filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                   {
                           { "controller", "Home" },
                           { "action", "Index" },
                          { "id", UrlParameter.Optional }
                   });
        }
    }
}
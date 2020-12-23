using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NHRIDB.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AjaxValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                //只有Ajax 才處理
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ValidateRequestHeader(filterContext.HttpContext.Request);
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 404;
                    filterContext.Result = new HttpNotFoundResult();
                }
            }
            catch (HttpAntiForgeryException e)
            {
                throw new HttpAntiForgeryException("Anti forgery token cookie not found");
            }
        }

        /// <summary>
        /// 解析前端丟過來的Token 是否正確
        /// </summary>
        /// <param name="request"></param>
        private void ValidateRequestHeader(HttpRequestBase request)
        {
            String cookieToken = String.Empty;
            String formToken = String.Empty;
            String TokenValue = request.Headers["RequestVerificationToken"];

            if (!String.IsNullOrWhiteSpace(TokenValue))
            {
                String[] Tokens = TokenValue.Split(':');
                if (Tokens.Length == 2)
                {
                    cookieToken = Tokens[0];
                    formToken = Tokens[1];
                }
                AntiForgery.Validate(cookieToken, formToken);
            }
        }
    }
}
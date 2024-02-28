using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Fileter
{
    public class Auth : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["AuthCookie"];
            // 로그인 여부 확인
            if (authCookie == null)
            {
                // 로그인되지 않은 경우, 로그인 페이지로 리다이렉션
                filterContext.Result = new RedirectResult("~/Account/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
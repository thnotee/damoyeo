using Damoyeo.Model.Model;
using Damoyeo.Util.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Fileter
{
    public class Auth : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {


            base.OnAuthorization(filterContext);

            if (!this.AuthorizeCore(filterContext.HttpContext))
            {
                //일반회원 or 비밀번호 생성한 카카오회원
                // 로그인되지 않은 경우, 로그인 페이지로 리다이렉션
                string returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl);
                filterContext.Result = new RedirectResult("~/Auth/Login?returnUrl=" + returnUrl);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!UserManager.IsLogin())
            {
                return false;
            }
            return true;

        }

    }
}
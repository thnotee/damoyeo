using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Fileter
{
    public class AuthUserinfoAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {


            base.OnAuthorization(filterContext);

            if (!this.AuthorizeCore(filterContext.HttpContext))
            {
                var userCookie = Damoyeo.Util.Manager.UserManager.GetCookie();
                
                //비밀번호 생성안한 카카오 회원
                if (userCookie.SignupType == "1") 
                {
                    filterContext.Result = new RedirectResult("/Auth/CreatePassword");
                }
                else
                {
                    //일반회원 or 비밀번호 생성한 카카오회원
                    filterContext.Result = new RedirectResult("/Auth/CheckPassword");
                }
                
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            /*
                0 : 일반로그인
                1 : 카카오로그인
                2 : 카카오로그인 + 비밀번호 변경
             */
            var cookieValue = HttpContext.Current.Request.Cookies["UserInfoCookie"]?.Value;
            if (string.IsNullOrEmpty(cookieValue))
            {
                return false;
            }
            return true;

        }
    }
}
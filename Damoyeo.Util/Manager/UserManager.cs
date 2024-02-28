using Damoyeo.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Damoyeo.Util.Manager
{
    public static class UserManager
    {

        public static bool IsLogin() 
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["AuthCookie"];
            if (authCookie == null)
            {
                return false;
            }
            return true;
        }

        public static AuthMdoel GetCookie()
        {
            AuthMdoel authModel = null;
            // 쿠키 가져오기
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["AuthCookie"];

            if (authCookie != null)
            {
                // 쿠키에 값이 있는 경우 AuthModel에 저장
                authModel.UserId = int.Parse(authCookie.Values["UserId"]);
                authModel.Email = authCookie.Values["Email"];
                authModel.ProfileImage = authCookie.Values["ProfileImage"];
                authModel.Slf_Intro = authCookie.Values["Slf_Intro"];
                authModel.Nickname = authCookie.Values["Nickname"];
            }

            return authModel;
        }


    }
}

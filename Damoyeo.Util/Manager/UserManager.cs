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
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["UserCookie"];
            if (authCookie == null)
            {
                return false;
            }
            return true;
        }

        public static AuthMdoel GetCookie()
        {
            AuthMdoel authModel = new AuthMdoel();
            // 쿠키 가져오기
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["UserCookie"];

            if (authCookie == null)
            {
                return null;
            }

            // 쿠키에 값이 있는 경우 AuthModel에 저장
            authModel.UserId = int.Parse(authCookie.Values["user_id"]);
            authModel.Email = authCookie.Values["email"];
            authModel.Nickname = authCookie.Values["nickname"];
            authModel.ProfileImage = authCookie.Values["profile_image"];
            authModel.Slf_Intro = authCookie.Values["slf_Intro"];
            return authModel;

        }


    }
}

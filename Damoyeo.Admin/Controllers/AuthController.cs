using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string email, string password, string returnUrl = "/User/index")
        {

            var user = new DamoyeoUser();
            user.email = email;
            var userObj = await _unitOfWork.Users.GetAsync(user);

            if (userObj != null)
            {
                if (userObj.password == StringUtil.GetSHA256(password) && userObj.signup_type == 3)
                {
                    // 쿠키 생성
                    HttpCookie userCookie = new HttpCookie("AdminCookie");
                    // 값 추가
                    userCookie.Values["user_id"] = HttpUtility.UrlEncode(userObj.user_id.ToString());
                    userCookie.Values["email"] = HttpUtility.UrlEncode(userObj.email);
                    userCookie.Values["nickname"] = HttpUtility.UrlEncode(userObj.nickname);
                    userCookie.Values["profile_image"] = HttpUtility.UrlEncode(userObj.profile_image);
                    userCookie.Values["slf_Intro"] = HttpUtility.UrlEncode(userObj.slf_Intro);
                    userCookie.Values["signup_type"] = HttpUtility.UrlEncode(userObj.signup_type.ToString());


                    // 쿠키 만료 시간 설정
                    userCookie.Expires = DateTime.Now.AddDays(7); // 예를 들어, 7일 후에 만료되도록 설정
                    // 쿠키 추가
                    Response.Cookies.Add(userCookie);
                    return Redirect(returnUrl);

                }
                else
                {
                    TempData["errorMsg"] = "아이디 또는 비밀번호를 확인해주세요.";
                }
            }

            TempData["errorMsg"] = "아이디 또는 비밀번호를 확인해주세요.";
            return View();
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["AdminCookie"] != null)
            {
                HttpCookie userCookie = Request.Cookies["AdminCookie"];
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            return RedirectToAction("Index", "Auth");
        }


    }
}
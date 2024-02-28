using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Damoyeo.Web.Controllers
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

        public async Task<ActionResult> Signup()
        {
            //unitOfWork
            
            var user = new DamoyeoUser
            {
                Email = "test@example.com",
                Password = "password123",
                ProfileImage = "profileImage.jpg",
                Slf_Intro = "안녕하세요, 저는 테스트 유저입니다.",
                Nickname = "TestUser",
                Use_Tf = "1",
                Reg_Date = DateTime.Now
            };

            user.Password = StringUtil.GetSHA256(user.Password);
            await _unitOfWork.Users.AddAsync(user);
            _unitOfWork.Commit();
            return View();
        }

        public async Task<ActionResult> Login(string email, string password)
        {

            password = "password123";

            var user = new DamoyeoUser();
            user.Email = "test@example.com";

            var userObj = await _unitOfWork.Users.GetAsync(user);
           
            if (userObj != null)
            {
                if (userObj.Password == StringUtil.GetSHA256(password)) 
                {
                    // 쿠키 생성
                    HttpCookie userCookie = new HttpCookie("UserCookie");
                    // 값 추가
                    userCookie.Values["user_id"] = userObj.UserId.ToString();
                    userCookie.Values["email"] = userObj.Email;
                    userCookie.Values["nickname"] = userObj.Nickname;
                    userCookie.Values["profile_image"] = userObj.ProfileImage;
                    userCookie.Values["slf_Intro"] = userObj.Slf_Intro;
                    // 쿠키 만료 시간 설정
                    userCookie.Expires = DateTime.Now.AddDays(7); // 예를 들어, 7일 후에 만료되도록 설정

                    // 쿠키 추가
                    Response.Cookies.Add(userCookie);

                }
            }
            
            return View();
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["UserCookie"] != null)
            {
                HttpCookie userCookie = Request.Cookies["UserCookie"];
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            return View();
        }
    }


 

}
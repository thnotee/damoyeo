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
         
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Signup(DamoyeoUser user, HttpPostedFileBase profile_image)
        {
         
            
            if (profile_image != null && profile_image.ContentLength > 0)
            {
                // 파일 이름에 GUID를 추가하여 중복을 방지합니다.
                var fileName = Path.GetFileNameWithoutExtension(user.email)
                               + "_"
                               + Guid.NewGuid().ToString()
                               + Path.GetExtension(profile_image.FileName);

                // 파일을 저장할 경로를 지정합니다.
                var path = Path.Combine(Server.MapPath("~/Content/upload/profile_image"), fileName);

                // 해당 경로에 폴더가 없으면 폴더를 생성합니다.
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 파일을 지정한 경로에 저장합니다.
                profile_image.SaveAs(path);
                user.profile_image = Url.Content("~/Content/upload/profile_image" + fileName);
            }

            
            user.reg_date = DateTime.Now;
            user.use_tf = "1";
            user.password = StringUtil.GetSHA256(user.password);
            await _unitOfWork.Users.AddAsync(user);
            _unitOfWork.Commit();
            return View();
        }


        public ActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {

            password = "password123";

            var user = new DamoyeoUser();
            user.email = "test@example.com";

            var userObj = await _unitOfWork.Users.GetAsync(user);
           
            if (userObj != null)
            {
                if (userObj.password == StringUtil.GetSHA256(password)) 
                {
                    // 쿠키 생성
                    HttpCookie userCookie = new HttpCookie("UserCookie");
                    // 값 추가
                    userCookie.Values["user_id"] = userObj.user_id.ToString();
                    userCookie.Values["email"] = userObj.email;
                    userCookie.Values["nickname"] = userObj.nickname;
                    userCookie.Values["profile_image"] = userObj.profile_image;
                    userCookie.Values["slf_Intro"] = userObj.slf_Intro;
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
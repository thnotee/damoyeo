using Damoyeo.Data.DataAccess;
using Damoyeo.Data.Repository;
using Damoyeo.Data.Repository.IRepository;
using Damoyeo.Model.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
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

        public ActionResult Signup()
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
            _unitOfWork.Users.AddAsync(user);
            //_unitOfWork.Commit
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }


    /*
    public class CookieSerializer
    {
        // 객체를 쿠키에 직렬화하여 저장
        public void SerializeToCookie(object obj, string key)
        {
            HttpCookie encodedCookie = new HttpCookie(key);
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, obj);
            byte[] byteArr = memoryStream.ToArray();
            encodedCookie.Value = Convert.ToBase64String(byteArr);
            HttpContext.Current.Response.Cookies.Add(encodedCookie);
        }

        // 쿠키에서 객체를 역직렬화하여 반환
        public object DeserializeFromCookie(string key)
        {
            HttpCookie encodedCookie = HttpContext.Current.Request.Cookies[key];
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(encodedCookie.Value));
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            return binaryFormatter.Deserialize(memoryStream);
        }
    }
    */


}
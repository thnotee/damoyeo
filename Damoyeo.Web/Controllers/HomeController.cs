using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Controllers
{
    public class HomeController : Controller
    {

 
        public ActionResult Index()
        {

      

            /*

            CookieSerializer cookieSerializer = new CookieSerializer();
            User user = new User { Email = "user@example.com", Nickname = "example" };
            // User 객체를 쿠키에 직렬화하여 저장
            cookieSerializer.SerializeToCookie(user, "UserCookie");
            // 쿠키에서 User 객체를 역직렬화하여 반환
            User deserializedUser = (User)cookieSerializer.DeserializeFromCookie("UserCookie");
            */
            /*
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DamoyeoConnectionString"].ConnectionString)) 
            {
                var sql = @"
SELECT NAME FROM Test
WHERE 
NAME= '이태환'
";
                var result = conn.QueryFirstOrDefault<string>(sql);
                ViewData["test"] = result;
                return View();
            }
            */
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class User
    {

        public string id { get; set; }

        public string email { get; set; }

    }


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



}
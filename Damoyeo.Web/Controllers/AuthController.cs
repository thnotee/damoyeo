using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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


        public ActionResult Signup()
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

            TempData["successMsg"] = user.nickname + "님 회원가입 되었습니다.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login(string returnUrl = "/Home/index")
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password, string returnUrl = "/Home/index")
        {

            var user = new DamoyeoUser();
            user.email = email;
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
            if (Request.Cookies["UserCookie"] != null)
            {
                HttpCookie userCookie = Request.Cookies["UserCookie"];
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// 유튜브 콜백함수
        /// 받은 code를 사용하여 쿠키에 AccessToken을 등록합니다.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ActionResult> CallBack(string code)
        {

            var tokenResponse = await GetAccessTokenAsync(code);
            Response.Cookies["AccessToken"].Value = tokenResponse.access_token;
            Response.Cookies["AccessToken"].Expires = DateTime.Now.AddSeconds(tokenResponse.expires_in);


            return View();
        }


        /// <summary>
        /// access_token 을 받아옵니다.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<TokenResponse> GetAccessTokenAsync(string code)
        {

   

                  using (var httpClient = new HttpClient())
                  {
                      // POST 요청에 필요한 데이터 구성
                      var formData = new List<KeyValuePair<string, string>>
                      {
                          new KeyValuePair<string, string>("grant_type", "authorization_code"),
                          new KeyValuePair<string, string>("client_id", "6fdb7adf5e19747b49b3d6585e26de48" ),
                          new KeyValuePair<string, string>("redirect_uri", "https://localhost:44369/Auth/CallBack"),
                          new KeyValuePair<string, string>("code", code),
                          new KeyValuePair<string, string>("client_secret","62INf9ybK77JZ4NcwV3IMyaLFsOwR7S7")

                      };

                      //https://kauth.kakao.com/oauth/token
                      // HTTP POST 요청 보내기
                      var response = await httpClient.PostAsync("https://kauth.kakao.com/oauth/token", new FormUrlEncodedContent(formData));

                      // 응답 확인
                      response.EnsureSuccessStatusCode();

                      // 응답 데이터를 TokenResponse 클래스로 변환
                      var responseBody = await response.Content.ReadAsStringAsync();
                      var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                      return tokenResponse;
                  }
            
        }

        public class TokenResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }
            public string id_token { get; set; } //디코딩해제
            
        }


    }

}
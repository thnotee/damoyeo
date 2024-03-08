using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
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
using static Damoyeo.Web.Controllers.AuthController;

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
            var user = new DamoyeoUser();
            return View(user);
        }

        public ActionResult KaKaoSignup()
        {
            var user = new DamoyeoUser();
            user.email = Session["email"] as string;
            user.nickname = Session["nickname"] as string;
            user.profile_image = Session["profile_image"] as string;

            Session.Clear();
            if (string.IsNullOrEmpty(user.email)) 
            {
                throw new Exception("카카오 ID NULL");
            }
            
            return View("Signup",user);
        }

        [HttpPost]
        public async Task<ActionResult> Signup(DamoyeoUser user, HttpPostedFileBase profile_image, string kakao_profile_image = "")
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
                user.profile_image = Url.Content("~/Content/upload/profile_image/" + fileName);
            }
            else 
            {
                if (user.signup_type == 1 && !string.IsNullOrEmpty(kakao_profile_image)) 
                {
                    user.profile_image = kakao_profile_image;
                }
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
                    userCookie.Values["signup_type"] = userObj.signup_type.ToString();
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

            var idToken = await GetKakaoIdTokenAsync(code);
            //Response.Cookies["AccessToken"].Value = tokenResponse.access_token;
            //Response.Cookies["AccessToken"].Expires = DateTime.Now.AddSeconds(tokenResponse.expires_in);


            if (idToken != null) {
                var user = new DamoyeoUser();
                user.email = idToken.sub;
                //1.DB_조회 
                var userObj = await _unitOfWork.Users.GetAsync(user);
     
                //3.로그인 처리
                if (userObj != null && userObj.signup_type == 1)
                {
                    HttpCookie userCookie = new HttpCookie("UserCookie");
                    // 값 추가
                    userCookie.Values["user_id"] = userObj.user_id.ToString();
                    userCookie.Values["email"] = userObj.email;
                    userCookie.Values["nickname"] = userObj.nickname;
                    userCookie.Values["profile_image"] = userObj.profile_image;
                    userCookie.Values["slf_Intro"] = userObj.slf_Intro;
                    userCookie.Values["signup_type"] = userObj.signup_type.ToString();
                    // 쿠키 만료 시간 설정
                    userCookie.Expires = DateTime.Now.AddDays(7); // 예를 들어, 7일 후에 만료되도록 설정
                                                                  // 쿠키 추가
                    Response.Cookies.Add(userCookie);
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    // 세션에 값을 추가
                    Session["email"] = idToken.sub;
                    Session["nickname"] = idToken.nickname;
                    Session["profile_image"] = idToken.picture;
                    // 다른 컨트롤러의 메서드로 리다이렉트
                    return RedirectToAction("KaKaoSignup", "Auth");
                }

              
            }
            
            return View();
        }


        /// <summary>
        /// access_token 을 받아옵니다.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<KaKaoIdToken> GetKakaoIdTokenAsync(string code)
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

                //base64 byte 배열 전환 후 디코딩
                var kaKaoIdToken = DecodeJwtToKaKaoIdToken(tokenResponse.id_token);

                return kaKaoIdToken;
            }

        }

        private KaKaoIdToken DecodeJwtToKaKaoIdToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwt) as JwtSecurityToken;

            var idToken = new KaKaoIdToken();
            idToken.iss = jsonToken.Issuer;
            idToken.aud = jsonToken.Audiences.FirstOrDefault();
            idToken.sub = jsonToken.Subject;
            idToken.iat = int.Parse(jsonToken.Claims.FirstOrDefault(c => c.Type == "iat").Value);
            idToken.exp = int.Parse(jsonToken.Claims.FirstOrDefault(c => c.Type == "exp").Value);
            idToken.nickname = jsonToken.Claims.FirstOrDefault(c => c.Type == "nickname").Value;
            idToken.picture = jsonToken.Claims.FirstOrDefault(c => c.Type == "picture").Value;

            return idToken;
        }

        public class TokenResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }
            public string id_token { get; set; }

            public KaKaoIdToken decodeKakaoJWT { get; set; }

        }

        public class KaKaoIdToken
        {
            /// <summary>
            ///  ID 토큰을 발급한 인증 기관 정보
            ///  https://kauth.kakao.com로 고정
            /// </summary>
            public string iss { get; set; }
            /// <summary>
            /// ID 토큰이 발급된 앱의 앱 키
            /// 인가 코드 받기 요청 시 client_id에 전달된 앱 키
            /// </summary>
            public string aud { get; set; }

            /// <summary>
            ///ID 토큰에 해당하는 사용자의 회원번호
            /// </summary>
            public string sub { get; set; }

            /// <summary>
            /// ID 토큰 발급 또는 갱신 시각, UNIX 타임스탬프(Timestamp)
            /// </summary>
            public int iat { get; set; }

            /// <summary>
            ///ID 토큰 만료 시간, UNIX 타임스탬프(Timestamp)
            /// </summary>
            public int exp { get; set; }

            /// <summary>
            /// 닉네임
            /// </summary>
            public string nickname { get; set; }

            /// <summary>
            /// 프로필사진 이미지 
            /// </summary>
            public string picture { get; set; }

        }


    }

}
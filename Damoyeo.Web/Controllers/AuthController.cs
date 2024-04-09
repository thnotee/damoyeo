using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using Damoyeo.Web.Fileter;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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


        public async Task<ActionResult> Signup()
        {
            var user = new DamoyeoUser();
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            return View(user);
        }

        public async Task<ActionResult> KaKaoSignup()
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

            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            return View("Signup",user);
        }
        [HttpPost]
        public async Task<ActionResult> Signup(DamoyeoUser user, List<int> interest)
        {


            user.reg_date = DateTime.Now;
            user.use_tf = "1";
            if (user.signup_type == 0)
            {
                user.password = StringUtil.GetSHA256(user.password);
            }
            else 
            {
                user.password = user.password;
            }
            
            int userId = await _unitOfWork.Users.AddAsync(user);

            foreach (var item in interest) 
            {
                var interestEntity = new DamoyeoUserInterestCategory();
                interestEntity.user_id = userId;
                interestEntity.category_id = item;     
                await _unitOfWork.UserInterestCategory.AddAsync(interestEntity);
            }
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

        public ActionResult FindPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FindPassword(string email = "")
        {

            if (!string.IsNullOrEmpty(email))
            {
                var parameter = new DamoyeoUser();
                parameter.email = email;
                var userInfo = await _unitOfWork.Users.GetAsync(parameter);
                if (userInfo != null)
                {
                    string fromAddress = "thlee5842@gmail.com";
                    string password = "sbyj eoss vubl nbse";

                    // SMTP 서버 설정
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com", // SMTP 서버 호스트명
                        Port = 587, // SMTP 서버 포트 번호
                        EnableSsl = true, // SSL 사용 여부
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress, password)
                    };

                    Guid guid = Guid.NewGuid();
                    string newPassword = guid.ToString().Substring(0, 4);

                    // 이메일 보낼 정보 설정
                    MailAddress from = new MailAddress(fromAddress);
                    MailAddress to = new MailAddress(email);
                    MailMessage message = new MailMessage(from, to)
                    {
                        Subject = "[다모여] 새로운 비밀번호를 전송해드립니다.",
                        Body = $"새로운 비밀번호는 {newPassword} 입니다."
                    };

                    // 이메일 전송
                    try
                    {
                        smtp.Send(message);
                        userInfo.password = StringUtil.GetSHA256(newPassword);
                        await _unitOfWork.Users.UpdateAsync(userInfo);
                        _unitOfWork.Commit();
                        TempData["successMsg"] = "이메일 전송완료!!";
                        return View();

                    }
                    catch (Exception ex)
                    {
                        var dd = ex;
                    }
                }
            }
            TempData["errorMsg"] = "이메일이 존재하지 않습니다!";
            return View();
        }

        [Auth]
        public ActionResult CreatePassword()
        {
            return View();
        }

        [Auth]
        [HttpPost]
        public async Task<ActionResult> CreatePassword(string password)
        {
            if (!string.IsNullOrEmpty(password)) 
            {
                var userCookie = UserManager.GetCookie();
                var userParameter = new DamoyeoUser();
                userParameter.email = userCookie.Email;

                var userObj = await _unitOfWork.Users.GetAsync(userParameter);
                userObj.password = StringUtil.GetSHA256(password);
                userObj.signup_type = 2;


                // 기존 쿠키 가져오기
                HttpCookie UserCookie = HttpContext.Request.Cookies["UserCookie"];
                UserCookie.Values["signup_type"] = "2";
                HttpContext.Response.Cookies.Add(UserCookie);
                

                await _unitOfWork.Users.UpdateAsync(userObj);
                _unitOfWork.Commit();

                return RedirectToAction("CheckPassword");

            }
            return View();
        }


        [Auth]
        public ActionResult CheckPassword()
        {
            return View();
        }

        [Auth]
        [HttpPost]
        public async Task<ActionResult> CheckPassword(string password)
        {

            var userCookie = UserManager.GetCookie();
            var userParameter = new DamoyeoUser();
            userParameter.email = userCookie.Email;
            var userObj = await _unitOfWork.Users.GetAsync(userParameter);
            if (userObj.password == StringUtil.GetSHA256(password))
            {
                HttpCookie userInfoCookie = new HttpCookie("UserInfoCookie");
                userInfoCookie.Expires = DateTime.Now.AddMinutes(30); //10분뒤 쿠키 만료
                userInfoCookie.Value = "1";
                HttpContext.Response.Cookies.Add(userInfoCookie);
                return RedirectToAction("Index", "User");
            }
            else 
            {
                TempData["errorMsg"] = "비밀번호를 확인해주세요.";
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

            return RedirectToAction("Index", "Home");
        }


        /////////////////////////////////////
        ///API CALLS
        /////////////////////////////////////
        public async Task<ActionResult> CheckDuplicateId(string email)
        {
            var user = new DamoyeoUser();
            user.email = email;
            var userObj = await _unitOfWork.Users.GetAsync(user);

            if (userObj == null)
            {
                //사용가능
                return Json(new { seccess = true, data = true });
            }
            else 
            {
                //불가능
                return Json(new { seccess = true, data = false }); ;
            }
        }

        public async Task<ActionResult> CheckDuplicateNickname(string nickname)
        {
            var user = new DamoyeoUser();
            user.nickname = nickname;
            var userObj = await _unitOfWork.Users.GetNicknameAsync(user);

            if (userObj == null)
            {
                //사용가능
                return Json(new { seccess = true, data = true });
            }
            else
            {
                //불가능
                return Json(new { seccess = true, data = false }); ;
            }
        }




        /// <summary>
        /// 카카오 콜백함수
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
                if (userObj != null && userObj.signup_type != 0)
                {
                    HttpCookie userCookie = new HttpCookie("UserCookie");
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
                          new KeyValuePair<string, string>("redirect_uri", StringUtil.GetAppSetting("BaseUrl")+"/Auth/CallBack"),
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
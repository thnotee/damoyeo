using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.Model;
using Damoyeo.Web.Fileter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Util.Manager;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util;
using Microsoft.IdentityModel.Tokens;
using Damoyeo.Model.Model.option;
using System.Globalization;
using System.IO;
using System.Web.UI;
using Damoyeo.Model.Model.Procedure;

namespace Damoyeo.Web.Controllers
{
    
    public class UserController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: User

        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> Index()
        {
            ViewBag.TabIndex = 1;
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            var userCookie = UserManager.GetCookie();
            var userParameter = new DamoyeoUser();
            userParameter.email = userCookie.Email;

            DamoyeoUserInterestCategory userInterestParameter = new DamoyeoUserInterestCategory();
            userInterestParameter.user_id = userCookie.UserId;

            var userTask =  _unitOfWork.Users.GetAsync(userParameter);
            var interestCategoryTask =  _unitOfWork.UserInterestCategory.GetAllAsync(userInterestParameter);

            
            var userInfo = await userTask;
            var interestCategoryList = await interestCategoryTask;

            UserInfoVm viewModel = new UserInfoVm
            {
                userInfo = userInfo,
                interestCategoryList = interestCategoryList
            };

            //GetAllAsync
            ViewData["categoryList"] = categoryList;
            return View(viewModel);
        }

        [Auth]
        [AuthUserinfo]
        [HttpPost]
        public async Task<ActionResult> Index(DamoyeoUser userInfo, List<int> interest, string newPassword)
        {

            ViewBag.TabIndex = 1;
            var userCookie = UserManager.GetCookie();
            var userParameter = new DamoyeoUser();
            userParameter.email = userCookie.Email;

            //회원정보를 가져옵니다.
            var userInfoFromDb = await _unitOfWork.Users.GetAsync(userParameter);

            if (StringUtil.GetSHA256(userInfo.password) != userInfoFromDb.password) 
            {
                TempData["errorMsg"] = "비밀번호가 틀립니다.";
                return RedirectToAction("Index");
            }

            //비밀번호 변경값 존재시 비밀번호 변경
            if (!string.IsNullOrEmpty(newPassword)) 
            {
                userInfoFromDb.password = StringUtil.GetSHA256(newPassword);
            }

            //닉네임 변경시 
            if (!string.IsNullOrEmpty(userInfo.nickname)) 
            {
                userInfoFromDb.nickname = userInfo.nickname;
            }
            
            userInfoFromDb.slf_Intro = userInfo.slf_Intro;
            await _unitOfWork.Users.UpdateAsync(userInfoFromDb);
            //관심 카테고리 삭제
            await _unitOfWork.UserInterestCategory.RemoveAsync(userCookie.UserId);
            //관심카테고리 추가
            if (interest.Any())
            {
                foreach (var item in interest)
                {
                    var interestEntity = new DamoyeoUserInterestCategory();
                    interestEntity.user_id = userCookie.UserId;
                    interestEntity.category_id = item;
                    await _unitOfWork.UserInterestCategory.AddAsync(interestEntity);
                }

            }
            _unitOfWork.Commit();

            TempData["successMsg"] = "성공했습니다.";
            return RedirectToAction("Index");
        }

        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> Opening()
        {
            ViewBag.TabIndex = 2;
            MeetupListVm viewModel = new MeetupListVm();

            viewModel.MeetupSearchOpt = new MeetupSearchOpt();
            viewModel.MeetupSearchOpt.userId = UserManager.GetCookie().UserId;

            viewModel.list = await _unitOfWork.Meetup.GetPagedListAsync(1, 100, viewModel.MeetupSearchOpt);
            viewModel.categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            return View(viewModel);
            
        }

        /// <summary>
        /// 신청자 상세
        /// </summary>
        /// <returns></returns>
        
        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> OpeningDetail(int meetup_id)
        {
            ViewBag.TabIndex = 2;
            DamoyeoApplications parameter = new DamoyeoApplications();
            parameter.meetup_id = meetup_id;
            IEnumerable<DamoyeoApplications> viewModel = await _unitOfWork.Applications.GetAllAsync(parameter);

            return View(viewModel);
        }
        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> Join()
        {
            ViewBag.TabIndex = 3;
            DamoyeoApplications parameter = new DamoyeoApplications();
            parameter.user_id = UserManager.GetCookie().UserId;

            IEnumerable<DamoyeoApplications> viewModel =  await _unitOfWork.Applications.GetAllAsync(parameter);

            //GetAllAsync
            return View(viewModel);

        }

        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> Wish()
        {
            ViewBag.TabIndex = 4;
            MeetupListVm viewModel = new MeetupListVm();

            viewModel.MeetupSearchOpt = new MeetupSearchOpt();
            viewModel.MeetupSearchOpt.userId = UserManager.GetCookie().UserId;

            viewModel.list = await _unitOfWork.Meetup.GetPagedListAsync(1, 100, viewModel.MeetupSearchOpt);
            viewModel.categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            return View(viewModel);
        }

        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> Post(int page =1)
        {

            ViewBag.TabIndex = 5;
            var entity = new CommunitySearchOpt();
            entity.user_id = UserManager.GetCookie().UserId;
            PagedList<DamoyeoCommunity> list = await _unitOfWork.Community.GetPagedListAsync(page, 10, entity);
            list.pagerOptions.Path = "/User/Post";
            return View(list);
        }

        [Auth]
        [AuthUserinfo]
        public async Task<ActionResult> Comment(int page = 1)
        {

            ViewBag.TabIndex = 5;
            var entity = new CommunitySearchOpt();
            entity.user_id = UserManager.GetCookie().UserId;
            PagedList<GetCommentTree> list = await _unitOfWork.CommunityComment.GetUserCommentPagedListAsync(page, 10, entity);
            list.pagerOptions.Path = "/User/Comment";
            return View(list);
        }






        /////////////////////
        /// API CALL
        /////////////////////

        /// <summary>
        /// 회원정보를 가져옵니다.
        /// </summary>
        /// <returns></returns>


        [Auth]
        [HttpPost]
        public async Task<ActionResult> GetUserInfo(string email)
        {
            if (string.IsNullOrEmpty(email)) 
            {
                return Json(new { success = false });
            }

            var userParameter = new DamoyeoUser();
            userParameter.email = email;

            //회원정보를 가져옵니다.
            var userInfoFromDb = await _unitOfWork.Users.GetAsync(userParameter);

            var interestCategoryParameter = new DamoyeoUserInterestCategory();
            interestCategoryParameter.user_id = userInfoFromDb.user_id;  
            var interestCategoryFromDb = await _unitOfWork.UserInterestCategory.GetAsync(interestCategoryParameter);
            
            var returnDict = new Dictionary<string, object>();
            returnDict.Add("userInfo", userInfoFromDb);
            returnDict.Add("interestCategory", interestCategoryFromDb);
            return Json(new { success = true, data = returnDict });
        }


        [Auth]
        [HttpPost]
        public async Task<ActionResult> ChangeProfileImage(HttpPostedFileBase file)
        {
            if (!UserManager.ConfirmationPw()) 
            {
                return Json(new { success = false, data = "비밀번호를 다시 한번 입력해 주세요." });
            }

            var userCookie = UserManager.GetCookie();
            var userParameter = new DamoyeoUser();
            userParameter.email = userCookie.Email;
            var user  = await _unitOfWork.Users.GetAsync(userParameter);

            var savePath = "/Content/upload/profile_image";

            if (file != null)
            {
                string originFileName = Path.GetFileName(file.FileName);

               
                var savefileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(file.FileName);

                // 파일을 저장할 경로를 지정
                var path = Path.Combine(Server.MapPath(savePath), savefileName);

                // 해당 경로에 폴더가 없으면 폴더를 생성합니다.
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                file.SaveAs(path);
                var finalPath = savePath + "/" + savefileName;
                user.profile_image = finalPath;

                // db 저장
                await _unitOfWork.Users.UpdateAsync(user);
                _unitOfWork.Commit();

                // 기존 쿠키 수정
                HttpCookie UserCookie = HttpContext.Request.Cookies["UserCookie"];
                UserCookie.Values["profile_image"] = finalPath;
                HttpContext.Response.Cookies.Add(UserCookie);

                return Json(new { success = true, data = finalPath });
            }

            return Json(new { success = false, data = "관리자에게 문의해주세요."});
        }


        [HttpPost]
        public async Task<ActionResult> UpsertWish(int meetup_id)
        {
            if (UserManager.IsLogin())
            {
                var entity = new DamoyeoWishlist();
                entity.meetup_id = meetup_id;
                entity.user_id = UserManager.GetCookie().UserId;
                entity.wish_date = DateTime.Now;
                var data = await _unitOfWork.Wishlist.GetAsync(entity);
                if (data == null)
                {
                    await _unitOfWork.Wishlist.AddAsync(entity);
                    _unitOfWork.Commit();
                    return Json(new { success = true, code = 1 }); //추가
                }
                else
                {
                    await _unitOfWork.Wishlist.RemoveAsync(data.wish_id);
                    _unitOfWork.Commit();
                    return Json(new { success = true, code = 2 }); //삭제
                }

            }
            else
            {
                return Json(new { success = false, code = 3 });
            }

        }






    }
}
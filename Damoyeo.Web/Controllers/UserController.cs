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

namespace Damoyeo.Web.Controllers
{
    [Auth]
    [AuthUserinfo]
    public class UserController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: User
        public async Task<ActionResult> Index()
        {
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


        [HttpPost]
        public async Task<ActionResult> Index(DamoyeoUser userInfo, List<int> interest, string newPassword)
        {
            

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


        public async Task<ActionResult> Opening()
        {
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
        public async Task<ActionResult> OpeningDetail(int meetup_id)
        {

            DamoyeoApplications parameter = new DamoyeoApplications();

            parameter.meetup_id = meetup_id;
            IEnumerable<DamoyeoApplications> viewModel = await _unitOfWork.Applications.GetAllAsync(parameter);

            return View(viewModel);
        }

        public async Task<ActionResult> Join()
        {

            DamoyeoApplications parameter = new DamoyeoApplications();
            parameter.user_id = UserManager.GetCookie().UserId;

            IEnumerable<DamoyeoApplications> viewModel =  await _unitOfWork.Applications.GetAllAsync(parameter);

            //GetAllAsync
            return View(viewModel);

        }


        public ActionResult Wish()
        {

            return View();
        }


        /////////////////////
        /// API CALL
        /////////////////////

        /// <summary>
        /// 회원정보를 가져옵니다.
        /// </summary>
        /// <returns></returns>
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

    }
}
using Damoyeo.Admin.Filter;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Damoyeo.Admin.Controllers
{
    [Auth]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult> Index(int page = 1, string searchString = "")
        {
            PagedList<DamoyeoUser> userList =  await _unitOfWork.Users.GetUserPagedListAsync(page,10, searchString);
            userList.pagerOptions.Path = "/User/Index";
            userList.pagerOptions.AddQueryString = "searchString=" + searchString;
            return View(userList);
        }

        [HttpPost]
        public async Task<ActionResult> GetUserInfo(string email = "")
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false });
            }

            var userParameter = new DamoyeoUser();
            List<string> categoryList = new List<string>();
            userParameter.email = email;

            //회원정보를 가져옵니다.
            var userInfoFromDb = await _unitOfWork.Users.GetAsync(userParameter);

            var interestCategoryParameter = new DamoyeoUserInterestCategory();
            interestCategoryParameter.user_id = userInfoFromDb.user_id;
            IEnumerable<DamoyeoUserInterestCategory> interestCategoryFromDb = await _unitOfWork.UserInterestCategory.GetAllAsync(interestCategoryParameter);
            if (interestCategoryFromDb.Any())
            {
                categoryList = interestCategoryFromDb.Select(x => x.Category.category_name).ToList();
            }


            var returnDict = new Dictionary<string, object>();

            userInfoFromDb.password = "";
            if (!string.IsNullOrEmpty(userInfoFromDb.profile_image)) 
            {
                userInfoFromDb.profile_image = StringUtil.ReplaceFilePath(userInfoFromDb.profile_image);
            }

            returnDict.Add("userInfo", userInfoFromDb);
            returnDict.Add("interestCategory", categoryList);
            return Json(new { success = true, data = returnDict });

        }

        
        /// <summary>
        /// 회원정지
        /// </summary>
        /// <param name="email"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateUserState(string email = "")
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false });
            }

            var userParameter = new DamoyeoUser();
            List<string> categoryList = new List<string>();
            userParameter.email = email;

            //회원정보를 가져옵니다.
            var userInfoFromDb = await _unitOfWork.Users.GetAsync(userParameter);
            var check = 1;
            if (userInfoFromDb.stop_tf == "0")
            {
                userInfoFromDb.stop_tf = "1"; //정지
            }
            else 
            {
                check = 0;
                userInfoFromDb.stop_tf = "0"; //정지해제
            }

            await _unitOfWork.Users.UpdateAsync(userInfoFromDb);
            _unitOfWork.Commit();
        
            return Json(new { success = true, data = check });

        }

    }
}
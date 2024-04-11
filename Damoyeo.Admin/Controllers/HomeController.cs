using Damoyeo.Admin.Filter;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Damoyeo.Admin.Controllers
{
    [Auth]
    public class HomeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ActionResult> Index()
        {
            ViewBag.TabIndex = 5;
            DamoyeoUser user = new DamoyeoUser();
            user.email = AdminManager.GetCookie().Email;
            DamoyeoUser userData = await _unitOfWork.Users.GetAsync(user);

            PagedList<DamoyeoUser> userList = await _unitOfWork.Users.GetUserPagedListAsync(1, 10,"",1);

            ViewData["userList"] = userList;
            return View(userData);
        }

        
        [HttpPost]
        public async Task<ActionResult> Index(DamoyeoUser userInfo, string newPassword)
        {

            ViewBag.TabIndex = 1;
            var userCookie = AdminManager.GetCookie();
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
      
            _unitOfWork.Commit();

            TempData["successMsg"] = "성공했습니다.";
            return RedirectToAction("Index");
        }
    }
}
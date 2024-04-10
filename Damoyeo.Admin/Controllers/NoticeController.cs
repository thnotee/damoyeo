using Damoyeo.Admin.Filter;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Admin.Controllers
{
    [Auth]
    public class NoticeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NoticeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            return View();
        }

   
        public async Task<ActionResult> Detail(int board_id = 0)
        {
            CommunityDetailVm viewModel = new CommunityDetailVm();

            DamoyeoCommunity entity = new DamoyeoCommunity();
            entity.board_id = board_id;
            viewModel.detail = await _unitOfWork.Community.GetAsync(entity);
            if (viewModel.detail == null)
            {
                viewModel.detail = entity;
            }
            return View(viewModel);
        }


        [HttpPost]
        [Auth]
        [ValidateInput(false)]
        public async Task<ActionResult> Detail(DamoyeoCommunity entity)
        {
            entity.title = Server.HtmlEncode(entity.title);
            entity.content = Server.HtmlEncode(entity.content);
            var cookieData = UserManager.GetCookie();
            entity.use_tf = "1";
            entity.user_id = cookieData.UserId;
            entity.post_date = DateTime.Now;
            await _unitOfWork.Community.AddAsync(entity);
            _unitOfWork.Commit();


            TempData["successMsg"] = "게시글 등록 성공!!";

            return RedirectToAction("Index");
        }
    }
}
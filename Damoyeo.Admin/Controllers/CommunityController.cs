using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
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
    public class CommunityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult> Index(int page = 1, string searchString = "")
        {

            var entity = new CommunitySearchOpt();
            entity.searchString = searchString;
            PagedList<DamoyeoCommunity> list = await _unitOfWork.Community.GetPagedListAsync(page, 10, entity);
            list.pagerOptions.Path = "/Community/Index";
            list.pagerOptions.AddQueryString = "searchString=" + searchString;
            return View(list);
        }


        public async Task<ActionResult> Remove(int board_id = 0)
        {
            DamoyeoCommunity entity = new DamoyeoCommunity();
            entity.board_id = board_id;
            var data = await _unitOfWork.Community.GetAsync(entity);
            await _unitOfWork.Community.RemoveAsync(board_id);
            _unitOfWork.Commit();
            TempData["successMsg"] = "게시글 삭제 성공!!";
            return RedirectToAction("Index");

        }
    }
}
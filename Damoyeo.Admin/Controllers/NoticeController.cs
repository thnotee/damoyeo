using Damoyeo.Admin.Filter;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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

        public async Task<ActionResult> Index(int page = 1, string searchString = "")
        {
            ViewBag.TabIndex = 4;
            var entity = new CommunitySearchOpt();
            entity.searchString = searchString;
            PagedList<DamoyeoNotice> list = await _unitOfWork.Notice.GetPagedListAsync(page, 10, entity);
            list.pagerOptions.Path = "/Notice/Index";
            list.pagerOptions.AddQueryString = "searchString=" + searchString;
            return View(list);
        }


        public async Task<ActionResult> Detail(int board_id = 0)
        {

            ViewBag.TabIndex = 4;
            DamoyeoNotice entity = new DamoyeoNotice();
            entity.board_id = board_id;
            var detail = await _unitOfWork.Notice.GetAsync(entity);
            if (detail == null)
            {
                detail = entity;
            }
            return View(detail);
        }


        [HttpPost]
        [Auth]
        [ValidateInput(false)]
        public async Task<ActionResult> Detail(DamoyeoNotice entity)
        {
            if (entity.board_id == 0)
            {
                entity.title = Server.HtmlEncode(entity.title);
                entity.content = Server.HtmlEncode(entity.content);
                var cookieData = AdminManager.GetCookie();
                entity.use_tf = "1";
                entity.user_id = cookieData.UserId;
                entity.post_date = DateTime.Now;
                entity.view_count = 0;
                await _unitOfWork.Notice.AddAsync(entity);

                _unitOfWork.Commit();

                TempData["successMsg"] = "게시글 등록 성공!!";
            }
            else 
            {
                //업데이트로직
                var data = await _unitOfWork.Notice.GetAsync(entity);
                if (data != null) 
                {
                    data.title = Server.HtmlEncode(data.title);
                    data.content = Server.HtmlEncode(data.content);
                    await _unitOfWork.Notice.UpdateAsync(data);
                    _unitOfWork.Commit();

                    TempData["successMsg"] = "게시글 수정 성공!!";
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Remove(int board_id = 0)
        {
            var entity = new DamoyeoNotice();
            entity.board_id = board_id;
            var data = await _unitOfWork.Notice.GetAsync(entity);
            if (data != null)
            {
                await _unitOfWork.Notice.RemoveAsync(board_id);
                _unitOfWork.Commit();
                TempData["successMsg"] = "게시글 삭제 성공!!";
            }
           
            return RedirectToAction("Index");

        }
    }
}
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
    
    public class NoticeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public NoticeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: User
        public async Task<ActionResult> Index(int page=1, string searchString = "")
        {
            var entity = new CommunitySearchOpt();
            entity.searchString = searchString;
            PagedList<DamoyeoNotice> list = await _unitOfWork.Notice.GetPagedListAsync(page, 10, entity);
            list.pagerOptions.Path = "/Notice/Index";
            list.pagerOptions.AddQueryString = "searchString=" + searchString;
            return View(list);
            
        }

        public async Task<ActionResult> Detail(int board_id)
        {

            DamoyeoNotice entity = new DamoyeoNotice();
            entity.board_id = board_id;
            var detail = await _unitOfWork.Notice.GetAsync(entity);
            if (detail == null)
            {
                detail = entity;
            }
            else 
            {
                detail.view_count += 1;
                await _unitOfWork.Notice.UpdateAsync(detail);
                _unitOfWork.Commit();
            }

            return View(detail);
        }

    }
}
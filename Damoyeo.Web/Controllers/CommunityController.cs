using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Util.Manager;
using Damoyeo.Web.Fileter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Controllers
{
    public class CommunityController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CommunityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Community
        public async Task<ActionResult> Index(int page = 1, string searchString = "")
        {
             PagedList<DamoyeoCommunity> list = await _unitOfWork.Community.GetPagedListAsync(page, 10, searchString);
            return View(list);
        }

        [Auth]
        public ActionResult Write()
        {
            return View();
        }

        [HttpPost]
        [Auth]
        [ValidateInput(false)] 
        public async Task<ActionResult> Write(DamoyeoCommunity enrity)
        {
            enrity.title = Server.HtmlEncode(enrity.title);
            enrity.content = Server.HtmlEncode(enrity.content);
            var cookieData = UserManager.GetCookie();
            enrity.use_tf = "1";
            enrity.user_id = cookieData.UserId;
            enrity.post_date = DateTime.Now;
            await _unitOfWork.Community.AddAsync(enrity);
            _unitOfWork.Commit();
            return View();
        }

    }
}
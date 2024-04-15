using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util.Manager;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Damoyeo.Model.Model.Pager;
using System.Web.UI;

namespace Damoyeo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ActionResult> Index()
        {
            MainVm viewModel = new MainVm();

            viewModel.MeetupSearchOpt = new MeetupSearchOpt();
            viewModel.MeetupSearchOpt.applicationSdate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture); 
            viewModel.MeetupSearchOpt.searchOrder = 1; //최신순
            //최신순 소모임 리스트 가져오기
            viewModel.latestList = await _unitOfWork.Meetup.GetPagedListAsync(1, 3, viewModel.MeetupSearchOpt);
            viewModel.MeetupSearchOpt.searchOrder = 2;

            //인기순 소모임 리스트 가져오기 
            var popularityListTask = _unitOfWork.Meetup.GetPagedListAsync(1, 4, viewModel.MeetupSearchOpt);
            var categoryTask =  _unitOfWork.Category.GetPagedListAsync(1, 10);

            //커뮤니티 리스트 가져오기
            var communityentity = new CommunitySearchOpt();
            var communityTask = _unitOfWork.Community.GetPagedListAsync(1, 3, communityentity);

            //공지 리스트 가져오기
            var noticeSearchOpt = new CommunitySearchOpt();
            noticeSearchOpt.searchString = "";
            var noticeTask =  _unitOfWork.Notice.GetPagedListAsync(1, 1, noticeSearchOpt);

            //동시에 시작 및 작업 할당
            await Task.WhenAll(popularityListTask, categoryTask, communityTask, noticeTask);
            viewModel.popularityList = await popularityListTask;
            viewModel.categoryList = await categoryTask;
            viewModel.communityList = await communityTask;
            viewModel.noticeList = await noticeTask;

            //로그인 되어있으면 나의 WISH 리스트도 가져온다
            if (UserManager.IsLogin())
            {
                var entity = new DamoyeoWishlist();
                entity.user_id = UserManager.GetCookie().UserId;
                viewModel.WishList =  await _unitOfWork.Wishlist.GetAllAsync(entity);
            }

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

  
}
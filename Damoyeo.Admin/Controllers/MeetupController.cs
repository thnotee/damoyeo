using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util;
using Damoyeo.Util.Manager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Admin.Controllers
{
    public class MeetupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeetupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ActionResult> Index(int page = 1, string applicationSdate = "", string searchString = ""
             , string searchArea = "", int searchCategory = 0, int searchOrder = 1)
        {

            MeetupListVm viewModel = new MeetupListVm();

            if (string.IsNullOrEmpty(applicationSdate))
            {
                DateTime now = DateTime.Now; // 현재 날짜와 시간
                DateTime oneWeekAgo = now; // 일주일 전
                applicationSdate = oneWeekAgo.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            }
            viewModel.MeetupSearchOpt = new MeetupSearchOpt();
            //viewModel.MeetupSearchOpt.applicationSdate = applicationSdate;
            viewModel.MeetupSearchOpt.searchString = searchString;
            viewModel.MeetupSearchOpt.searchArea = searchArea;
            viewModel.MeetupSearchOpt.searchCategory = searchCategory;
            viewModel.MeetupSearchOpt.searchOrder = searchOrder;



            viewModel.list = await _unitOfWork.Meetup.GetPagedListAsync(1, 10, viewModel.MeetupSearchOpt);
            viewModel.categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            if (UserManager.IsLogin())
            {
                var entity = new DamoyeoWishlist();
                entity.user_id = UserManager.GetCookie().UserId;
                viewModel.WishList = await _unitOfWork.Wishlist.GetAllAsync(entity);
            }

            viewModel.list.pagerOptions.Path = "/Meetup/Index";
            viewModel.list.pagerOptions.AddQueryString = "searchString=" + searchString;

            return View(viewModel);
        }

        public async Task<ActionResult> Detail(DamoyeoMeetup entity)
        {
            MeetupDetailVm meetupDetailVm = new MeetupDetailVm();


            ///비동기 작업 시작
            var detailTask = _unitOfWork.Meetup.GetAsync(entity);
            var applicationEntity = new DamoyeoApplications { meetup_id = entity.meetup_id };
            var applicationsTask = _unitOfWork.Applications.GetAllAsync(applicationEntity);
            var imageParameta = new DamoyeoImage { table_name = "Damoyeo_Meetup", table_id = entity.meetup_id };
            var imagesTask = _unitOfWork.Image.GetAllAsync(imageParameta);
            var meetupTagsMappingParameta = new DamoyeoMeetupTags { meetup_id = entity.meetup_id };
            var tagsTask = _unitOfWork.MeetupTagsMapping.GetAllAsync(meetupTagsMappingParameta);

            await Task.WhenAll(detailTask, applicationsTask, imagesTask, tagsTask);


            meetupDetailVm.detail = await detailTask;
            meetupDetailVm.applicationList = await applicationsTask;
            meetupDetailVm.image = await imagesTask;
            meetupDetailVm.Tags = await tagsTask;
            ///비동기 작업 끝

            return View(meetupDetailVm);
        }

    }
}
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Controllers
{
    public class MeetupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeetupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Meetup
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Write()
        {
           PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            ViewData["categoryList"]  = categoryList;


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Write(DamoyeoMeetup meetup, HttpPostedFileBase main_image, IEnumerable<HttpPostedFileBase> files)
        {
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            ViewData["categoryList"] = categoryList;

            
            return View();
        }

        /////////////
        ///API CALLS
        ////////////


        public async Task<ActionResult> SearchLocationDataAsunc(string searchData) 
        {

            // 요청할 URL
            string url = "https://api.vworld.kr/req/data?service=data&request=GetFeature&data=LT_C_ADEMD_INFO&key=E9B5C916-7952-30C4-810A-E79B531155F7&domain=https://localhost:44369&attrFilter=emd_kor_nm:=:수택동";


            try {
                using (var client = new HttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync(); // 응답 본문을 읽기
              

                    return Json(new { seccess = true, data = responseBody }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new { seccess = false, data = "" }, JsonRequestBehavior.AllowGet);
            }
       
          
        }


    }
}
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Util.Manager;
using Damoyeo.Web.Fileter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

        [Auth]
        public async Task<ActionResult> Write()
        {
           PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            ViewData["categoryList"]  = categoryList;

            
            return View();
        }

        [HttpPost]
        [Auth]
        public async Task<ActionResult> Write(DamoyeoMeetup meetup, HttpPostedFileBase main_image, IEnumerable<HttpPostedFileBase> files)
        {
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            meetup.reg_date = DateTime.Now;
            meetup.use_tf = 1;
            meetup.meetup_master_id = UserManager.GetCookie().UserId;
            var savePath = "~/Content/upload/meetup";
            
            //메인이미지가 존재하면 파일 업로드 해준다.
            if (main_image != null) 
            {
                meetup.meetup_image = FileUpload(main_image, savePath);
            }
            
            
            //await _unitOfWork.Meetup.AddAsync(meetup);
            //_unitOfWork.Commit();




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




        /// <summary>
        /// 지정된 경로에 file upload를 합니다.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string FileUpload(HttpPostedFileBase file, string savePath, string tableSaveName = "", int tableId = 0)
        {

            // 파일 이름에 GUID를 추가하여 중복을 방지합니다.
            var fileName = Guid.NewGuid().ToString()
                           + Path.GetExtension(file.FileName);

            //~/Content/upload/meetup
            // 파일을 저장할 경로를 지정합니다.
            var path = Path.Combine(Server.MapPath(savePath), fileName);
            
            // 해당 경로에 폴더가 없으면 폴더를 생성합니다.
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 파일을 지정한 경로에 저장합니다.
            file.SaveAs(path);
            if (string.IsNullOrEmpty(tableSaveName)) 
            {
             /*
                DamoyeoImage image = new DamoyeoImage();
                image.directory_path = @"\" + imgPath + @"\";
                image.origin_filename = originFileName;
                image.save_filename = savefileName;
                image.table_name = tableSaveName;
                image.ta = product.Id; // SQL SERVER ID값 먼저 가져오는방법
                await _unitOfWork.Image.AddAsync(image);*/

            }

            return Url.Content(savePath + "/" + fileName);
        }
        
    }
}
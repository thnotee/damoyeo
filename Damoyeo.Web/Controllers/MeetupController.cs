using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Util.Manager;
using Damoyeo.Web.Fileter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Damoyeo.Model.ViewModel;
using System.Globalization;
using Damoyeo.DataAccess.Repository;

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
        public async Task<ActionResult> Index(int page =1, string applicationSdate ="", string applicationEdate = "", string searchString = ""
            , string searchArea = "", int searchCategory = 0, int searchOrder = 1 )
        {

            MeetupListVm viewModel = new MeetupListVm();

            if (applicationSdate == "" || applicationEdate == "") 
            {
                DateTime now = DateTime.Now; // 현재 날짜와 시간
                //DateTime oneWeekAgo = now.AddDays(-7); // 일주일 전
                DateTime oneWeekAgo = now; // 일주일 전
                DateTime oneWeekLater = now.AddDays(7); // 일주일 후
                applicationSdate = oneWeekAgo.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                applicationEdate = oneWeekLater.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            }
            viewModel.MeetupSearchOpt = new MeetupSearchOpt();
            viewModel.MeetupSearchOpt.applicationSdate = applicationSdate;
            viewModel.MeetupSearchOpt.applicationEdate = applicationEdate;
            viewModel.MeetupSearchOpt.searchString = searchString;
            viewModel.MeetupSearchOpt.searchArea = searchArea;
            viewModel.MeetupSearchOpt.searchCategory = searchCategory;
            viewModel.MeetupSearchOpt.searchOrder = searchOrder;



            viewModel.list = await _unitOfWork.Meetup.GetPagedListAsync(1, 100, viewModel.MeetupSearchOpt);
            viewModel.categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            
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






        [Auth]
        public async Task<ActionResult> Write()
        {
           PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            ViewData["categoryList"]  = categoryList;

            
            return View();
        }

        [HttpPost]
        [Auth]
        public async Task<ActionResult> Write(DamoyeoMeetup meetup, HttpPostedFileBase main_image, IEnumerable<HttpPostedFileBase> files, List<string> tags)
        {
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            meetup.reg_date = DateTime.Now;
            meetup.use_tf = 1;
            meetup.meetup_master_id = UserManager.GetCookie().UserId;
            var savePath = "/Content/upload/meetup";
            
            //메인이미지가 존재하면 파일 업로드 해준다.
            if (main_image != null) 
            {
                meetup.meetup_image = await FileUpload(main_image, savePath);
            }
            
            var insertId = await _unitOfWork.Meetup.AddAsync(meetup);

            foreach (var item in files) 
            {
                if (item != null) 
                {
                    await FileUpload(item, savePath, "Damoyeo_Meetup", insertId);
                }   
            }

            /*태그 처리*/
            if (tags != null) 
            {
                foreach (var tagText in tags) 
                {
                    var tag = new DamoyeoTags();
                    tag.tag_name = tagText;
                    var result = await _unitOfWork.Tags.GetAsync(tag);
                    var tagId = 0;
                    //1. 태그아이디를 가져옵니다.
                    if (result == null) { tagId = await _unitOfWork.Tags.AddAsync(tag); }
                    else  { tagId = result.tag_id; }

                    if (tagId > 0) 
                    {
                        var mapping = new DamoyeoMeetupTags();
                        mapping.tag_id = tagId;
                        mapping.meetup_id = insertId;
                        //2. 태그 매핑테이블에 추가
                        await _unitOfWork.MeetupTagsMapping.AddAsync(mapping);
                    }
                }
            }

            _unitOfWork.Commit();


            return View();
        }

        
        [Auth]
        public async Task<ActionResult> Edit(int meetup_id)
        {
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            DamoyeoMeetup parameter = new DamoyeoMeetup();
            parameter.meetup_id = meetup_id;
            
            MeetupDetailVm meetupDetailVm = new MeetupDetailVm();
            ///비동기 작업 시작
            var detailTask = _unitOfWork.Meetup.GetAsync(parameter);
            var applicationEntity = new DamoyeoApplications { meetup_id = parameter.meetup_id };
            var applicationsTask = _unitOfWork.Applications.GetAllAsync(applicationEntity);
            var imageParameta = new DamoyeoImage { table_name = "Damoyeo_Meetup", table_id = parameter.meetup_id };
            var imagesTask = _unitOfWork.Image.GetAllAsync(imageParameta);
            var meetupTagsMappingParameta = new DamoyeoMeetupTags { meetup_id = parameter.meetup_id };
            var tagsTask = _unitOfWork.MeetupTagsMapping.GetAllAsync(meetupTagsMappingParameta);

            await Task.WhenAll(detailTask, applicationsTask, imagesTask, tagsTask);


            meetupDetailVm.detail = await detailTask;
            meetupDetailVm.applicationList = await applicationsTask;
            meetupDetailVm.image = await imagesTask;
            meetupDetailVm.Tags = await tagsTask;

            return View(meetupDetailVm);
        }

        [HttpPost]
        [Auth]
        public async Task<ActionResult> Edit(DamoyeoMeetup meetup, HttpPostedFileBase main_image, IEnumerable<HttpPostedFileBase> files, List<string> tags)
        {
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            meetup.reg_date = DateTime.Now;
            meetup.use_tf = 1;
            meetup.meetup_master_id = UserManager.GetCookie().UserId;
            var savePath = "/Content/upload/meetup";

            //메인이미지가 존재하면 파일 업로드 해준다.
            if (main_image != null)
            {
                meetup.meetup_image = await FileUpload(main_image, savePath);
            }

            await _unitOfWork.Meetup.UpdateAsync(meetup);

            //DB에서 이미지삭제
            var imageParameta = new DamoyeoImage { table_name = "Damoyeo_Meetup", table_id = meetup.meetup_id };
            await _unitOfWork.Image.RemoveTableIdAsync(imageParameta);
            foreach (var item in files)
            {
                if (item != null)
                {
                    await FileUpload(item, savePath, "Damoyeo_Meetup", meetup.meetup_id);
                }
            }


            await _unitOfWork.MeetupTagsMapping.RemoveAsync(meetup.meetup_id);
            /*태그 처리*/
            if (tags != null)
            {
                foreach (var tagText in tags)
                {
                    var tag = new DamoyeoTags();
                    tag.tag_name = tagText;
                    var result = await _unitOfWork.Tags.GetAsync(tag);
                    var tagId = 0;
                    //1. 태그아이디를 가져옵니다.
                    if (result == null) { tagId = await _unitOfWork.Tags.AddAsync(tag); }
                    else { tagId = result.tag_id; }

                    if (tagId > 0)
                    {
                        var mapping = new DamoyeoMeetupTags();
                        mapping.tag_id = tagId;
                        mapping.meetup_id = meetup.meetup_id;
                        //2. 태그 매핑테이블에 추가
                        await _unitOfWork.MeetupTagsMapping.AddAsync(mapping);
                    }
                }
            }

            _unitOfWork.Commit();
            ///리다이렉트

            return View();
        }

        /////////////
        ///API CALLS
        ////////////




        [HttpPost]
        public async Task<ActionResult> Application(int meetup_id)
        {
            if (UserManager.IsLogin())
            {
                var entity = new DamoyeoApplications();
                entity.meetup_id = meetup_id;
                entity.user_id = UserManager.GetCookie().UserId;
                entity.application_date = DateTime.Now;
                var data = await _unitOfWork.Applications.GetAsync(entity);
                if (data == null)
                {
                    await _unitOfWork.Applications.AddAsync(entity);
                    _unitOfWork.Commit();
                    return Json(new { success = true, code = 1 });
                }
                else 
                {
                    await _unitOfWork.Applications.RemoveAsync(data.application_id);
                    _unitOfWork.Commit();
                    return Json(new { success = true, code = 2 });
                }

            }
            else 
            {
                return Json(new { success = false,  code = 3 });
            }
            
        }

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
        private async Task<string> FileUpload(HttpPostedFileBase file, string savePath, string tableSaveName = "", int tableId = 0)
        {

            string originFileName = Path.GetFileName(file.FileName);

            // 파일 이름에 GUID를 추가하여 중복을 방지합니다.
            var savefileName = Guid.NewGuid().ToString()
                           + Path.GetExtension(file.FileName);

            var saveDetailfileName = savefileName.Split('.')[0]
                           + "_detail"
                           + Path.GetExtension(file.FileName);

            //~/Content/upload/meetup
            // 파일을 저장할 경로를 지정합니다.
            var path = Path.Combine(Server.MapPath(savePath), savefileName);

            
            var detailPath = Path.Combine(Server.MapPath(savePath), saveDetailfileName);

            // 해당 경로에 폴더가 없으면 폴더를 생성합니다.
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 파일을 지정한 경로에 저장합니다.
            //file.SaveAs(path);
            ResizeAndSaveImage(file,406,354,path); //리스트용 이미지
            ResizeAndSaveImage(file, 728, 728, detailPath); // 상세이미지
            if (!string.IsNullOrEmpty(tableSaveName) && tableId > 0) 
            {
                DamoyeoImage image = new DamoyeoImage();                
                image.directory_path =  savePath + "/";
                image.origin_filename = originFileName;
                image.save_filename = savefileName;
                image.table_name = tableSaveName;
                image.table_id = tableId; // SQL SERVER ID값 먼저 가져오는방법
                await _unitOfWork.Image.AddAsync(image);

            }

            return Url.Content(savePath + "/" + savefileName);
        }


        private void ResizeAndSaveImage(HttpPostedFileBase file, int width, int height, string outputPath)
        {
            using (Image originalImage = Image.FromStream(file.InputStream, true, true))
            {
                using (Bitmap newImage = new Bitmap(width, height))
                {
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        graphics.DrawImage(originalImage, 0, 0, width, height);
                    }

                    // 리사이즈된 이미지 저장 (JPEG 형식으로 저장)
                    newImage.Save(outputPath, ImageFormat.Jpeg);
                }
            }
        }
    }
}
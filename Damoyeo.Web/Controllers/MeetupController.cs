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
        public async Task<ActionResult> Index(int page =1, string applicationSdate ="", string searchString = ""
            , string searchArea = "", int searchCategory = 0, int searchOrder = 1 )
        {

            MeetupListVm viewModel = new MeetupListVm();

            if (string.IsNullOrEmpty(applicationSdate)) 
            {
                DateTime now = DateTime.Now; // 현재 날짜와 시간
                DateTime oneWeekAgo = now; // 일주일 전
                applicationSdate = oneWeekAgo.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
              
            }
            viewModel.MeetupSearchOpt = new MeetupSearchOpt();
            viewModel.MeetupSearchOpt.applicationSdate = applicationSdate;
            viewModel.MeetupSearchOpt.searchString = searchString;
            viewModel.MeetupSearchOpt.searchArea = searchArea;
            viewModel.MeetupSearchOpt.searchCategory = searchCategory;
            viewModel.MeetupSearchOpt.searchOrder = searchOrder;



            viewModel.list = await _unitOfWork.Meetup.GetPagedListAsync(1, 100, viewModel.MeetupSearchOpt);
            viewModel.categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);

            if (UserManager.IsLogin())
            {
                var entity = new DamoyeoWishlist();
                entity.user_id = UserManager.GetCookie().UserId;
                viewModel.WishList = await _unitOfWork.Wishlist.GetAllAsync(entity);
            }
                
            
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
        public async Task<ActionResult> Write(DamoyeoMeetup meetup, HttpPostedFileBase main_image
            , HttpPostedFileBase file1
            , HttpPostedFileBase file2
            , HttpPostedFileBase file3
            , HttpPostedFileBase file4
            , List<string> tags)
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

            if (file1 != null)
            {
                await FileUpload(file1, savePath, "Damoyeo_Meetup", insertId, "일");
            }
            if (file2 != null)
            {
                await FileUpload(file2, savePath, "Damoyeo_Meetup", insertId, "이");
            }
            if (file3 != null)
            {
                await FileUpload(file3, savePath, "Damoyeo_Meetup", insertId, "삼");
            }
            if (file4 != null)
            {
                await FileUpload(file4, savePath, "Damoyeo_Meetup", insertId, "사");
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

            TempData["successMsg"] = "모임 등록 성공!!";
            return RedirectToAction("Index");

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

            meetupDetailVm.detail = await detailTask;
            meetupDetailVm.applicationList = await applicationsTask;
            meetupDetailVm.image = await imagesTask;
            meetupDetailVm.Tags = await tagsTask;

            return View(meetupDetailVm);
        }

        [HttpPost]
        [Auth]
        public async Task<ActionResult> Edit(DamoyeoMeetup meetup, HttpPostedFileBase main_image
            , HttpPostedFileBase file1
            , HttpPostedFileBase file2
            , HttpPostedFileBase file3
            , HttpPostedFileBase file4
            , List<string> tags)
        {
            PagedList<DamoyeoCategory> categoryList = await _unitOfWork.Category.GetPagedListAsync(1, 10);
            ViewData["categoryList"] = categoryList;
            var oriMeetup = await _unitOfWork.Meetup.GetAsync(meetup);
            meetup.reg_date = oriMeetup.reg_date;
            meetup.use_tf = oriMeetup.use_tf;
            meetup.meetup_master_id = oriMeetup.meetup_master_id;
            var savePath = "/Content/upload/meetup";

            
            //메인이미지가 존재하면 파일 업로드 해준다.
            if (main_image != null)
            {
                meetup.meetup_image = await FileUpload(main_image, savePath);
            }
            else 
            {
                meetup.meetup_image = oriMeetup.meetup_image;
            }

            await _unitOfWork.Meetup.UpdateAsync(meetup);

            //DB에서 이미지삭제 하기위한 파라메타
            var imageParameta = new DamoyeoImage { table_name = "Damoyeo_Meetup", table_id = meetup.meetup_id };
            
            if (file1 != null) 
            {
                await _unitOfWork.Image.RemoveTableIdAsync(imageParameta,"일");
                await FileUpload(file1, savePath, "Damoyeo_Meetup", meetup.meetup_id, "일");
            }
            if (file2 != null)
            {
                await _unitOfWork.Image.RemoveTableIdAsync(imageParameta, "이");
                await FileUpload(file2, savePath, "Damoyeo_Meetup", meetup.meetup_id, "이");
            }
            if (file3 != null)
            {
                await _unitOfWork.Image.RemoveTableIdAsync(imageParameta, "삼");
                await FileUpload(file3, savePath, "Damoyeo_Meetup", meetup.meetup_id, "삼");
            }
            if (file4 != null)
            {
                await _unitOfWork.Image.RemoveTableIdAsync(imageParameta, "사");
                await FileUpload(file4, savePath, "Damoyeo_Meetup", meetup.meetup_id, "사");
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

            TempData["successMsg"] = "모임 수정 성공!!";
            return RedirectToAction("Detail", new { meetup_id = meetup.meetup_id });
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


        [HttpPost]
        public async Task<ActionResult> RemoveImg(int id, string type)
        {
            if (UserManager.IsLogin())
            {
                if (type == "sub")
                {
                    await _unitOfWork.Image.RemoveAsync(id);
                    _unitOfWork.Commit();
                    return Json(new { success = true, code = 1 });

                }
                else 
                {   //메인이미지 삭제
                    DamoyeoMeetup parameter = new DamoyeoMeetup();
                    parameter.meetup_id = id;
                    var oriMeetup = await _unitOfWork.Meetup.GetAsync(parameter);
                    oriMeetup.meetup_image = null;
                    await _unitOfWork.Meetup.UpdateAsync(oriMeetup);
                    _unitOfWork.Commit();
                    return Json(new { success = true, code = 1 });
                }
            }
            else
            {
                return Json(new { success = false, code = 3 });
            }

        }




        /// <summary>
        /// 지정된 경로에 file upload를 합니다.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<string> FileUpload(HttpPostedFileBase file, string savePath, string tableSaveName = "", int tableId = 0, string separationValue= "")
        {

            string originFileName = Path.GetFileName(file.FileName);

            // 파일 이름에 GUID를 추가하여 중복을 방지합니다.
            var savefileName = separationValue + Guid.NewGuid().ToString()
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
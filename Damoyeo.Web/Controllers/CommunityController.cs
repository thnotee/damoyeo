using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Model.Model;
using Damoyeo.Model.Model.option;
using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.Model.Procedure;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util.Manager;
using Damoyeo.Web.Fileter;
using System;
using System.Collections.Generic;
using System.IO;
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

            var entity = new CommunitySearchOpt();
            entity.searchString = searchString;
            PagedList<DamoyeoCommunity> list = await _unitOfWork.Community.GetPagedListAsync(page, 2, entity);
            list.pagerOptions.Path = "/Community/Index";
            list.pagerOptions.AddQueryString = "searchString=" + searchString;
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
        public async Task<ActionResult> Write(DamoyeoCommunity entity)
        {
            entity.title = Server.HtmlEncode(entity.title);
            entity.content = Server.HtmlEncode(entity.content);
            var cookieData = UserManager.GetCookie();
            entity.use_tf = "1";
            entity.user_id = cookieData.UserId;
            entity.post_date = DateTime.Now;
            await _unitOfWork.Community.AddAsync(entity);
            _unitOfWork.Commit();


            TempData["successMsg"] = "게시글 등록 성공!!";

            return RedirectToAction("Index");   
        }



        
        public async Task<ActionResult> Detail(int board_id)
        {

            CommunityDetailVm viewModel = new CommunityDetailVm();

            DamoyeoCommunity entity = new DamoyeoCommunity();
            entity.board_id = board_id;
            viewModel.detail = await _unitOfWork.Community.GetAsync(entity);
            if (viewModel.detail != null)
            {
                viewModel.detail.view_count += 1;
                await _unitOfWork.Community.UpdateAsync(viewModel.detail);
                _unitOfWork.Commit();
            }
            else 
            {
                viewModel.detail = entity;
            }
            return View(viewModel);
        }


        [Auth]
        public async Task<ActionResult> Edit(int board_id = 0)
        {
            DamoyeoCommunity entity = new DamoyeoCommunity();
            entity.board_id = board_id;
            var viewModel= await _unitOfWork.Community.GetAsync(entity);
            return View(viewModel);
        }

        [HttpPost]
        [Auth]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(DamoyeoCommunity entity)
        {
            entity.title = Server.HtmlEncode(entity.title);
            entity.content = Server.HtmlEncode(entity.content);
            var cookieData = UserManager.GetCookie();
            entity.use_tf = "1";
            entity.user_id = cookieData.UserId;
            entity.post_date = DateTime.Now;
            await _unitOfWork.Community.UpdateAsync(entity);
            _unitOfWork.Commit();


            TempData["successMsg"] = "게시글 수정 성공!!";

            return RedirectToAction("Detail", new { board_id  = entity.board_id});
        }



        [Auth]
        public async Task<ActionResult> Remove(int board_id = 0)
        {
            DamoyeoCommunity entity = new DamoyeoCommunity();
            entity.board_id = board_id;
            var data = await _unitOfWork.Community.GetAsync(entity);

            var cookieData = UserManager.GetCookie();
            if (cookieData.UserId == data.user_id) 
            {
                await _unitOfWork.Community.RemoveAsync(board_id);
                _unitOfWork.Commit();
                TempData["successMsg"] = "게시글 삭제 성공!!";
                return RedirectToAction("Index");
            }
            TempData["errorMsg"] = "잘못된 접근입니다.";
            return RedirectToAction("Index");
        }

        /////////////////
        /// API CALL  ///
        /////////////////


        public async Task<ActionResult> GetCommentPartialView(int page=1, int board_id=0)
        {
            PagedList<GetCommentTree> viewModel = null;
            var entity = new CommunitySearchOpt();
            entity.board_id = board_id;
            viewModel = await _unitOfWork.CommunityComment.GetPagedListAsync(page, 100, entity);
            return PartialView("/Views/Community/_Partial/_Comment.cshtml", viewModel);
        }



        public async Task<ActionResult> AddComment(int board_id, string content, int parent_commentid = 0)
        {

            var entity = new DamoyeoCommunityComment();
            entity.content = content;
            entity.board_id = board_id;
            entity.parent_commentid = parent_commentid;
            entity.user_id = UserManager.GetCookie().UserId;
            entity.comment_date = DateTime.Now;
            entity.use_tf = "1";
           
            await _unitOfWork.CommunityComment.AddAsync(entity);
            _unitOfWork.Commit();
        
            
            return Json(new { success = true, data = 1 });
        }



        [HttpPost]
        public async Task<ActionResult> EditComment(int comment_id, string content)
        {

            var entity = new DamoyeoCommunityComment();
            entity.comment_id = comment_id;
            entity.content = content;
            await _unitOfWork.CommunityComment.UpdateAsync(entity);
            _unitOfWork.Commit();


            return Json(new { success = true, data = 1 });
        }

        [HttpPost]
        public async Task<ActionResult> RemoveComment(int comment_id)
        {

            var entity = new DamoyeoCommunityComment();
            entity.comment_id = comment_id;
            
            await _unitOfWork.CommunityComment.RemoveAsync(comment_id);
            _unitOfWork.Commit();


            return Json(new { success = true, data = 1 });
        }

       
        /// <summary>
        /// 이미지 업로드
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult> UploadImage(HttpPostedFileBase file)
        {
      
            var userCookie = UserManager.GetCookie();
            var userParameter = new DamoyeoUser();
            userParameter.email = userCookie.Email;
            var user = await _unitOfWork.Users.GetAsync(userParameter);

            var savePath = "/Content/upload/community";

            if (file != null)
            {
                string originFileName = Path.GetFileName(file.FileName);


                var savefileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(file.FileName);

                // 파일을 저장할 경로를 지정
                var path = Path.Combine(Server.MapPath(savePath), savefileName);

                // 해당 경로에 폴더가 없으면 폴더를 생성합니다.
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                file.SaveAs(path);
                var finalPath = savePath + "/" + savefileName;
             

                return Json(new { location = finalPath });
            }

            return Json(new { location = "" });
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Damoyeo.Web.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var imagePath = "";
            string wwwRootPath = HostingEnvironment.MapPath("~/");
            if (file != null)
            {
                string originFileName = Path.GetFileName(file.FileName);
                string savefileName = Guid.NewGuid().ToString() + originFileName; //중복 회피를 위해
                string imgPath = @"Content\upload\board-" + DateTime.Now.ToString("yyyyMM");
                string finalPath = Path.Combine(wwwRootPath, imgPath);
                if (!Directory.Exists(finalPath)) { Directory.CreateDirectory(finalPath); } //폴더생성
                using (var fileStream = new FileStream(Path.Combine(finalPath, savefileName), FileMode.Create))
                {
                    file.InputStream.CopyTo(fileStream);
                }
                imagePath = @"\" + imgPath + @"\" + savefileName;
            }
            return Json(new { location = imagePath });
        }
    }
}
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
        // GET: Meetup
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Write()
        {
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
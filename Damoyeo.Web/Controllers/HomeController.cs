using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Damoyeo.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DamoyeoConnectionString"].ConnectionString)) 
            {
                var sql = @"
SELECT NAME FROM Test
WHERE 
NAME= '이태환'
";
                var result = conn.QueryFirstOrDefault<string>(sql);
                ViewData["test"] = result;
                return View();
            }
            
            return View();
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
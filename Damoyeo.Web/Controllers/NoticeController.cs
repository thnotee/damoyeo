using Damoyeo.Model.Model.Pager;
using Damoyeo.Model.Model;
using Damoyeo.Web.Fileter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.Util.Manager;
using Damoyeo.Model.ViewModel;
using Damoyeo.Util;
using Microsoft.IdentityModel.Tokens;
using Damoyeo.Model.Model.option;
using System.Globalization;
using System.IO;
using System.Web.UI;
using Damoyeo.Model.Model.Procedure;

namespace Damoyeo.Web.Controllers
{
    
    public class NoticeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public NoticeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: User
        public async Task<ActionResult> Index()
        { 
            return View();
        }

        public async Task<ActionResult> Detail()
        {
            return View();
        }

    }
}
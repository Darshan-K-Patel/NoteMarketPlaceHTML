using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trails.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Route("Dashboard")]
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
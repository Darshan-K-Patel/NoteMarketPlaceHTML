using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trails.Controllers
{
    public class FAQController : Controller
    {
        // GET: FAQ
        [Route("FAQ")]
        public ActionResult FAQ()
        {
            return View();
        }
    }
}
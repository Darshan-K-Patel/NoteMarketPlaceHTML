using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trails.Controllers
{
    public class BuyerRequestController : Controller
    {
        // GET: BuyerRequest
        [Route("BuyerRequest")]
        public ActionResult BuyerRequest()
        {
            return View();
        }
    }
}
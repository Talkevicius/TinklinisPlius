using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;

namespace TinklinisPlius.Controllers
{
    public class MatchController : Controller
    {
        public ActionResult MatchInfoWindow()
        {
            return View();
        }
    }
}
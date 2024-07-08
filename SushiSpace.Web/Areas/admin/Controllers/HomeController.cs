﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SushiSpace.Web.Areas.admin.Controllers
{
    
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}

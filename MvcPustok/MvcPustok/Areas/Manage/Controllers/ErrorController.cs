using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    [Area("manage")]
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}


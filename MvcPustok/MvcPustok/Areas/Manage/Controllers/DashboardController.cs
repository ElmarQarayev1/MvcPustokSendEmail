using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcPustok.Data;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    [Area("manage")]
    public class DashboardController:Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Areas.Manage.ViewModels;
using MvcPustok.Data;
using MvcPustok.Models;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    [Area("manage")]
    public class AuthorController:Controller
	{
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
		{
             _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Authors.Include(x => x.Books);
            return View(PaginatedList<Author>.Create(query, page, 2));
        }
        public IActionResult Create()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View(author);
            }

            if (_context.Authors.Any(x => x.Fullname == author.Fullname))
            {
                ModelState.AddModelError("Fullname", "Fullname already exists!");
                return View(author);
            }

            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}


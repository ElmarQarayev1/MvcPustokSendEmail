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
	public class GenreController:Controller
	{
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
		{
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            var query = _context.Genres.Include(x => x.Books);
            return View(PaginatedList<Genre>.Create(query, page, 2));
        }
        public IActionResult Create()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {

                return View(genre);
            }

            if (_context.Genres.Any(x => x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "Genre already exists!");
                return View(genre);
            }

            _context.Genres.Add(genre);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.Find(id);

            if (genre == null) return RedirectToAction("notfound", "error");

            return View(genre);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }

            Genre existGenre = _context.Genres.Find(genre.Id);

            if (existGenre == null) return RedirectToAction("notfound", "error");

            if (_context.Genres.Any(x => x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "Genre already exists!");
                return View(genre);
            }

            existGenre.Name = genre.Name;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}


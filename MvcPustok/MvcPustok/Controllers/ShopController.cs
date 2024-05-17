using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.ViewModels;
namespace MvcPustok.Controllers
{
	public class ShopController:Controller
	{
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? genreId = null, List<int>? authorIds = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            ShopViewModel viewmodel = new ShopViewModel
            {
                Authors = _context.Authors.Include(x => x.Books).ToList(),
                Genres = _context.Genres.Include(x => x.Books).ToList(),
            };

            var queryy = _context.Books.Include(x => x.BookImages.Where(bi => bi.Status != null)).Include(x => x.Author).AsQueryable();

            if (genreId != null)
                queryy = queryy.Where(x => x.GenreId == genreId);
            if (authorIds != null)
                queryy = queryy.Where(x => authorIds.Contains(x.AuthorId));
            if (minPrice != null && maxPrice != null)
                queryy = queryy.Where(x => x.SalePrice >= minPrice && x.SalePrice <= maxPrice);

            viewmodel.Books = queryy.ToList();
           
            ViewBag.AuthorIds = authorIds;
            ViewBag.GenreId = genreId;
            ViewBag.MinPrice = _context.Books.Where(x => !x.IsDeleted).Min(x => x.SalePrice);
            ViewBag.MaxPrice = _context.Books.Where(x => !x.IsDeleted).Max(x => x.SalePrice);
            ViewBag.SelectedMinPrice = minPrice ?? ViewBag.MinPrice;
            ViewBag.SelectedMaxPrice = maxPrice ?? ViewBag.MaxPrice;
            return View(viewmodel);
        }
    
}
}


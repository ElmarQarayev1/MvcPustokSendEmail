using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Areas.Manage.ViewModels;
using MvcPustok.Data;
using MvcPustok.Helpers;
using MvcPustok.Models;
using System.Net;
using MvcPustok.Models.Enum;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    [Area("manage")]
	public class BookController:Controller
	{
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context,IWebHostEnvironment env)
		{
            _context = context;
            _env = env;
        }
        //public IActionResult setSession()
        //{
        //    HttpContext.Session.SetString("name", "elmar");
        //    return Content("");
        //}

        //public IActionResult getSession()
        //{
        //    return Content(HttpContext.Session.GetString("name"));
        //}
        public IActionResult setCookie()
        {
            CookieOptions opt = new CookieOptions();
            opt.Expires = DateTime.Now.AddMinutes(3);

            HttpContext.Response.Cookies.Append("name", "Elmar", opt);
            return Content("");
        }
        public IActionResult getCookie()
        {
            return Content(HttpContext.Request.Cookies["name"]);
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Books.Include(x => x.Author).Include(x => x.Genre).Include(x => x.BookImages.
            Where(x => x.Status == true)).Where(x=>!x.IsDeleted).Include(x=>x.BookReviews).Include(x=>x.BookTags).
            ThenInclude(x=>x.Tag).OrderByDescending(x => x.Id);
            return View(PaginatedList<Book>.Create(query,page,3));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags= _context.Tags.ToList();
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (book.PosterFile == null) ModelState.AddModelError("PosterFile", "PosterFile is required");
            if (book.HoverFile  == null) ModelState.AddModelError("HoverFile", "HoverFile is required");

            if (!ModelState.IsValid)
            {

                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View(book);
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                return RedirectToAction("notfound", "error");
            }
            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                return RedirectToAction("notfound", "error");

            }
            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");


                BookTags bookTag = new BookTags
                {
                    TagId = tagId,
                    Book = book
                };
                _context.BookTags.Add(bookTag);
            }
            BookImages poster = new BookImages
            {
                Name = FileManager.Save(book.PosterFile, _env.WebRootPath, "uploads/book"),
                Status = true,
                 Book = book
            };
            BookImages hover = new BookImages
            {
                Name = FileManager.Save(book.HoverFile, _env.WebRootPath, "uploads/book"),
                Status = false,
               Book = book
            };

            _context.BookImages.Add(poster);
            _context.BookImages.Add(hover);

            foreach (var imgFile in book.ImageFiles)
            {
                BookImages bookImg = new BookImages
                {
                    Name = FileManager.Save(imgFile, _env.WebRootPath, "uploads/book"),
                    Status = null,
                    Book = book
                };
                book.BookImages.Add(bookImg);
            }

            _context.Books.Add(book);

            _context.SaveChanges();

            return RedirectToAction("index");           
        }
        public IActionResult Edit(int id)
       {
            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                return RedirectToAction("notfound", "error");
            }
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();
            return View(book);
        }
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            Book? existBook = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == book.Id);
            if (existBook == null) return RedirectToAction("notfound", "error");

            if (book.AuthorId != existBook.AuthorId && !_context.Authors.Any(x => x.Id == book.AuthorId))
                return RedirectToAction("notfound", "error");

            if (book.GenreId != existBook.GenreId && !_context.Genres.Any(x => x.Id == book.GenreId))
                return RedirectToAction("notfound", "error");

            existBook.BookTags.RemoveAll(x => !book.TagIds.Contains(x.TagId));

            foreach (var tagId in book.TagIds.FindAll(a => !existBook.BookTags.Any(x => x.TagId == a)))
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");

                BookTags bookTag = new BookTags
                {
                    TagId = tagId,
                };

                existBook.BookTags.Add(bookTag);
            }
            List<string> removedFileNames = new List<string>();

            List<BookImages> removedImages = existBook.BookImages.FindAll(x => x.Status == null && !book.BookImageIds.Contains(x.Id));
            removedFileNames = removedImages.Select(x => x.Name).ToList();

            _context.BookImages.RemoveRange(removedImages);
            if (book.PosterFile != null)
            {
                BookImages poster = existBook.BookImages.FirstOrDefault(x => x.Status == true);

                removedFileNames.Add(poster.Name);

                poster.Name = FileManager.Save(book.PosterFile, _env.WebRootPath, "uploads/book");
            }
            foreach (var imgFile in book.ImageFiles)
            {
                BookImages bookImg = new BookImages
                {
                    Name = FileManager.Save(imgFile, _env.WebRootPath, "uploads/book"),
                    Status = null,
                };
                existBook.BookImages.Add(bookImg);
            }
            existBook.Name = book.Name;
            existBook.Desc = book.Desc;
            existBook.SalePrice = book.SalePrice;
            existBook.CostPrice = book.CostPrice;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.IsNew = book.IsNew;
            existBook.IsFeatured = book.IsFeatured;
            existBook.StockStatus = book.StockStatus;

            _context.SaveChanges();

            foreach (var fileName in removedFileNames)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/book", fileName);
            }

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.Author).Include(x => x.Genre)
                .Include(x => x.BookTags).ThenInclude(x => x.Tag).FirstOrDefault(x => x.Id == id&& !x.IsDeleted );

            if (book == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(book);
        }

        [HttpPost]
        public IActionResult Delete(Book book)
        {
            Book existBook = _context.Books.FirstOrDefault(x => x.Id == book.Id && !x.IsDeleted);

            if (existBook == null)
            {
                return RedirectToAction("notfound", "error");
            }
            existBook.IsDeleted = true;
            existBook.ModifiedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Review(int id)
        {
            var book = await _context.Books
                                     .Include(b => b.BookReviews).ThenInclude(x=>x.AppUser)
                                     .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return RedirectToAction("notfound", "error");
            }

            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var review = await _context.BookReviews.FindAsync(id);

            if (review == null)
            {
                return RedirectToAction("notfound", "error");
            }
            review.Status = ReviewStatus.Rejected;
            _context.Update(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("index", "book");
        }
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {

            var review = await _context.BookReviews.FindAsync(id);

            if (review == null)
            {
                return RedirectToAction("notfound", "error");
            }

            review.Status = ReviewStatus.Accepted;
            _context.Update(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "book");
        }
    }
}


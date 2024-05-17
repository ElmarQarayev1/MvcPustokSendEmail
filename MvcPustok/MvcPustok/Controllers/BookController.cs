using System;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.Models;
using MvcPustok.ViewModels;

namespace MvcPustok.Controllers
{
	
	public class BookController:Controller
	{
		 private readonly AppDbContext _context;
       
        private readonly UserManager<AppUser> _userManager;

        public BookController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            
            _userManager = userManager;
        }
        public IActionResult GetBookById(int id)
		{
			Book book = _context.Books.Include(x => x.Genre).Include(x => x.BookImages.Where(x => x.Status == true)).FirstOrDefault(x => x.Id == id);
			return PartialView("_BookModalContentPartial", book);
		}

        public async Task<IActionResult> Detail(int id)
        {
            var vm = await getBookDetail(id);

            if (vm?.Book == null) return RedirectToAction("notfound", "error");

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Review(BookReview review)
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
                return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = review.BookId }) });

            if (!_context.Books.Any(x => x.Id == review.BookId && !x.IsDeleted))
                return RedirectToAction("notfound", "error");

            if (_context.BookReviews.Any(x => x.Id == review.BookId && x.AppUserId == user.Id))
                return RedirectToAction("notfound", "error");


            if (!ModelState.IsValid)
            {
                var vm = await getBookDetail(review.BookId);  
                vm.Review = review;
                return View("detail", vm);
            }

            review.AppUserId = user.Id;
            review.CreatedAt = DateTime.Now;

            _context.BookReviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("detail", new { id = review.BookId });
        }

        private async Task<BookDetailViewModel> getBookDetail(int bookId)
        {
            Book? book = await _context.Books
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .Include(x => x.BookImages)
                .Include(x => x.BookTags).ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(x => x.Id == bookId && !x.IsDeleted);

            if (book == null)
            {
                return null;
            }

            BookDetailViewModel bookDetail = new BookDetailViewModel
            {
                Book = book,
                RelatedBooks = await _context.Books
                    .Include(x => x.Author)
                    .Include(x => x.BookImages.Where(bi => bi.Status != null))
                    .Where(x => x.GenreId == book.GenreId && x.Id != bookId)
                    .Take(5).ToListAsync(),
                Review = new BookReview { BookId = bookId }
            };

            AppUser? user = await _userManager.GetUserAsync(User);

            if (user != null && await _userManager.IsInRoleAsync(user, "member") &&
                _context.BookReviews.Any(x => x.BookId == bookId && x.AppUserId == user.Id && x.Status != Models.Enum.ReviewStatus.Rejected))
            {
                bookDetail.HasUserReview = true;
            }

            bookDetail.TotalReviewsCount = await _context.BookReviews.CountAsync(x => x.BookId == bookId && x.Status != Models.Enum.ReviewStatus.Rejected);

            bookDetail.Book.BookReviews = await _context.BookReviews
                .Where(x => x.BookId == bookId && x.Status != Models.Enum.ReviewStatus.Rejected)
                .Include(r => r.AppUser)
                .OrderByDescending(r => r.CreatedAt)
                .Take(2)
                .ToListAsync();

            bookDetail.AvgRate = bookDetail.TotalReviewsCount > 0 ? (int)Math.Ceiling(await _context.BookReviews.Where(x => x.BookId == bookId && x.Status != Models.Enum.ReviewStatus.Rejected).AverageAsync(x => x.Rate)) : 0;

            return bookDetail;
        }
        public async Task<IActionResult> GetMoreReviews(int bookId, int skip, int take)
        {
            var reviews = await _context.BookReviews
                .Where(x => x.BookId == bookId && x.Status != Models.Enum.ReviewStatus.Rejected)
                .Include(r => r.AppUser)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return Json(reviews);
        }
        public IActionResult AddToBasket(int bookId)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == bookId && !x.IsDeleted);
            if (book == null) return RedirectToAction("notfound", "error");

            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                BasketItem? item = _context.BasketItems.FirstOrDefault(x => x.AppUserId == userId && x.BookId == bookId);

                if (item == null)
                {
                    item = new BasketItem
                    {
                        AppUserId = userId,
                        BookId = bookId,
                        Count = 1
                    };
                    _context.BasketItems.Add(item);
                }
                else item.Count++;

                _context.SaveChanges();
            }
            else
            {
                List<BasketCookiesViewModel> basketItems = new List<BasketCookiesViewModel>();

                var cookieItem = Request.Cookies["basket"];

                if (cookieItem != null)
                {
                    basketItems = JsonSerializer.Deserialize<List<BasketCookiesViewModel>>(cookieItem);
                }
                //basketcookiesviewmodel
                var item = basketItems.FirstOrDefault(x => x.BookId == bookId);

                if (item == null)
                {
                    item = new BasketCookiesViewModel
                    {
                        BookId = bookId,
                        Count = 1
                    };
                    basketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }

                Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));

            }
            return RedirectToAction("index", "home");

        }


    }
}
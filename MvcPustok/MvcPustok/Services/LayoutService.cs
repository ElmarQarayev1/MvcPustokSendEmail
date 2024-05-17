using System;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.Models;
using MvcPustok.ViewModels;

namespace MvcPustok.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<String, String> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }
        public BasketViewModel GetBasket()
        {
            BasketViewModel vm = new BasketViewModel();

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.User.IsInRole("member"))
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var basketItems = _context.BasketItems
               .Include(x => x.Book).ThenInclude(b => b.BookImages.Where(x => x.Status == true))
               .Where(x => x.AppUserId == userId)
               .ToList();

                vm.Items = basketItems.Select(x => new BasketItemViewModel
                {
                    BookId = x.BookId,
                    BookName = x.Book.Name,
                    BookPrice = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
                    BookImage = x.Book.BookImages.FirstOrDefault(x => x.Status == true)?.Name,
                    Count = x.Count
                }).ToList();

                vm.TotalPrice = vm.Items.Sum(x => x.BookPrice*x.Count);
            }
            else
            {
                var cookieBasket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

                if (cookieBasket != null)
                {
                    List<BasketCookiesViewModel> cookieItemsVM = JsonSerializer.Deserialize<List<BasketCookiesViewModel>>(cookieBasket);
                    ;
                    foreach (var cookieItem in cookieItemsVM)
                    {
                        Book? book = _context.Books.Include(x => x.BookImages.Where(c => c.Status == true)).FirstOrDefault(x => x.Id == cookieItem.BookId && !x.IsDeleted);

                        if (book != null)
                        {
                            BasketItemViewModel item = new BasketItemViewModel
                            {
                                BookId = cookieItem.BookId,
                                Count = cookieItem.Count,
                                BookName = book.Name,
                                BookPrice = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice,
                                BookImage = book.BookImages.FirstOrDefault(x => x.Status == true)?.Name
                            };
                            vm.Items.Add(item);
                        }
                    }

                    vm.TotalPrice = vm.Items.Sum(x => x.Count * x.BookPrice);
                }
            }
            return vm;
        }
       


    }
}            



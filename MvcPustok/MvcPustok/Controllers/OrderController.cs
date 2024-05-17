using System;
using Azure.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.Models;
using MvcPustok.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MvcPustok.Controllers
{
	public class OrderController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context,UserManager<AppUser> userManager)
		{
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Checkout()
        {
            CheckoutViewModel vm = new CheckoutViewModel
            {
                BasketViewModel = getBasket()
            };

            return View(vm);
        }
        [Authorize(Roles = "member")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Checkout(OrderCreateViewModel orderviewmodel)
        {
            if (!ModelState.IsValid)
            {
                CheckoutViewModel vm = new CheckoutViewModel
                {
                    BasketViewModel = getBasket(),
                    Order = orderviewmodel
                };
                return View(vm);
            }

            AppUser user = await _userManager.GetUserAsync(User);

            Order order = new Order
            {
                Address = orderviewmodel.Address,
                Phone = orderviewmodel.Phone,
                CreatedAt = DateTime.Now,
                AppUserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Note = orderviewmodel.Note,
                Status = Models.Enum.OrderStatus.Pending
            };
            var basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id).ToList();

            order.OrderItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id).Select(x => new OrderItem
            {
                BookId = x.BookId,
                Count = x.Count,
                SalePrice = x.Book.SalePrice,
                DiscountPercent = x.Book.DiscountPercent,
                CostPrice = x.Book.CostPrice,
            }).ToList();

            _context.Orders.Add(order);
            _context.BasketItems.RemoveRange(basketItems);
            _context.SaveChanges();

            return RedirectToAction("profile", "account", new { tab = "orders"});
        }
        private BasketViewModel getBasket()
        {
            BasketViewModel vm = new BasketViewModel();

            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var basketItems = _context.BasketItems
               .Include(x => x.Book)
               .Where(x => x.AppUserId == userId)
               .ToList();

                vm.Items = basketItems.Select(x => new BasketItemViewModel
                {
                    BookId = x.BookId,
                    BookName = x.Book.Name,
                    BookPrice = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
                    Count = x.Count
                }).ToList();

                vm.TotalPrice = vm.Items.Sum(x => x.Count * x.BookPrice);
            }
            else
            {
                var cookieBasket = Request.Cookies["basket"];

                if (cookieBasket != null)
                {
                    List<BasketCookiesViewModel> cookieItemViewModel = System.Text.Json.JsonSerializer.Deserialize<List<BasketCookiesViewModel>>(cookieBasket);
                    ;
                    foreach (var cookieItem in cookieItemViewModel)
                    {
                        Book? book = _context.Books.Include(x => x.BookImages.Where(bi => bi.Status == true)).FirstOrDefault(x => x.Id == cookieItem.BookId && !x.IsDeleted);

                        if (book != null)
                        {
                            BasketItemViewModel itemviewmodel = new BasketItemViewModel
                            {
                                BookId = cookieItem.BookId,
                                Count = cookieItem.Count,
                                BookName = book.Name,
                                BookPrice = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice,
                            };
                            vm.Items.Add(itemviewmodel);
                        }

                    }

                    vm.TotalPrice = vm.Items.Sum(x => x.Count * x.BookPrice);
                }
            }
            return vm;
        }
        public IActionResult Delete(int bookId)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
     
                var basketItem = _context.BasketItems.FirstOrDefault(x => x.AppUserId == userId && x.BookId == bookId);
                if (basketItem != null)
                {
                    _context.BasketItems.Remove(basketItem);
                    _context.SaveChanges();
                }
            }
            else
            {  
                var cookieBasket = Request.Cookies["basket"];

                if (cookieBasket != null)
                {
                    List<BasketCookiesViewModel> cookieItemsVM = JsonSerializer.Deserialize<List<BasketCookiesViewModel>>(cookieBasket);

                    var itemToRemove = cookieItemsVM.FirstOrDefault(x => x.BookId == bookId);
                    if (itemToRemove != null)
                    {
                        cookieItemsVM.Remove(itemToRemove);

                        var updatedCookieValue = JsonSerializer.Serialize(cookieItemsVM);
                        Response.Cookies.Append("basket", updatedCookieValue);
                    }
                }
            }

            return RedirectToAction("index", "home");
        }

        [Authorize(Roles = "member")]
        public IActionResult Details(int id)
        {

            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(order);
        }

        //private List<OrderBasketItemViewModel> getOrderBasket(string userId)
        //{

        //    List<OrderBasketItemViewModel> items = new List<OrderBasketItemViewModel>();

        //    var basketItems = _context.BasketItems
        //   .Include(x => x.Book)
        //   .Where(x => x.AppUserId == userId)
        //   .ToList();

        //    items = basketItems.Select(x => new OrderBasketItemViewModel
        //    {
        //        BookId = x.BookId,
        //        Count = x.Count
        //    }).ToList();
        //    return items;
        //}

    }
}


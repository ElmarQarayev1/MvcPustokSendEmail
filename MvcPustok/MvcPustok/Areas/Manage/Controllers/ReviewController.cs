using System;
using Microsoft.AspNetCore.Mvc;
using MvcPustok.Data;
using MvcPustok.Models.Enum;

namespace MvcPustok.Areas.Manage.Controllers
{
	public class ReviewController:Controller
	{
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
		{
            _context = context;
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


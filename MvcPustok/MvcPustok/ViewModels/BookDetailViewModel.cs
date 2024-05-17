using System;
using MvcPustok.Models;

namespace MvcPustok.ViewModels
{
	public class BookDetailViewModel
    { 
        public int TotalReviewsCount { get; set; }
        public bool HasUserReview { get; set; }
        public BookReview Review { get; set; }
        public int AvgRate { get; set; }
        public Book? Book { get; set; }
        public List<Book> RelatedBooks { get; set; }
    }
}


using System;
using MvcPustok.Models;

namespace MvcPustok.ViewModels
{
	public class ShopViewModel
	{
        public List<Genre> Genres { get; set; }
        public List<Author> Authors { get; set; }
        public List<Book> Books { get; set; }
    }
}


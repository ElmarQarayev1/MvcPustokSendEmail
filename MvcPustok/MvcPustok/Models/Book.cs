using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using MvcPustok.Attributes.ValidationAttributes;

namespace MvcPustok.Models
{
	public class Book : AuditEntity
    {
        [MaxLength(50)]
        [MinLength(4)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Desc { get; set; }
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }

		public int AuthorId { get; set; }

		public int GenreId {get;set;}

		public bool StockStatus { get; set; }

        public bool IsNew { get; set; }

        public bool IsFeatured { get; set; }

        public Author? Author { get; set; }

		public Genre? Genre { get; set; }
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? PosterFile { get; set; }
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? HoverFile { get; set; }
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public List<IFormFile>? ImageFiles { get; set; } = new List<IFormFile>();
        public List<BookImages>? BookImages { get; set; } = new List<BookImages>();
        public List<BookTags> BookTags { get; set; } = new List<BookTags>();
        [NotMapped]
        public List<int>? TagIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int>? BookImageIds { get; set; } = new List<int>();

        public List<BookReview> BookReviews { get; set; }
    }
}


using System;
using System.ComponentModel.DataAnnotations;

namespace MvcPustok.ViewModels
{
	public class MemberLoginViewModel
	{
        [EmailAddress]
        [MaxLength(25)]
        [MinLength(5)]
        [Required]
        public string Email { get; set; }
        [MaxLength(25)]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

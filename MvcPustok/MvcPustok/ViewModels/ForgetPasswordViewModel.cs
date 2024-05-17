using System;
using System.ComponentModel.DataAnnotations;

namespace MvcPustok.ViewModels
{
	public class ForgetPasswordViewModel
	{
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [MinLength(8)]
        public string Email { get; set; }
    }
}


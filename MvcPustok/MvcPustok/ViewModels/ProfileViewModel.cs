using System;
using MvcPustok.Models;

namespace MvcPustok.ViewModels
{
	public class ProfileViewModel
	{
		public ProfileEditViewModel ProfileEditView { get; set; }
        public List<Order> Orders { get; set; } =new List<Order>();
    }
}


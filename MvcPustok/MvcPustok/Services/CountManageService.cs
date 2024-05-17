using System;
namespace MvcPustok
{
	public class CountManageService
	{
        private readonly CountService countService;

        public CountManageService(CountService countService)
        {
            this.countService = countService;
        }
        public int Count => countService.Count;
        public void Add()
        {
            countService.Add();
        }
    }
}


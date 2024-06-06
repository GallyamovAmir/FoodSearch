using FoodSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSearch.Controllers
{
    public class ManufacturerController : BaseController
    {

        private FoodSearchContext _context;


        public ManufacturerController(FoodSearchContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult MainPage()
        {
            string username = ViewBag.UserName;
            var user = _context.Users.FirstOrDefault(u => u.FullName == username);
            if (user == null)
            {
                return View();
            }
            return View();
        }

        [Authorize]
        public IActionResult ProductEditor()
        {

            return View();
        }

        //public IActionResult ShowProduct()
        //{

        //}

        //public async Task<IActionResult> AddProduct()
        //{

        //}

        //public async Task<IActionResult> UpdateProduct()
        //{

        //}

        //public async Task<IActionResult> DeleteProduct()
        //{

        //}
    }
}

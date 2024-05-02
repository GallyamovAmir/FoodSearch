using FoodSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodSearch.Controllers
{
    public class AccountController : BaseController
    {
        private readonly FoodSearchContext _context;
        private readonly UserManager<User> _userManager;

        public AccountController(FoodSearchContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Subscription()
        {
            return View();
        }

        [Authorize]
        public IActionResult Analytics()
        {
            return View();
        }

        [Authorize]
        public IActionResult Fixation()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Получение данных пользователя
            var userId = currentUser.Id;
            var userName = currentUser.FullName;
            var organization = currentUser.Organization;
            var subscription = currentUser.Subscription;

            // Передача данных пользователя в представление
            var model = new User
            {
                Id = userId,
                FullName = userName,
                Organization = organization,
                Subscription = subscription
            };

            return View(model);
        }
    }
}
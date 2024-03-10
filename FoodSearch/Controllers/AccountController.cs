using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodSearch.Controllers
{
    public class AccountController : BaseController
    {
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
        public IActionResult Profile()
        {


            return View();
        }
    }
}

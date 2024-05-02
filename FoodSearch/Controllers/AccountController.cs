using FoodSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSearch.Controllers
{
    public class AccountController : BaseController
    {
        private readonly FoodSearchContext _context;

        public AccountController(FoodSearchContext context)
        {
            _context = context;
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
        public IActionResult Profile()
        {
            string username = ViewBag.UserName;
            var user = _context.Users.FirstOrDefault(u => u.FullName == username);
           
            if (user != null)
            {
                var organization = _context.Organizations.Where(o => o.Id == user.OrganizationId).FirstOrDefault();
                var subscription = _context.Subscriptions.Where(s => s.Id == user.SubscriptionId).FirstOrDefault();

                ViewBag.UserFullName = user.FullName;
                ViewBag.UserId = user.Id;

                ViewBag.OrganizationName = organization?.Name;
                ViewBag.OrganizationOGRN = organization?.OGRN;
                ViewBag.OrganizationEmail = organization?.EMail;

                ViewBag.SubscriptionName = subscription?.Name;
                ViewBag.SubscriptionDescription = subscription?.Description;
                // и т.д.
            }

            return View();
        }
    }
}
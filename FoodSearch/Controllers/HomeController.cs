using FoodSearch.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FoodSearch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly FoodSearchContext _context;

        public HomeController(ILogger<HomeController> logger, FoodSearchContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password)
        {
            var form = Request.Form;

            // ���� login �/��� ������ �� �����������, �������� ��������� ��� ������ 400
            if (!form.ContainsKey("loginInput") || !form.ContainsKey("PasswordInput"))
                return BadRequest("login �/��� ������ �� �����������");

            login = form["loginInput"];
            password = form["PasswordInput"];

            // ������� ������������ 
            var user = _context.Users.FirstOrDefault(p => p.FullName == login && p.Password == password);

            // ���� ������������ �� ������, ���������� ��������� ��� 401
            if (user is null)
            {
                return Unauthorized("������������ ����");
            }

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login)
                };

                // ������� ������ ClaimsIdentity
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                // ��������� ������������������ ����
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}

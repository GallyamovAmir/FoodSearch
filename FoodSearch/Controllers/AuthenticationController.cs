﻿using FoodSearch.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodSearch.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> _logger;

        private readonly FoodSearchContext _context;


        public AuthenticationController(ILogger<AuthenticationController> logger, FoodSearchContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult SecondStepRegistration()
        {
            return View();
        }


        /// <summary>
        ///  Логика функции авторизации
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? login, string? password, int? subscription)
        {
            var form = Request.Form;

            // если login и/или пароль не установлены, посылаем статусный код ошибки 400
            if (!form.ContainsKey("loginInput") || !form.ContainsKey("PasswordInput"))
                return BadRequest("login и/или пароль не установлены");

            login = form["loginInput"];
            password = form["PasswordInput"];

            // находим пользователя 
            var user = _context.Users.FirstOrDefault(p => p.FullName == login && p.Password == password);


            // если пользователь не найден, отправляем статусный код 401
            if (user is null)
            {
                return Unauthorized("Недостаточно прав");
            }

            if (user != null)
            {

                string subscriptionValue = subscription.HasValue ? subscription.Value.ToString() : "";
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, login),
                    new(ClaimTypes.Role, subscriptionValue)
                };

                // создаем объект ClaimsIdentity
                ClaimsIdentity claimsIdentity = new(claims, "ApplicationCookie",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);



                // установка аутентификационных куки
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Логика функции выхода из аккаунта
        /// </summary>
        /// <returns></returns>

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        



    }
}

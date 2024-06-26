﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


namespace FoodSearch.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Получаем имя пользователя из утверждений
            var userroleClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (userroleClaim != null && int.TryParse(userroleClaim.Value, out int userRole))
            {
                ViewBag.userSubscription = userRole;
            }

            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (usernameClaim != null)
            {
                ViewBag.userName = usernameClaim.Value;
            }


        }
    }


}

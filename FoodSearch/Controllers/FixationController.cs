using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using FoodSearch.Models;

namespace FoodSearch.Controllers
{
    public class FixationController(FoodSearchContext context) : BaseController
    {
        private readonly FoodSearchContext _context = context;

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int userId)
        {

            // Проверяем, существует ли у пользователя корзина
            var cart = _context.Fixations.FirstOrDefault(f => f.UserId == userId);

            // Если корзины нет, создаем новую
            if (cart == null)
            {
                cart = new Fixation
                {
                    UserId = userId,
                    DateOdFixation = DateTime.Now
                };
                _context.Fixations.Add(cart);
            }

            // Проверяем, есть ли уже такой товар в корзине
            var existingCartItem = cart.FixationItems.FirstOrDefault(fi => fi.ProductId == productId);

            // Если товар уже есть в корзине, увеличиваем его количество
            if (existingCartItem != null)
            {
                existingCartItem.Count += quantity;
            }
            else
            {
                // Иначе добавляем новый товар в корзину
                var newItem = new FixationItem
                {
                    ProductId = productId,
                    Count = quantity
                };
                cart.FixationItems.Add(newItem);
            }

            // Сохраняем изменения в базе данных
            _context.SaveChanges();

            return RedirectToAction("", ""); // Или перенаправьте куда-то еще, в зависимости от вашего приложения
        }
    }
}

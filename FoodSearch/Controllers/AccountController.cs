using FoodSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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
            string username = ViewBag.UserName;
            var user = _context.Users.FirstOrDefault(u => u.FullName == username);

            var cartItems = _context.FixationItems
                         .Include(fi => fi.Product)
                         .Where(fi => fi.Fixation.UserId == user.Id)
                         .ToList();
            return View(cartItems);
        }

        [Authorize]
        public IActionResult GenerateExcelReport()
        {
            string username = ViewBag.UserName;
            var user = _context.Users.FirstOrDefault(u => u.FullName == username);

            // Получаем данные о товарах из корзины пользователя
            var cartItems = _context.FixationItems
                       .Include(fi => fi.Product)
                       .Where(fi => fi.Fixation.UserId == user.Id)
                       .ToList();

            var organization = _context.Organizations.FirstOrDefault(o => o.Id == user.OrganizationId);

            // Создаем новый пакет Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Установка контекста лицензирования
            using (var package = new ExcelPackage())
            {
                // Добавляем лист в Excel-файл
                var worksheet = package.Workbook.Worksheets.Add("Отчет");

                worksheet.Cells.Style.Font.Size = 14;

                using (var range = worksheet.Cells["A1:E1"])
                {
                    range.Style.Font.Bold = true; // Выделение заголовков жирным шрифтом
                    range.Style.Font.Size = 14; // Установка шрифта 14pt
                }

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Название товара";
                worksheet.Cells[1, 2].Value = "URL-Адрес товара";
                worksheet.Cells[1, 3].Value = "Количество";
                worksheet.Cells[1, 4].Value = "Цена за единицу";
                worksheet.Cells[1, 5].Value = "Общая цена";

                // Заполняем данные из корзины
                int row = 2;
                foreach (var item in cartItems)
                {
                    if (item.Product != null)
                    {
                        worksheet.Cells[row, 1].Value = item.Product.Name;
                        worksheet.Cells[row, 2].Value = item.Product.Url;
                        worksheet.Cells[row, 3].Value = item.Count;
                        worksheet.Cells[row, 4].Value = item.Product.Price;
                        worksheet.Cells[row, 5].Value = item.Count * item.Product.Price;
                        row++;
                    }
                }

                worksheet.Cells[row + 2, 1].Value = $"Данный отчет был сформирован для \"{organization.Name}\", ОГРН: {organization.OGRN}";
                worksheet.Cells[row + 3, 1].Value = "Пользователь запросивший отчет: " + user.FullName;
                worksheet.Cells[row + 4, 1].Value = "Отчет был сформирован FoodSearch";
                worksheet.Cells[row + 4, 1].Style.Font.UnderLine = true;

                // Сохраняем пакет Excel в поток
                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);

                // Устанавливаем позицию потока на начало
                stream.Position = 0;

                // Удаляем корзину фиксации пользователя
                _context.FixationItems.RemoveRange(cartItems);
                _context.SaveChanges();

                // Возвращаем Excel-файл как поток данных
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчет.xlsx");


            }
        }

        [Authorize]
        public IActionResult Profile()
        {
            string username = ViewBag.UserName;
            var user = _context.Users.FirstOrDefault(u => u.FullName == username);

            if (user != null)
            {
                var organization = _context.Organizations.FirstOrDefault(o => o.Id == user.OrganizationId);
                var subscription = _context.Subscriptions.FirstOrDefault(s => s.Id == user.SubscriptionId);

                ViewBag.UserFullName = user.FullName;
                ViewBag.UserId = user.Id;

                ViewBag.OrganizationName = organization?.Name;
                ViewBag.OrganizationOGRN = organization?.OGRN;
                ViewBag.OrganizationEmail = organization?.EMail;
                ViewBag.SubscriptionId = subscription?.Id;
                ViewBag.SubscriptionName = subscription?.Name;
                ViewBag.SubscriptionDescription = subscription?.Description;
                // и т.д.
            }

            return View();
        }
    }
}

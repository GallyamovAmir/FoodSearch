using FoodSearch.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FoodSearch.Controllers
{
    public class HomeController : BaseController
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

        public IActionResult SecondStepRegistration()
        {
            return View();
        }

        /// <summary>
        /// Первый шаг регистрации
        /// </summary>
        /// <param name="OGRN"></param>
        /// <param name="UserEmail"></param>
        /// <returns></returns>

        public IActionResult FirstStep(string? OGRN, string? UserEmail)
        {
            _logger.LogInformation($"Received OGRN: {OGRN}, UserEmail: {UserEmail}");

            var html = @"https://spark-interfax.ru/search?Query=" + OGRN;
            _logger.LogInformation($"Attempting to load HTML from: {html}");

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            if (htmlDoc == null)
            {
                _logger.LogInformation("Failed to load HTML document");
                return Json(new { Error = "Failed to load HTML document" });
            }

            var OGRNFinder = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='highlight']");
            var NameFinder = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='summary']/div/h3[@class='summary__title']/a");

            if (OGRNFinder != null && NameFinder != null)
            {
                var organizationOGRN = OGRNFinder.InnerText.Trim();
                var organizationName = NameFinder.InnerText.Trim();

                _logger.LogInformation($"Organization found. OGRN: {organizationOGRN}, Name: {organizationName}");

                if (OGRN == organizationOGRN)
                {
                    _logger.LogInformation("Organization OGRN matches input OGRN");

                    var existingOrganization = _context.Organizations.FirstOrDefault(o => o.OGRN == OGRN);

                    if (existingOrganization == null)
                    {
                        _logger.LogInformation("Organization not found in database. Creating new organization.");

                        // Организации не существует, создаем новую
                        var newOrganization = new Organization
                        {
                            OGRN = OGRN,
                            Name = organizationName,
                            EMail = UserEmail

                        };

                        // Сохраняем новую организацию в базе данных
                        _context.Organizations.Add(newOrganization);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _logger.LogInformation("Organization found in database.");
                    }

                    _logger.LogInformation("Redirecting to SecondStepRegistration");
                    ViewBag.OGRN = OGRN;
                    ViewBag.OrganizationName = organizationName;
                    return View("SecondStepRegistration");
                }
                else
                {
                    _logger.LogInformation($"Organization OGRN does not match input OGRN. Input OGRN: {OGRN}, Organization OGRN: {organizationOGRN}");
                }
            }
            else
            {
                _logger.LogInformation("Failed to find organization data in HTML document");
            }

            _logger.LogInformation("Organization with such OGRN does not exist");

            // Возвращаем сообщение об ошибке
            return Json(new { Error = "Organization with such OGRN does not exist" });
        }





        /// <summary>
        /// Второй шаг регистрации
        /// </summary>
        /// <param name="ogrn"></param>
        /// <param name="FIO"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        public IActionResult SecondStep(string? ogrn, string? FIO, string? password, string? confirmPassword)
        {
            string OGRN = ogrn;
            _logger.LogInformation($"Received OGRN: {OGRN}, FIO: {FIO}");

            // Проверяем наличие организации в базе данных
            var existingOrganization = _context.Organizations.FirstOrDefault(o => o.OGRN == OGRN);

            if (existingOrganization == null)
            {
                _logger.LogInformation("Organization not found in database.");
                return BadRequest("Организация не найдена в базе данных.");
            }
            else
            {
                _logger.LogInformation("Organization found in database.");
            }

            // Проверяем, совпадают ли пароль и его подтверждение
            if (password != confirmPassword)
            {
                _logger.LogInformation("Password and confirmation do not match.");
                return BadRequest("Пароль и его подтверждение не совпадают.");
            }

            // Проверяем наличие пользователя в базе данных
            var existingUser = _context.Users.FirstOrDefault(u => u.FullName == FIO);

            if (existingUser == null)
            {
                _logger.LogInformation("Creating new user.");

                // Пользователя не существует, создаем нового
                var newUser = new User
                {
                    FullName = FIO,
                    Password = password,
                    OrganizationId = existingOrganization.Id,
                    SubscriptionId = 1 // Устанавливаем SubscriptionId по умолчанию
                };

                // Сохраняем нового пользователя в базе данных
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            else
            {
                _logger.LogInformation("User with this name already exists.");
                return BadRequest("Пользователь с таким именем уже существует.");
            }

            // После сохранения пользователя выполните необходимые действия

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Search(string? query)
        {
            // Поиск товаров в базе данных по названию
            var products = _context.Products.Where(p => p.Name.ToLower().Contains(query.ToLower())).ToList();

            // Если товары найдены в базе данных, передайте их в представление
            if (products.Any())
            {
                return View("Search", products);
            }
            else
            {
                // Если товары не найдены в базе данных, вызовите парсеры
                var metroParserResult = MetroParser(query);
                var selgrosParserResult = SelgrosParser(query);
                if ((metroParserResult != null && metroParserResult.Any()) && (selgrosParserResult != null && selgrosParserResult.Any()))
                {
                    // Обновляем базу данных
                    _context.Products.AddRange(metroParserResult);
                    _context.Products.AddRange(selgrosParserResult);
                    _context.SaveChanges();

                    // Повторно ищем товары в базе данных
                    products = _context.Products.Where(p => p.Name.ToLower().Contains(query.ToLower())).ToList();

                    _logger.LogInformation($"Number of products found: {products.Count}");
                    // Возвращаем представление с найденными товарами
                    return View("Search", products);
                }
                else
                {
                    // Возвращаем представление с сообщением об отсутствии результатов
                    ViewData["Message"] = "Результаты поиска отсутствуют.";
                    return View("Search");
                }


            }
        }

        // Метод для парсинга первого сайта
        private List<Product> MetroParser(string query)
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://online.metro-cc.ru/search?q={encodedQuery}";

            _logger.LogInformation($"Attempting to load HTML from: {url}");

            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            if (htmlDoc == null)
            {
                _logger.LogInformation("Failed to load HTML document");
                return null;
            }

            var products = new List<Product>();

            var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'catalog-2-level-product-card')]");

            if (productNodes != null)
            {
                // Добавляем счетчик, чтобы проходить только один раз
                int counter = 0;

                foreach (var productNode in productNodes)
                {
                    if (counter >= 1)
                    {
                        break; // Прерываем цикл после первого прохода
                    }

                    var product = new Product();

                    // Извлекаем данные о товаре из HTML узлов
                    product.Name = productNode.SelectSingleNode(".//span[contains(@class, 'product-card-name__text')]")?.InnerText.Trim();
                    product.ImageSource = productNode.SelectSingleNode(".//img[contains(@class, 'product-card-photo__image')]")?.GetAttributeValue("src", "");
                    var metroBaseUrl = "https://online.metro-cc.ru";
                    product.Url = metroBaseUrl + (productNode.SelectSingleNode(".//a[contains(@class, 'product-card-photo__link')]")?.GetAttributeValue("href", "") ?? "");
                    var priceNode = productNode.SelectSingleNode(".//span[contains(@class, 'product-price__sum-rubles')]");
                    if (priceNode != null)
                    {
                        var priceText = priceNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(priceText) && decimal.TryParse(priceText, out decimal price))
                        {
                            product.Price = price;
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to parse price for product: {product.Name}");
                            continue; // Пропускаем текущую итерацию цикла и переходим к следующей
                        }

                        product.Description = "Нет описания";
                        product.FactotyId = 1;

                        products.Add(product);
                    }

                    // Увеличиваем счетчик после каждой итерации
                    counter++;
                }
            }

            return products;
        }

        // Метод для парсинга второго сайта
        private List<Product> SelgrosParser(string query)
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://kazan.selgros24.ru/search/index.php?q={encodedQuery}";

            _logger.LogInformation($"Attempting to load HTML from: {url}");

            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            if (htmlDoc == null)
            {
                _logger.LogInformation("Failed to load HTML document");
                return null;
            }

            var products = new List<Product>();

            var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'catalog-grid-item')]");
            // Добавляем счетчик, чтобы проходить только один раз

            if (productNodes != null)
            {
                // Добавляем счетчик, чтобы проходить только один раз
                int counter = 0;

                foreach (var productNode in productNodes)
                {
                    if (counter >= 1)
                    {
                        break; // Прерываем цикл после первого прохода
                    }

                    var product = new Product();

                    // Извлекаем данные о товаре из HTML узлов
                    product.Name = productNode.SelectSingleNode(".//div[contains(@class, 'catalog-grid-item__title')]/a")?.InnerText.Trim();
                    var selgrosBaseUrl = "https://kazan.selgros24.ru";
                    product.ImageSource = selgrosBaseUrl + (productNode.SelectSingleNode(".//div[contains(@class, 'catalog-grid-item__img')]/a/img")?.GetAttributeValue("src", "") ?? "");
                    
                    product.Url = selgrosBaseUrl + (productNode.SelectSingleNode(".//div[contains(@class, 'catalog-grid-item__title')]/a")?.GetAttributeValue("href", "") ?? "");

                    var priceNode = productNode.SelectSingleNode(".//div[contains(@class, 'price-pane__current')]/text()");
                    if (priceNode != null)
                    {
                        var priceText = priceNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(priceText) && decimal.TryParse(priceText, out decimal price))
                        {
                            product.Price = price;
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to parse price for product: {product.Name}");
                            continue; // Пропускаем текущую итерацию цикла и переходим к следующей
                        }

                        product.Description = "Нет описания";
                        product.FactotyId = 1;

                        products.Add(product);
                    }

                    // Увеличиваем счетчик после каждой итерации
                    counter++;
                }
            }
            else
            {
                _logger.LogInformation("No product nodes found on the page");
            }

            return products;
        }




        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

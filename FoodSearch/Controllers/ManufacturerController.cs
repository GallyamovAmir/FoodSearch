using FoodSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodSearch.Controllers
{
    public class ManufacturerController : BaseController
    {

        private FoodSearchContext _context;


        public ManufacturerController(FoodSearchContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> MainPage()
        {
            var userNameClaim = User.FindFirst(ClaimTypes.Name);
            if (userNameClaim == null)
            {
                return Problem("User is not authenticated.");
            }

            string userName = userNameClaim.Value;

            // Получаем текущего пользователя по имени
            var user = await _context.Users
                                     .Include(u => u.Organization)
                                     .FirstOrDefaultAsync(u => u.FullName == userName);

            if (user?.Organization == null)
            {
                return Problem("User organization not found.");
            }

            // Извлекаем имя организации пользователя
            string organizationName = user.Organization.Name;

            // Получаем продукты, у которых имя фабрики совпадает с именем организации пользователя
            var products = await _context.Products
                                         .Include(p => p.Factoty) // Подключаем фабрику для проверки имени
                                         .Where(p => p.Factoty != null && p.Factoty.Name == organizationName)
                                         .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageSource,Url,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                // Получаем имя текущего пользователя из контекста
                var userNameClaim = User.FindFirst(ClaimTypes.Name);
                if (userNameClaim == null)
                {
                    return Problem("User is not authenticated.");
                }

                string userName = userNameClaim.Value;

                // Получаем текущего пользователя по имени
                var user = await _context.Users
                                         .Include(u => u.Organization)
                                         .FirstOrDefaultAsync(u => u.FullName == userName);

                if (user?.Organization == null)
                {
                    return Problem("User organization not found.");
                }

                // Извлекаем имя организации пользователя
                string organizationName = user.Organization.Name;

                // Ищем фабрику, имя которой совпадает с именем организации пользователя
                var factory = await _context.Factories
                                            .FirstOrDefaultAsync(f => f.Name == organizationName);

                if (factory == null)
                {
                    return Problem("No factory found with the same name as the user's organization.");
                }

                // Устанавливаем FactoryId продукта на Id найденной фабрики
                product.FactotyId = factory.Id;

                // Добавляем продукт в контекст
                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MainPage));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageSource,Url,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userNameClaim = User.FindFirst(ClaimTypes.Name);
                    if (userNameClaim == null)
                    {
                        return Problem("User is not authenticated.");
                    }

                    string userName = userNameClaim.Value;

                    // Получаем текущего пользователя по имени
                    var user = await _context.Users
                                             .Include(u => u.Organization)
                                             .FirstOrDefaultAsync(u => u.FullName == userName);

                    if (user?.Organization == null)
                    {
                        return Problem("User organization not found.");
                    }

                    // Извлекаем имя организации пользователя
                    string organizationName = user.Organization.Name;

                    // Ищем фабрику, имя которой совпадает с именем организации пользователя
                    var factory = await _context.Factories
                                                .FirstOrDefaultAsync(f => f.Name == organizationName);

                    if (factory == null)
                    {
                        return Problem("No factory found with the same name as the user's organization.");
                    }

                    // Устанавливаем FactoryId продукта на Id найденной фабрики
                    product.FactotyId = factory.Id;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MainPage));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'VarkaDbContext.Drinks'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MainPage));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using MenuTatil.Data;
using MenuTatil.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MenuTatil.Controllers
{
    [Authorize]
    public class MenusController : Controller
    {
        private readonly MenuDbContext _context;

        public MenusController(MenuDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(Guid? restaurantId)
        {
            if (restaurantId == null)
            {
                return BadRequest();
            }

            var menu = await _context.Menus
                .Include(m => m.Categories)
                    .ThenInclude(c => c.MenuItems)
                .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        public async Task<IActionResult> Edit(Guid? restaurantId)
        {
            if (restaurantId == null)
            {
                return BadRequest();
            }

            var menu = await _context.Menus
                .Include(m => m.Categories)
                    .ThenInclude(c => c.MenuItems)
                .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId);

            if (menu == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            if (restaurant == null || restaurant.UserId != GetUserId())
            {
                return BadRequest();
            }

            return View(menu);
        }

        public async Task<IActionResult> Create(Guid? restaurantId)
        {
            if (restaurantId == null)
            {
                return BadRequest();
            }

            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            if (restaurant == null)
            {
                return NotFound();
            }

            var menuExists = await _context.Menus
                .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId);

            if (restaurant.UserId != GetUserId())
            {
                return BadRequest();
            }

            if (menuExists != null)
            {
                return RedirectToAction("Edit", "Menus", new { restaurantId = restaurantId });
            }

            Menu menu = new Menu
            {
                RestaurantId = restaurant.Id
            };

            await _context.Menus.AddAsync(menu);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Menus", new { restaurantId = restaurantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Guid? id, [FromBody] CategoryDTO model)
        {
            if (id == null || id != model.Id) 
            { 
                return BadRequest(); 
            }

            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
            {
                return BadRequest();
            }

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == menu.RestaurantId);

            if (restaurant == null || restaurant.UserId != GetUserId()) 
            {
                return BadRequest();
            }

            // TODO: Verificar se a categoria já existe

            Category category = new Category
            {
                MenuId = (Guid)model.Id,
                Name = model.CategoryName
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(Guid? id, [FromBody] CategoryDTO model)
        {
            if (id == null || id != model.Id)
            {
                return BadRequest();
            }

            var menu = await _context.Menus.FindAsync(model.MenuId);

            if (menu == null)
            {
                return BadRequest();
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return BadRequest();
            }

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == menu.RestaurantId);

            if (restaurant == null || restaurant.UserId != GetUserId())
            {
                return BadRequest();
            }

            category.Name = model.CategoryName;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var categoryExists = await _context.Categories.FindAsync(id);

            if (categoryExists == null)
            {
                return BadRequest();
            }

            var menuExists = await _context.Menus.FirstOrDefaultAsync(m => m.Id == categoryExists.MenuId);

            if (menuExists == null)
            {
                return BadRequest();
            }

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == menuExists.RestaurantId);

            if (restaurant == null || restaurant.UserId != GetUserId())
            {
                return BadRequest();
            }

            _context.Categories.Remove(categoryExists);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private Guid GetUserId()
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault().Value);

            return userId;
        }

        public class CategoryDTO
        {
            public Guid? Id { get; set; }
            public Guid? MenuId { get; set; }
            public string? CategoryName { get; set; }
        }
    }
}

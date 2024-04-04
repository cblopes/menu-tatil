using MenuTatil.Data;
using MenuTatil.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        private Guid GetUserId()
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault().Value);

            return userId;
        }
    }
}

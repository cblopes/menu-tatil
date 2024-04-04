using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenuTatil.Data;
using MenuTatil.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MenuTatil.Controllers
{
    [Authorize]
    public class RestaurantsController : Controller
    {
        private readonly MenuDbContext _context;

        public RestaurantsController(MenuDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Restaurants.ToListAsync());
        }

        public async Task<IActionResult> MyRestaurants(Guid id)
        {
            var restaurants = await _context.Restaurants
                .Where(m => m.UserId == id)
                .ToListAsync();

            if (restaurants == null)
            {
                return NotFound();
            }

            return View(restaurants);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Phone,Street,Number,Complement,District,City,State")] CreateRestaurantDTO model)
        {
            if (ModelState.IsValid)
            {
                Address address = new Address
                {
                    Street = model.Street,
                    Number = model.Number,
                    Complement = model.Complement,
                    District = model.District,
                    City = model.City,
                    State = model.State
                };

                await _context.Addresses.AddAsync(address);

                Restaurant restaurant = new Restaurant
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    UserId = GetUserId(),
                    AddressId = address.Id
                };

                await _context.Restaurants.AddAsync(restaurant);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Phone,IsActive")] Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(Guid id)
        {
            return _context.Restaurants.Any(e => e.Id == id);
        }

        private Guid GetUserId()
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault().Value);

            return userId;
        }
    }
}

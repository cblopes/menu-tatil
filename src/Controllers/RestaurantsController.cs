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

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .Include(r => r.Address)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var restaurantDTO = new CreateRestaurantDTO
            {
                Name = restaurant.Name,
                Phone = restaurant.Phone,
                Street = restaurant.Address?.Street,
                Number = restaurant.Address?.Number,
                Complement = restaurant.Address?.Complement,
                District = restaurant.Address?.District,
                City = restaurant.Address?.City,
                State = restaurant.Address?.State,
            };

            return View(restaurantDTO);
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

            var restaurant = await _context.Restaurants
                .Include(r => r.Address)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var restaurantDTO = new UpdateRestaurantDTO
            {
                Name = restaurant.Name,
                Phone = restaurant.Phone,
                Street = restaurant.Address?.Street,
                Number = restaurant.Address?.Number,
                Complement = restaurant.Address?.Complement,
                District = restaurant.Address?.District,
                City = restaurant.Address?.City,
                State = restaurant.Address?.State,
            };

            return View(restaurantDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Phone,Street,Number,Complement,District,City,State")] UpdateRestaurantDTO model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return BadRequest();
            }

            restaurant.Name = model.Name;
            restaurant.Phone = model.Phone;

            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();

            var address = await _context.Addresses.SingleOrDefaultAsync(a => a.Id == restaurant.AddressId);

            address.Street = model.Street;
            address.Number = model.Number;
            address.Complement = model.Complement;
            address.District = model.District;
            address.City = model.City;
            address.State = model.State;

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyRestaurants));
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

            return RedirectToAction(nameof(MyRestaurants));
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

using MenuTatil.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuTatil.Data
{
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set;}
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
    }
}

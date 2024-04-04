
namespace MenuTatil.Models
{
    public class Menu
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public ICollection<Category>? Categories { get; set; }

        public Menu()
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
        }
    }
}

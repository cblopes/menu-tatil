namespace MenuTatil.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid MenuId { get; set; }
        public Menu? Menu { get; set; }
        public ICollection<MenuItem>? MenuItems { get; set; }

        public Category()
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
        }
    }
}

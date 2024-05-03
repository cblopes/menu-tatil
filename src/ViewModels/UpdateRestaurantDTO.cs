namespace MenuTatil.ViewModels
{
    public class UpdateRestaurantDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Complement { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}

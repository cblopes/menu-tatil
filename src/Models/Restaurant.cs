namespace MenuTatil;

public class Restaurant
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public Guid UserId { get; set; }
    public Guid AddressId { get; set; }
    public Address? Address { get; set; }
    public bool IsActive { get; set; }

    public Restaurant()
    {
        if (Id == Guid.Empty) Id = Guid.NewGuid();
        IsActive = true;
    }
}

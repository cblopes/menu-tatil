namespace MenuTatil;

public class Address
{
    public Guid Id { get; set; }
    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }

    public Address()
    {
        if (Id == Guid.Empty) Id = Guid.NewGuid();
    }
}

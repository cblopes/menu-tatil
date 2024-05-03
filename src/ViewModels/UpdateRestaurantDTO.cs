using System.ComponentModel;

namespace MenuTatil.ViewModels
{
    public class UpdateRestaurantDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Nome")]
        public string? Name { get; set; }

        [DisplayName("Telefone")]
        public string? Phone { get; set; }

        [DisplayName("Rua")]
        public string? Street { get; set; }

        [DisplayName("Número")]
        public string? Number { get; set; }

        [DisplayName("Complemento")]
        public string? Complement { get; set; }

        [DisplayName("Bairro")]
        public string? District { get; set; }

        [DisplayName("Cidade")]
        public string? City { get; set; }

        [DisplayName("Estado")]
        public string? State { get; set; }
    }
}

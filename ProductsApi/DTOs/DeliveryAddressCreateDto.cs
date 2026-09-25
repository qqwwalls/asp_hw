using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class DeliveryAddressCreateDto
{
    [Required]
    [MaxLength(100)]
    [ProductsApi.Validation.AllowedCountry("Ukraine", "Poland", "USA", "UK")]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ZipCode { get; set; } = string.Empty;
}

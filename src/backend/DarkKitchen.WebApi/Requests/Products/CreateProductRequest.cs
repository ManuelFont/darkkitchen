using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Products;

namespace DarkKitchen.WebApi.Requests.Products;

public sealed record CreateProductRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Description { get; init; }

    [Required]
    [MinLength(1)]
    [MaxLength(3)]
    public required IReadOnlyList<string> ImagesUrls { get; init; }

    [Required]
    public required decimal Price { get; init; }

    [Required]
    public required Guid CategoryId { get; init; }

    public ProductCreateDto ToDto() => new()
    {
        Name = Name,
        Description = Description,
        ImagesUrls = ImagesUrls,
        Price = Price,
        CategoryId = CategoryId
    };
}

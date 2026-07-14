using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Categories;

namespace DarkKitchen.WebApi.Requests.Categories;

public sealed record CreateCategoryRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Description { get; init; }

    public CategoryCreateDto ToDto() => new()
    {
        Name = Name,
        Description = Description
    };
}

using DarkKitchen.Application.Services.Categories;

namespace DarkKitchen.WebApi.Responses.Categories;

public sealed record CategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public static CategoryResponse FromDto(CategoryReadDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description
    };
}

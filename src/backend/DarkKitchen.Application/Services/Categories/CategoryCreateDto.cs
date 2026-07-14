namespace DarkKitchen.Application.Services.Categories;

public sealed record CategoryCreateDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

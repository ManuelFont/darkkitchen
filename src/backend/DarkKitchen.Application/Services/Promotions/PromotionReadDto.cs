using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Application.Services.Promotions;

public sealed record PromotionReadDto
{
    [Required]
    public required Guid Id { get; init; }
    [Required]
    public required string Name { get; init; }
    [Required]
    public required decimal DiscountPercentage { get; init; }
    [Required]
    public required DateTime StartDate { get; init; }
    [Required]
    public required DateTime EndDate { get; init; }
}

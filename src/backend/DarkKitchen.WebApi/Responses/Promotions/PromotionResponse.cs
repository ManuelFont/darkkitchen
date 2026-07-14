using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Promotions;

namespace DarkKitchen.WebApi.Responses.Promotions;

public class PromotionResponse
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

    public static PromotionResponse FromDto(PromotionReadDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        DiscountPercentage = dto.DiscountPercentage,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate
    };
}

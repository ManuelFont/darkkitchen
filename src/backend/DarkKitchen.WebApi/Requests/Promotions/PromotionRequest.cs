using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Promotions;

namespace DarkKitchen.WebApi.Requests.Promotions;

public class PromotionRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required decimal DiscountPercentage { get; init; }

    [Required]
    public required DateTime StartDate { get; init; }

    [Required]
    public required DateTime EndDate { get; init; }

    public PromotionCreateDto ToDto() => new()
    {
        Name = Name,
        DiscountPercentage = DiscountPercentage,
        StartDate = StartDate,
        EndDate = EndDate
    };
}

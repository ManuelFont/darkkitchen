using DarkKitchen.Application.Services.Audit;

namespace DarkKitchen.WebApi.Responses.Audit;

public sealed record AuditSummaryResponse
{
    public int Created { get; init; }
    public int Updated { get; init; }
    public int Deleted { get; init; }
    public int Total { get; init; }

    public static AuditSummaryResponse FromDto(AuditSummaryDto dto) => new()
    {
        Created = dto.Created,
        Updated = dto.Updated,
        Deleted = dto.Deleted,
        Total = dto.Total
    };
}

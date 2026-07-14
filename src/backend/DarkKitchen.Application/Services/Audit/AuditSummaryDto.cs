namespace DarkKitchen.Application.Services.Audit;

public sealed record AuditSummaryDto
{
    public int Created { get; init; }
    public int Updated { get; init; }
    public int Deleted { get; init; }
    public int Total { get; init; }
}

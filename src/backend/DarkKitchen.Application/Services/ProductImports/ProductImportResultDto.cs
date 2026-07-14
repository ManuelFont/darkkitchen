namespace DarkKitchen.Application.Services.ProductImports;

public sealed record ProductImportResultDto
{
    public required string ImporterId { get; init; }
    public required int Total { get; init; }
    public required int Created { get; init; }
    public required int SkippedDuplicates { get; init; }
    public required int Failed { get; init; }
    public required IReadOnlyList<ProductImportIssueDto> Issues { get; init; }
}

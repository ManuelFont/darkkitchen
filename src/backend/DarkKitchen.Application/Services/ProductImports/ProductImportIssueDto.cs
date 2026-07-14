namespace DarkKitchen.Application.Services.ProductImports;

public sealed record ProductImportIssueDto
{
    public required string Name { get; init; }
    public required string Message { get; init; }
}

using DarkKitchen.Application.Services.ProductImports;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Responses.ProductImports;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.ProductImports;

[ApiController]
[Route("product-imports")]
[ServiceFilter(typeof(AuthenticationFilter))]
public sealed class ProductImportController(IProductImportService service) : ControllerBase
{
    private readonly IProductImportService _service = service;

    [HttpGet("importers")]
    [AuthorizationFilter(PermissionNames.CanCreateProduct)]
    public ActionResult<IReadOnlyList<ProductImporterResponse>> GetImporters()
    {
        return Ok(_service.GetImporters().Select(ProductImporterResponse.FromDto).ToList());
    }

    [HttpPost("importers/{importerId}/import")]
    [AuthorizationFilter(PermissionNames.CanCreateProduct)]
    public async Task<ActionResult<ProductImportResultResponse>> Import(
        string importerId,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if(file.Length == 0)
        {
            throw new InvalidArgumentException("File cannot be empty.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _service.ImportAsync(
            importerId,
            file.FileName,
            file.ContentType,
            stream,
            cancellationToken);

        return Ok(ProductImportResultResponse.FromDto(result));
    }
}

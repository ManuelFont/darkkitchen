using DarkKitchen.Application.Services.Products;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Products;
using DarkKitchen.WebApi.Responses.Products;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Products;

[ApiController]
[Route("products")]
[ServiceFilter(typeof(AuthenticationFilter))]
public class ProductController(IProductService service) : ControllerBase
{
    private readonly IProductService _service = service;

    [HttpGet("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanGetProduct)]
    public ActionResult<ProductResponse> GetById(Guid id)
    {
        var dto = _service.GetById(id);
        return Ok(ProductResponse.FromDto(dto));
    }

    [HttpGet]
    [AuthorizationFilter(PermissionNames.CanGetProduct)]
    public ActionResult<IReadOnlyList<ProductResponse>> GetAll(
        [FromQuery] string? name,
        [FromQuery] Guid? categoryId)
    {
        if(name != null)
        {
            return Ok(_service.GetByName(name).Select(ProductResponse.FromDto).ToList());
        }

        if(categoryId != null)
        {
            return Ok(_service.GetByCategory(categoryId.Value).Select(ProductResponse.FromDto).ToList());
        }

        return Ok(_service.GetAll().Select(ProductResponse.FromDto).ToList());
    }

    [HttpPost]
    [AuthorizationFilter(PermissionNames.CanCreateProduct)]
    public ActionResult<ProductResponse> Create(CreateProductRequest request)
    {
        var response = ProductResponse.FromDto(_service.Create(request.ToDto()));
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanUpdateProduct)]
    public IActionResult Update(Guid id, CreateProductRequest request)
    {
        _service.Update(id, request.ToDto());
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanDeleteProduct)]
    public IActionResult Delete(Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }

    [HttpPost("{productId:guid}/promotions/{promotionId:guid}")]
    [AuthorizationFilter(PermissionNames.CanDeleteProduct)]
    public IActionResult AddPromotion(Guid productId, Guid promotionId)
    {
        _service.AddPromotion(productId, promotionId);
        return NoContent();
    }

    [HttpDelete("{productId:guid}/promotions/{promotionId:guid}")]
    [AuthorizationFilter(PermissionNames.CanDeleteProduct)]
    public IActionResult RemovePromotion(Guid productId, Guid promotionId)
    {
        _service.RemovePromotion(productId, promotionId);
        return NoContent();
    }
}

using DarkKitchen.Application.Services.Promotions;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Promotions;
using DarkKitchen.WebApi.Responses.Promotions;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Promotions;

[ApiController]
[Route("promotions")]
[ServiceFilter(typeof(AuthenticationFilter))]
public class PromotionController(IPromotionService service) : ControllerBase
{
    private readonly IPromotionService _service = service;

    [HttpGet("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanGetPromotion)]
    public ActionResult<PromotionResponse> GetById(Guid id)
    {
        var dto = _service.GetById(id);
        return Ok(PromotionResponse.FromDto(dto));
    }

    [HttpGet]
    [AuthorizationFilter(PermissionNames.CanGetPromotion)]
    public ActionResult<IReadOnlyList<PromotionResponse>> GetAll(
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? categoryId = null)
    {
        if(productId != null)
        {
            return Ok(_service.GetByProduct(productId.Value).Select(PromotionResponse.FromDto).ToList());
        }

        if(categoryId != null)
        {
            return Ok(_service.GetByCategory(categoryId.Value).Select(PromotionResponse.FromDto).ToList());
        }

        return Ok(_service.GetAll().Select(PromotionResponse.FromDto).ToList());
    }

    [HttpPost]
    [AuthorizationFilter(PermissionNames.CanCreatePromotion)]
    public ActionResult<PromotionResponse> Create(PromotionRequest request)
    {
        var response = PromotionResponse.FromDto(_service.Create(request.ToDto()));
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanUpdatePromotion)]
    public ActionResult<PromotionResponse> Update(Guid id, PromotionRequest request)
    {
        var response = PromotionResponse.FromDto(_service.Update(id, request.ToDto()));
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanDeletePromotion)]
    public IActionResult Delete(Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }
}

using DarkKitchen.Application.Services.DeliveryTypes;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.DeliveryTypes;
using DarkKitchen.WebApi.Responses.DeliveryTypes;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.DeliveryTypes;

[ApiController]
[Route("delivery-types")]
[ServiceFilter(typeof(AuthenticationFilter))]
public sealed class DeliveryTypeController(IDeliveryTypeService service) : ControllerBase
{
    private readonly IDeliveryTypeService _service = service;

    [HttpGet]
    [AuthorizationFilter(PermissionNames.CanGetDeliveryType)]
    public ActionResult<IReadOnlyList<DeliveryTypeResponse>> GetAll()
    {
        return Ok(_service.GetAll().Select(DeliveryTypeResponse.FromDto).ToList());
    }

    [HttpGet("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanGetDeliveryType)]
    public ActionResult<DeliveryTypeResponse> GetById(Guid id)
    {
        return Ok(DeliveryTypeResponse.FromDto(_service.GetById(id)));
    }

    [HttpPost]
    [AuthorizationFilter(PermissionNames.CanCreateDeliveryType)]
    public ActionResult<DeliveryTypeResponse> Create(DeliveryTypeRequest request)
    {
        var response = DeliveryTypeResponse.FromDto(_service.Create(request.ToDto()));
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanUpdateDeliveryType)]
    public IActionResult Update(Guid id, DeliveryTypeRequest request)
    {
        _service.Update(id, request.ToDto());
        return NoContent();
    }
}

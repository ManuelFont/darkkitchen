using DarkKitchen.Application.Services.Categories;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Categories;
using DarkKitchen.WebApi.Responses.Categories;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Categories;

[ApiController]
[Route("categories")]
[ServiceFilter(typeof(AuthenticationFilter))]
public class CategoryController(ICategoryService service) : ControllerBase
{
    private readonly ICategoryService _service = service;

    [HttpGet("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanGetCategory)]
    public ActionResult<CategoryResponse> GetById(Guid id)
    {
        var dto = _service.GetById(id);
        return Ok(CategoryResponse.FromDto(dto));
    }

    [HttpGet]
    [AuthorizationFilter(PermissionNames.CanGetCategory)]
    public ActionResult<IReadOnlyList<CategoryResponse>> GetAll()
    {
        return Ok(_service.GetAll().Select(CategoryResponse.FromDto).ToList());
    }

    [HttpPost]
    [AuthorizationFilter(PermissionNames.CanCreateCategory)]
    public ActionResult<CategoryResponse> Create(CreateCategoryRequest request)
    {
        var response = CategoryResponse.FromDto(_service.Create(request.ToDto()));
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanUpdateCategory)]
    public ActionResult<CategoryResponse> Update(Guid id, CreateCategoryRequest request)
    {
        var response = CategoryResponse.FromDto(_service.Update(id, request.ToDto()));
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [AuthorizationFilter(PermissionNames.CanDeleteCategory)]
    public IActionResult Delete(Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }
}

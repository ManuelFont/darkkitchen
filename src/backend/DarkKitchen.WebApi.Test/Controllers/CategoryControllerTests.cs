using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.Categories;
using DarkKitchen.WebApi.Requests.Categories;
using DarkKitchen.WebApi.Responses.Categories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class CategoryControllerTests
{
    private Mock<ICategoryService> _categoryServiceMock = null!;
    private CategoryController _categoryController = null!;

    [TestInitialize]
    public void Setup()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _categoryController = new CategoryController(_categoryServiceMock.Object);
    }

    [TestMethod]
    public void GetById_WithExistingId_Returns200WithCategory()
    {
        var id = Guid.NewGuid();
        var dto = new CategoryReadDto { Id = id, Name = "Pastas", Description = "Italian pasta dishes" };

        _categoryServiceMock
            .Setup(s => s.GetById(id))
            .Returns(dto);

        var result = (OkObjectResult)_categoryController.GetById(id).Result!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(CategoryResponse.FromDto(dto), (CategoryResponse)result.Value!);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _categoryServiceMock
            .Setup(s => s.GetById(id))
            .Throws(new ResourceNotFoundException("Category", id));

        _categoryController.GetById(id);
    }

    [TestMethod]
    public void GetAll_Returns200WithAllCategories()
    {
        var dtos = new List<CategoryReadDto>
        {
            new CategoryReadDto { Id = Guid.NewGuid(), Name = "Pastas" },
            new CategoryReadDto { Id = Guid.NewGuid(), Name = "Fritos" }
        };

        _categoryServiceMock
            .Setup(s => s.GetAll())
            .Returns(dtos);

        var result = (OkObjectResult)_categoryController.GetAll().Result!;
        var response = (List<CategoryResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        CollectionAssert.AreEqual(dtos.Select(CategoryResponse.FromDto).ToList(), response);
    }

    [TestMethod]
    public void Create_WithValidRequest_Returns201WithCreatedCategory()
    {
        var request = new CreateCategoryRequest { Name = "Pastas", Description = "Italian pasta dishes" };
        var dto = new CategoryReadDto { Id = Guid.NewGuid(), Name = "Pastas", Description = "Italian pasta dishes" };

        _categoryServiceMock
            .Setup(s => s.Create(request.ToDto()))
            .Returns(dto);

        var result = (CreatedAtActionResult)_categoryController.Create(request).Result!;

        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(CategoryResponse.FromDto(dto), (CategoryResponse)result.Value!);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var request = new CreateCategoryRequest { Name = "Pastas", Description = "Italian pasta dishes" };

        _categoryServiceMock
            .Setup(s => s.Create(request.ToDto()))
            .Throws(new DuplicateResourceException("Category", "name", "Pastas"));

        _categoryController.Create(request);
    }

    [TestMethod]
    public void Update_WithValidRequest_Returns200WithUpdatedCategory()
    {
        var id = Guid.NewGuid();
        var request = new CreateCategoryRequest { Name = "Grill", Description = "Meat and sausages grilled" };
        var dto = new CategoryReadDto { Id = id, Name = "Grill", Description = "Meat and sausages grilled" };

        _categoryServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Returns(dto);

        var result = (OkObjectResult)_categoryController.Update(id, request).Result!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(CategoryResponse.FromDto(dto), (CategoryResponse)result.Value!);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();
        var request = new CreateCategoryRequest { Name = "Grill", Description = "Meat and sausages grilled" };

        _categoryServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Throws(new ResourceNotFoundException("Category", id));

        _categoryController.Update(id, request);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Update_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var id = Guid.NewGuid();
        var request = new CreateCategoryRequest { Name = "Pastas", Description = "Italian pasta dishes" };

        _categoryServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Throws(new DuplicateResourceException("Category", "name", "Pastas"));

        _categoryController.Update(id, request);
    }

    [TestMethod]
    public void Delete_WithExistingId_Returns204()
    {
        var id = Guid.NewGuid();

        var result = (NoContentResult)_categoryController.Delete(id);

        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _categoryServiceMock
            .Setup(s => s.Delete(id))
            .Throws(new ResourceNotFoundException("Category", id));

        _categoryController.Delete(id);
    }
}

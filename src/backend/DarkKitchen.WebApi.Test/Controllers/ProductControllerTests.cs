using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Application.Services.Products;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.Products;
using DarkKitchen.WebApi.Requests.Products;
using DarkKitchen.WebApi.Responses.Products;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class ProductControllerTests
{
    private Mock<IProductService> _productServiceMock = null!;
    private ProductController _productController = null!;
    private ProductReadDto _dto = null!;
    private ProductReadDto _dto2 = null!;

    [TestInitialize]
    public void Setup()
    {
        _productServiceMock = new Mock<IProductService>();
        _productController = new ProductController(_productServiceMock.Object);
        _dto = new ProductReadDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "Test Description",
            ImagesUrls = ["https://example.com/product.jpg"],
            Price = 9.99m,
            CategoryId = Guid.NewGuid(),
            Category = new CategoryReadDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                Description = "Test Category Description"
            },
            ActivePromotion = null,
            Promotions = []
        };
        _dto2 = new ProductReadDto
        {
            Id = Guid.NewGuid(),
            Name = "Gadget",
            Description = "Test Description",
            ImagesUrls = ["https://example.com/gadget.jpg"],
            Price = 9.99m,
            CategoryId = Guid.NewGuid(),
            Category = new CategoryReadDto
            {
                Id = Guid.NewGuid(),
                Name = "Other Category",
                Description = "Other Category Description"
            },
            ActivePromotion = null,
            Promotions = []
        };
    }

    [TestMethod]
    public void GetById_WithExistingId_Returns200WithProduct()
    {
        _productServiceMock
            .Setup(s => s.GetById(_dto.Id))
            .Returns(_dto);

        var result = (OkObjectResult)_productController.GetById(_dto.Id).Result!;
        var response = (ProductResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        AssertProductResponse(_dto, response);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _productServiceMock
            .Setup(s => s.GetById(id))
            .Throws(new ResourceNotFoundException("Product", id));

        _productController.GetById(id);
    }

    [TestMethod]
    public void GetAll_WithNoQueryParams_Returns200WithAllProducts()
    {
        var dtos = new List<ProductReadDto> { _dto, _dto2 };

        _productServiceMock
            .Setup(s => s.GetAll())
            .Returns(dtos);

        var result = (OkObjectResult)_productController.GetAll(null, null).Result!;
        var response = (List<ProductResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        AssertProductResponses(dtos, response);
    }

    [TestMethod]
    public void GetAll_WithNameQueryParam_Returns200WithMatchingProduct()
    {
        _productServiceMock
            .Setup(s => s.GetByName("Widget"))
            .Returns([_dto]);

        var result = (OkObjectResult)_productController.GetAll("Widget", null).Result!;
        var response = (List<ProductResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        AssertProductResponses([_dto], response);
    }

    [TestMethod]
    public void GetAll_WithCategoryIdQueryParam_Returns200WithMatchingProducts()
    {
        var categoryId = Guid.NewGuid();
        var dtos = new List<ProductReadDto>
        {
            _dto, _dto2
        };

        _productServiceMock
            .Setup(s => s.GetByCategory(categoryId))
            .Returns(dtos);

        var result = (OkObjectResult)_productController.GetAll(null, categoryId).Result!;
        var response = (List<ProductResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        AssertProductResponses(dtos, response);
    }

    [TestMethod]
    public void GetAll_WithNameQueryParam_WhenProductDoesNotExist_Returns200WithEmptyList()
    {
        _productServiceMock
            .Setup(s => s.GetByName("Ghost"))
            .Returns([]);

        var result = (OkObjectResult)_productController.GetAll("Ghost", null).Result!;
        var response = (List<ProductResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(0, response.Count);
    }

    [TestMethod]
    public void Create_WithValidRequest_Returns201WithCreatedProduct()
    {
        var request = new CreateProductRequest
        {
            Name = "Widget",
            Description = "A great widget",
            ImagesUrls = ["https://example.com/widget.jpg"],
            Price = 9.99m,
            CategoryId = Guid.NewGuid()
        };

        _productServiceMock
            .Setup(s => s.Create(request.ToDto()))
            .Returns(_dto);

        var result = (CreatedAtActionResult)_productController.Create(request).Result!;
        var response = (ProductResponse)result.Value!;

        Assert.AreEqual(201, result.StatusCode);
        AssertProductResponse(_dto, response);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var request = new CreateProductRequest
        {
            Name = "Widget",
            Description = "A great widget",
            ImagesUrls = ["https://example.com/widget.jpg"],
            Price = 9.99m,
            CategoryId = Guid.NewGuid()
        };

        _productServiceMock
            .Setup(s => s.Create(request.ToDto()))
            .Throws(new DuplicateResourceException("Product", "name", "Widget"));

        _productController.Create(request);
    }

    [TestMethod]
    public void Update_WithValidRequest_Returns204()
    {
        var id = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Name = "UpdatedWidget",
            Description = "An updated widget",
            ImagesUrls = ["https://example.com/updated-widget.jpg"],
            Price = 19.99m,
            CategoryId = Guid.NewGuid()
        };

        var result = (NoContentResult)_productController.Update(id, request);

        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Name = "UpdatedWidget",
            Description = "An updated widget",
            ImagesUrls = ["https://example.com/updated-widget.jpg"],
            Price = 19.99m,
            CategoryId = Guid.NewGuid()
        };

        _productServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Throws(new ResourceNotFoundException("Product", id));

        _productController.Update(id, request);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Update_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var id = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Name = "Widget",
            Description = "A great widget",
            ImagesUrls = ["https://example.com/widget.jpg"],
            Price = 9.99m,
            CategoryId = Guid.NewGuid()
        };

        _productServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Throws(new DuplicateResourceException("Product", "name", "Widget"));

        _productController.Update(id, request);
    }

    [TestMethod]
    public void Delete_WithExistingId_Returns204()
    {
        var id = Guid.NewGuid();

        var result = (NoContentResult)_productController.Delete(id);

        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _productServiceMock
            .Setup(s => s.Delete(id))
            .Throws(new ResourceNotFoundException("Product", id));

        _productController.Delete(id);
    }

    [TestMethod]
    public void AddPromotion_WithValidIds_Returns204()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        var result = (NoContentResult)_productController.AddPromotion(productId, promotionId);

        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    public void RemovePromotion_WithValidIds_Returns204()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        var result = (NoContentResult)_productController.RemovePromotion(productId, promotionId);

        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void AddPromotion_WhenServiceThrowsNotFound_PropagatesException()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productServiceMock
            .Setup(s => s.AddPromotion(productId, promotionId))
            .Throws(new ResourceNotFoundException("Product", productId));

        _productController.AddPromotion(productId, promotionId);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void RemovePromotion_WhenServiceThrowsNotFound_PropagatesException()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productServiceMock
            .Setup(s => s.RemovePromotion(productId, promotionId))
            .Throws(new ResourceNotFoundException("Product", productId));

        _productController.RemovePromotion(productId, promotionId);
    }

    private static void AssertProductResponses(
        IReadOnlyList<ProductReadDto> expected,
        IReadOnlyList<ProductResponse> actual)
    {
        Assert.AreEqual(expected.Count, actual.Count);

        for(var index = 0; index < expected.Count; index++)
        {
            AssertProductResponse(expected[index], actual[index]);
        }
    }

    private static void AssertProductResponse(ProductReadDto expected, ProductResponse actual)
    {
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.Description, actual.Description);
        CollectionAssert.AreEqual(expected.ImagesUrls.ToList(), actual.ImagesUrls.ToList());
        Assert.AreEqual(expected.Price, actual.Price);
        Assert.AreEqual(expected.CategoryId, actual.CategoryId);
        Assert.AreEqual(expected.Category.Id, actual.Category.Id);
        Assert.AreEqual(expected.Category.Name, actual.Category.Name);
        Assert.AreEqual(expected.Category.Description, actual.Category.Description);
        Assert.AreEqual(expected.ActivePromotion?.Id, actual.ActivePromotion?.Id);
        CollectionAssert.AreEqual(
            expected.Promotions.Select(promotion => promotion.Id).ToList(),
            actual.Promotions.Select(promotion => promotion.Id).ToList());
    }
}

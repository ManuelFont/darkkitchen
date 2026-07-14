using DarkKitchen.Application.Services.Promotions;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.Promotions;
using DarkKitchen.WebApi.Requests.Promotions;
using DarkKitchen.WebApi.Responses.Promotions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class PromotionControllerTests
{
    private Mock<IPromotionService> _promotionServiceMock = null!;
    private PromotionController _promotionController = null!;
    private PromotionRequest _promotionSummer = null!;

    [TestInitialize]
    public void Setup()
    {
        _promotionServiceMock = new Mock<IPromotionService>();
        _promotionController = new PromotionController(_promotionServiceMock.Object);

        _promotionSummer = new PromotionRequest
        {
            Name = "Summer Sale",
            DiscountPercentage = 0.20m,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30)
        };
    }

    [TestMethod]
    public void GetById_WithExistingId_Returns200WithPromotion()
    {
        var id = Guid.NewGuid();
        var dto = new PromotionReadDto { Id = id, Name = _promotionSummer.Name, DiscountPercentage = _promotionSummer.DiscountPercentage, StartDate = _promotionSummer.StartDate, EndDate = _promotionSummer.EndDate };

        _promotionServiceMock
            .Setup(s => s.GetById(id))
            .Returns(dto);

        var result = (OkObjectResult)_promotionController.GetById(id).Result!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(PromotionResponse.FromDto(dto).Id, ((PromotionResponse)result.Value!).Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _promotionServiceMock
            .Setup(s => s.GetById(id))
            .Throws(new ResourceNotFoundException("Promotion", id));

        _promotionController.GetById(id);
    }

    [TestMethod]
    public void GetAll_Returns200WithAllPromotions()
    {
        var dtos = new List<PromotionReadDto>
        {
            new PromotionReadDto { Id = Guid.NewGuid(), Name = _promotionSummer.Name, DiscountPercentage = _promotionSummer.DiscountPercentage, StartDate = _promotionSummer.StartDate, EndDate = _promotionSummer.EndDate },
            new PromotionReadDto { Id = Guid.NewGuid(), Name = "Winter Promo", DiscountPercentage = 0.2m, StartDate = _promotionSummer.StartDate, EndDate = _promotionSummer.EndDate }
        };

        _promotionServiceMock
            .Setup(s => s.GetAll())
            .Returns(dtos);

        var result = (OkObjectResult)_promotionController.GetAll().Result!;
        var response = (List<PromotionResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(dtos[0].Id, response[0].Id);
    }

    [TestMethod]
    public void Create_WithValidRequest_Returns201WithCreatedPromotion()
    {
        var dto = new PromotionReadDto
        {
            Id = Guid.NewGuid(),
            Name = _promotionSummer.Name,
            DiscountPercentage = _promotionSummer.DiscountPercentage,
            StartDate = _promotionSummer.StartDate,
            EndDate = _promotionSummer.EndDate
        };

        _promotionServiceMock
            .Setup(s => s.Create(_promotionSummer.ToDto()))
            .Returns(dto);

        var result = (CreatedAtActionResult)_promotionController.Create(_promotionSummer).Result!;

        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(PromotionResponse.FromDto(dto).Id, ((PromotionResponse)result.Value!).Id);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        _promotionServiceMock
            .Setup(s => s.Create(_promotionSummer.ToDto()))
            .Throws(new DuplicateResourceException("Promotion", "name", "Summer Sale"));

        _promotionController.Create(_promotionSummer);
    }

    [TestMethod]
    public void Update_WithValidRequest_Returns200WithUpdatedPromotion()
    {
        var id = Guid.NewGuid();
        var request = new PromotionRequest
        {
            Name = "Updated Sale",
            DiscountPercentage = 0.30m,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(15)
        };
        var dto = new PromotionReadDto
        {
            Id = id,
            Name = request.Name,
            DiscountPercentage = request.DiscountPercentage,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _promotionServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Returns(dto);

        var result = (OkObjectResult)_promotionController.Update(id, request).Result!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(PromotionResponse.FromDto(dto).Id, ((PromotionResponse)result.Value!).Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();
        var request = new PromotionRequest
        {
            Name = "Updated Sale",
            DiscountPercentage = 0.30m,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(15)
        };

        _promotionServiceMock
            .Setup(s => s.Update(id, request.ToDto()))
            .Throws(new ResourceNotFoundException("Promotion", id));

        _promotionController.Update(id, request);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Update_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var id = Guid.NewGuid();

        _promotionServiceMock
            .Setup(s => s.Update(id, _promotionSummer.ToDto()))
            .Throws(new DuplicateResourceException("Promotion", "name", "Summer Sale"));

        _promotionController.Update(id, _promotionSummer);
    }

    [TestMethod]
    public void Delete_WithExistingId_Returns204()
    {
        var id = Guid.NewGuid();

        var result = (NoContentResult)_promotionController.Delete(id);

        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _promotionServiceMock
            .Setup(s => s.Delete(id))
            .Throws(new ResourceNotFoundException("Promotion", id));

        _promotionController.Delete(id);
    }

    [TestMethod]
    public void GetAll_ByProductId_Returns200WithFilteredPromotions()
    {
        var productId = Guid.NewGuid();
        var dtos = new List<PromotionReadDto>
        {
            new PromotionReadDto { Id = Guid.NewGuid(), Name = "Promo 1", DiscountPercentage = 0.2m, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) }
        };

        _promotionServiceMock
            .Setup(s => s.GetByProduct(productId))
            .Returns(dtos);

        var result = (OkObjectResult)_promotionController.GetAll(productId, null).Result!;
        var response = (List<PromotionResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(dtos[0].Id, response[0].Id);
    }

    [TestMethod]
    public void GetAll_ByCategoryId_Returns200WithFilteredPromotions()
    {
        var categoryId = Guid.NewGuid();
        var dtos = new List<PromotionReadDto>
        {
            new PromotionReadDto { Id = Guid.NewGuid(), Name = "Promo 1", DiscountPercentage = 0.2m, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) }
        };

        _promotionServiceMock
            .Setup(s => s.GetByCategory(categoryId))
            .Returns(dtos);

        var result = (OkObjectResult)_promotionController.GetAll(null, categoryId).Result!;
        var response = (List<PromotionResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(dtos[0].Id, response[0].Id);
    }

    [TestMethod]
    public void GetAll_ByProductIdTakesPriority_WhenBothProvided()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dtos = new List<PromotionReadDto>
        {
            new PromotionReadDto { Id = Guid.NewGuid(), Name = "Promo 1", DiscountPercentage = 0.2m, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) }
        };

        _promotionServiceMock
            .Setup(s => s.GetByProduct(productId))
            .Returns(dtos);

        var result = (OkObjectResult)_promotionController.GetAll(productId, categoryId).Result!;
        var response = (List<PromotionResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(dtos[0].Id, response[0].Id);
        _promotionServiceMock.Verify(s => s.GetByProduct(productId), Times.Once);
        _promotionServiceMock.Verify(s => s.GetByCategory(It.IsAny<Guid>()), Times.Never);
    }
}

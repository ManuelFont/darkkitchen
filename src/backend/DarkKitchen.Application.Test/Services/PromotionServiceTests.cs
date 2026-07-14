using System.Linq.Expressions;
using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Application.Services.Promotions;
using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public class PromotionServiceTests
{
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IAuditService> _auditMock = null!;
    private IPromotionService _service = null!;

    private PromotionCreateDto _createDto = null!;
    private Promotion _existingPromotion = null!;

    [TestInitialize]
    public void SetUp()
    {
        _promotionRepoMock = new Mock<IPromotionRepository>();
        _auditMock = new Mock<IAuditService>();
        _service = new PromotionService(_promotionRepoMock.Object, _auditMock.Object);

        _createDto = new PromotionCreateDto
        {
            Name = "Summer Sale",
            DiscountPercentage = 0.20m,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30)
        };

        _existingPromotion = new Promotion("Summer Sale", 0.20m, DateTime.Today, DateTime.Today.AddDays(30));
    }

    [TestMethod]
    public void GetById_ReturnsDto_WhenPromotionExists()
    {
        _promotionRepoMock.Setup(r => r.GetById(_existingPromotion.Id)).Returns(_existingPromotion);

        var result = _service.GetById(_existingPromotion.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingPromotion.Id, result.Id);
        Assert.AreEqual("Summer Sale", result.Name);
        Assert.AreEqual(0.20m, result.DiscountPercentage);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_Throws_WhenPromotionDoesNotExist()
    {
        _promotionRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Promotion?)null);

        _service.GetById(Guid.NewGuid());
    }

    [TestMethod]
    public void GetAll_ReturnsAllPromotions()
    {
        var promotion2 = new Promotion("Winter Sale", 0.15m, DateTime.Today, DateTime.Today.AddDays(15));
        _promotionRepoMock.Setup(r => r.GetAll()).Returns([_existingPromotion, promotion2]);

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(p => p.Name == "Summer Sale"));
        Assert.IsTrue(result.Any(p => p.Name == "Winter Sale"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyList_WhenNoPromotionsExist()
    {
        _promotionRepoMock.Setup(r => r.GetAll()).Returns([]);

        var result = _service.GetAll();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Create_ReturnsDto_WhenValid()
    {
        _promotionRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Promotion, bool>>>())).Returns(false);

        var result = _service.Create(_createDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Summer Sale", result.Name);
        Assert.AreEqual(0.20m, result.DiscountPercentage);
        _promotionRepoMock.Verify(r => r.Add(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_Throws_WhenPromotionNameAlreadyExists()
    {
        _promotionRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Promotion, bool>>>())).Returns(true);

        _service.Create(_createDto);
    }

    [TestMethod]
    public void Update_ReturnsDto_WhenValid()
    {
        _promotionRepoMock.Setup(r => r.GetById(_existingPromotion.Id)).Returns(_existingPromotion);

        var result = _service.Update(_existingPromotion.Id, _createDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingPromotion.Id, result.Id);
        Assert.AreEqual("Summer Sale", result.Name);
        Assert.AreEqual(0.20m, result.DiscountPercentage);
        _promotionRepoMock.Verify(r => r.Update(_existingPromotion), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_Throws_WhenPromotionDoesNotExist()
    {
        _promotionRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Promotion?)null);

        _service.Update(Guid.NewGuid(), _createDto);
    }

    [TestMethod]
    public void GetByProduct_ReturnsDtos_WhenPromotionsExist()
    {
        var promotion = new Promotion("Test Promo", 0.2m, DateTime.Today, DateTime.Today.AddDays(30));
        _promotionRepoMock.Setup(r => r.GetByProduct(It.IsAny<Guid>())).Returns([promotion]);

        var result = _service.GetByProduct(Guid.NewGuid());

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(promotion.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByProduct_ReturnsEmptyList_WhenNoPromotions()
    {
        _promotionRepoMock.Setup(r => r.GetByProduct(It.IsAny<Guid>())).Returns([]);

        var result = _service.GetByProduct(Guid.NewGuid());

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByCategory_ReturnsDtos_WhenPromotionsExist()
    {
        var promotion = new Promotion("Test Promo", 0.2m, DateTime.Today, DateTime.Today.AddDays(30));
        _promotionRepoMock.Setup(r => r.GetByCategory(It.IsAny<Guid>())).Returns([promotion]);

        var result = _service.GetByCategory(Guid.NewGuid());

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(promotion.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByCategory_ReturnsEmptyList_WhenNoPromotions()
    {
        _promotionRepoMock.Setup(r => r.GetByCategory(It.IsAny<Guid>())).Returns([]);

        var result = _service.GetByCategory(Guid.NewGuid());

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Delete_CallsRepository()
    {
        _promotionRepoMock.Setup(r => r.GetById(_existingPromotion.Id)).Returns(_existingPromotion);

        _service.Delete(_existingPromotion.Id);

        _promotionRepoMock.Verify(r => r.Delete(_existingPromotion.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_Throws_WhenPromotionDoesNotExist()
    {
        _promotionRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Promotion?)null);

        _service.Delete(Guid.NewGuid());
    }

    [TestMethod]
    public void Create_RecordsAuditEntry_WhenValid()
    {
        _promotionRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Promotion, bool>>>())).Returns(false);

        _service.Create(_createDto);

        _auditMock.Verify(a => a.Record(AuditAction.Created, AuditedEntityNames.Promotion, It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void Update_RecordsAuditEntry_WhenValid()
    {
        _promotionRepoMock.Setup(r => r.GetById(_existingPromotion.Id)).Returns(_existingPromotion);

        _service.Update(_existingPromotion.Id, _createDto);

        _auditMock.Verify(a => a.Record(AuditAction.Updated, AuditedEntityNames.Promotion, _existingPromotion.Id, It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void Delete_RecordsAuditEntry_WhenPromotionExists()
    {
        _promotionRepoMock.Setup(r => r.GetById(_existingPromotion.Id)).Returns(_existingPromotion);

        _service.Delete(_existingPromotion.Id);

        _auditMock.Verify(a => a.Record(AuditAction.Deleted, AuditedEntityNames.Promotion, _existingPromotion.Id, It.IsAny<string>()), Times.Once);
    }
}

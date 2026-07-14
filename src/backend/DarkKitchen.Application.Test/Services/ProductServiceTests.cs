using System.Linq.Expressions;
using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Application.Services.Products;
using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<ICategoryRepository> _categoryRepoMock = null!;
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IAuditService> _auditMock = null!;
    private IProductService _service = null!;

    private ProductCategory _category = null!;
    private ProductCreateDto _createDto = null!;
    private Product _existingProduct = null!;
    private Promotion _existingPromotion = null!;

    [TestInitialize]
    public void SetUp()
    {
        _category = new ProductCategory("Electronics", "Electronic products");
        _productRepoMock = new Mock<IProductRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _promotionRepoMock = new Mock<IPromotionRepository>();
        _auditMock = new Mock<IAuditService>();
        _service = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object, _promotionRepoMock.Object, _auditMock.Object);

        _createDto = new ProductCreateDto
        {
            Name = "Laptop",
            Description = "A great laptop",
            ImagesUrls = ["https://example.com/laptop.jpg"],
            Price = 999.99m,
            CategoryId = _category.CategoryId
        };

        _existingProduct = new Product("Laptop", "A great laptop", 999.99m, _category, ["https://example.com/image.jpg"]);
        _existingPromotion = new Promotion("My promo", 0.3m, DateTime.Today, DateTime.Today.AddDays(1));
    }

    [TestMethod]
    public void AddPromotion_WhenProductAndPromotionExist_AddsPromotionAndUpdatesProduct()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productRepoMock.Setup(r => r.GetById(productId)).Returns(_existingProduct);
        _promotionRepoMock.Setup(r => r.GetById(promotionId)).Returns(_existingPromotion);

        _service.AddPromotion(productId, promotionId);
        _service.RemovePromotion(productId, promotionId);

        _productRepoMock.Verify(r => r.Update(_existingProduct), Times.AtLeast(2));
        Assert.AreEqual(0, _existingProduct.Promotions.Count);
    }

    [TestMethod]
    public void RemovePromotion_WhenPromotionAdded_RemovesPromotionAndUpdatesProduct()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productRepoMock.Setup(r => r.GetById(productId)).Returns(_existingProduct);
        _promotionRepoMock.Setup(r => r.GetById(promotionId)).Returns(_existingPromotion);

        _service.AddPromotion(productId, promotionId);
        _service.RemovePromotion(productId, promotionId);

        _productRepoMock.Verify(r => r.Update(_existingProduct), Times.AtLeast(2));
        Assert.AreEqual(0, _existingProduct.Promotions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void AddPromotion_WhenProductNotFound_ThrowsResourceNotFoundException()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productRepoMock.Setup(r => r.GetById(productId)).Returns(null as Product);

        _service.AddPromotion(productId, promotionId);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void RemovePromotion_WhenProductNotFound_ThrowsResourceNotFoundException()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productRepoMock.Setup(r => r.GetById(productId)).Returns(null as Product);

        _service.RemovePromotion(productId, promotionId);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void AddPromotion_WhenPromotionNotFound_ThrowsResourceNotFoundException()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productRepoMock.Setup(r => r.GetById(productId)).Returns(_existingProduct);
        _promotionRepoMock.Setup(r => r.GetById(promotionId)).Returns(null as Promotion);

        _service.AddPromotion(productId, promotionId);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void RemovePromotion_WhenPromotionNotFound_ThrowsResourceNotFoundException()
    {
        var productId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _productRepoMock.Setup(r => r.GetById(productId)).Returns(_existingProduct);
        _promotionRepoMock.Setup(r => r.GetById(promotionId)).Returns(null as Promotion);

        _service.RemovePromotion(productId, promotionId);
    }

    [TestMethod]
    public void GetById_ReturnsDto_WhenProductExists()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);

        var result = _service.GetById(_existingProduct.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingProduct.Id, result.Id);
        Assert.AreEqual("Laptop", result.Name);
        Assert.AreEqual("A great laptop", result.Description);
        CollectionAssert.AreEqual(_existingProduct.ImagesUrls.ToList(), result.ImagesUrls.ToList());
        Assert.AreEqual(999.99m, result.Price);
        Assert.AreEqual(_category.CategoryId, result.CategoryId);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_Throws_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Product?)null);

        _service.GetById(Guid.NewGuid());
    }

    [TestMethod]
    public void GetByName_ReturnsDto_WhenProductExists()
    {
        _productRepoMock.Setup(r => r.GetByName("Laptop")).Returns([_existingProduct]);

        var result = _service.GetByName("Laptop");

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Laptop", result[0].Name);
    }

    [TestMethod]
    public void GetByName_ReturnsEmptyList_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetByName(It.IsAny<string>())).Returns([]);

        var result = _service.GetByName("nonexistent");

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAll_ReturnsAllProducts()
    {
        var product2 = new Product("Mouse", "A great mouse", 29.99m, _category, ["https://example.com/image.jpg"]);
        _productRepoMock.Setup(r => r.GetAll()).Returns([_existingProduct, product2]);

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(p => p.Name == "Laptop"));
        Assert.IsTrue(result.Any(p => p.Name == "Mouse"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyList_WhenNoProductsExist()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);

        var result = _service.GetAll();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByCategory_ReturnsProducts_WhenCategoryHasProducts()
    {
        _productRepoMock.Setup(r => r.GetByCategory(_category.CategoryId))
            .Returns([_existingProduct]);

        var result = _service.GetByCategory(_category.CategoryId);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Laptop", result[0].Name);
    }

    [TestMethod]
    public void GetByCategory_ReturnsEmptyList_WhenNoneMatch()
    {
        _productRepoMock.Setup(r => r.GetByCategory(It.IsAny<Guid>())).Returns([]);

        var result = _service.GetByCategory(Guid.NewGuid());

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Create_ReturnsDto_WhenValid()
    {
        _productRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(false);
        _categoryRepoMock.Setup(r => r.GetById(_category.CategoryId)).Returns(_category);

        var result = _service.Create(_createDto);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(Guid.Empty, result.Id);
        Assert.AreEqual("Laptop", result.Name);
        CollectionAssert.AreEqual(_createDto.ImagesUrls.ToList(), result.ImagesUrls.ToList());
        _productRepoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_Throws_WhenProductNameAlreadyExists()
    {
        _productRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(true);

        _service.Create(_createDto);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Create_Throws_WhenCategoryDoesNotExist()
    {
        _productRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(false);
        _categoryRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((ProductCategory?)null);

        _service.Create(_createDto);
    }

    [TestMethod]
    public void Update_UpdatesProduct_WhenValid()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);
        _categoryRepoMock.Setup(r => r.GetById(_category.CategoryId)).Returns(_category);

        _service.Update(_existingProduct.Id, _createDto);

        _productRepoMock.Verify(r => r.Update(_existingProduct), Times.Once);
        CollectionAssert.AreEqual(_createDto.ImagesUrls.ToList(), _existingProduct.ImagesUrls.ToList());
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_Throws_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Product?)null);

        _service.Update(Guid.NewGuid(), _createDto);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Update_Throws_WhenProductNameAlreadyExistsOnAnotherProduct()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);
        _productRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(true);

        _service.Update(_existingProduct.Id, _createDto);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_Throws_WhenCategoryDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);
        _productRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(false);
        _categoryRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((ProductCategory?)null);

        _service.Update(_existingProduct.Id, _createDto);
    }

    [TestMethod]
    public void Delete_CallsRepository_WhenProductExists()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);

        _service.Delete(_existingProduct.Id);

        _productRepoMock.Verify(r => r.Delete(_existingProduct.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Delete_Throws_WhenProductIsInOrder()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);
        _productRepoMock.Setup(r => r.IsInOrder(_existingProduct.Id)).Returns(true);

        _service.Delete(_existingProduct.Id);
    }

    [TestMethod]
    public void Delete_DoesNotDeleteOrAudit_WhenProductIsInOrder()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);
        _productRepoMock.Setup(r => r.IsInOrder(_existingProduct.Id)).Returns(true);

        Assert.ThrowsException<DuplicateResourceException>(() => _service.Delete(_existingProduct.Id));

        _productRepoMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        _auditMock.Verify(a => a.Record(It.IsAny<AuditAction>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_Throws_WhenProductDoesNotExist()
    {
        _productRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Product?)null);

        _service.Delete(Guid.NewGuid());
    }

    [TestMethod]
    public void Create_RecordsAuditEntry_WhenValid()
    {
        _productRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(false);
        _categoryRepoMock.Setup(r => r.GetById(_category.CategoryId)).Returns(_category);

        _service.Create(_createDto);

        _auditMock.Verify(a => a.Record(AuditAction.Created, AuditedEntityNames.Product, It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void Update_RecordsAuditEntry_WhenValid()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);
        _categoryRepoMock.Setup(r => r.GetById(_category.CategoryId)).Returns(_category);

        _service.Update(_existingProduct.Id, _createDto);

        _auditMock.Verify(a => a.Record(AuditAction.Updated, AuditedEntityNames.Product, _existingProduct.Id, It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void Delete_RecordsAuditEntry_WhenProductExists()
    {
        _productRepoMock.Setup(r => r.GetById(_existingProduct.Id)).Returns(_existingProduct);

        _service.Delete(_existingProduct.Id);

        _auditMock.Verify(a => a.Record(AuditAction.Deleted, AuditedEntityNames.Product, _existingProduct.Id, It.IsAny<string>()), Times.Once);
    }
}

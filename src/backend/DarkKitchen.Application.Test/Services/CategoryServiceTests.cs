using System.Linq.Expressions;
using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public class CategoryServiceTests
{
    private Mock<ICategoryRepository> _categoryRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private ICategoryService _service = null!;

    private ProductCategory _existingCategory = null!;
    private CategoryCreateDto _createDto = null!;

    [TestInitialize]
    public void SetUp()
    {
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _service = new CategoryService(_categoryRepoMock.Object, _productRepoMock.Object);

        _existingCategory = new ProductCategory("Pastas", "Italian pasta dishes");
        _createDto = new CategoryCreateDto
        {
            Name = "Pastas",
            Description = "Italian pasta dishes"
        };
    }

    [TestMethod]
    public void GetById_ReturnsDto_WhenCategoryExists()
    {
        _categoryRepoMock.Setup(r => r.GetById(_existingCategory.CategoryId)).Returns(_existingCategory);

        var result = _service.GetById(_existingCategory.CategoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingCategory.CategoryId, result.Id);
        Assert.AreEqual("Pastas", result.Name);
        Assert.AreEqual("Italian pasta dishes", result.Description);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_Throws_WhenCategoryDoesNotExist()
    {
        _categoryRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((ProductCategory?)null);

        _service.GetById(Guid.NewGuid());
    }

    [TestMethod]
    public void GetAll_ReturnsAllCategories()
    {
        var second = new ProductCategory("Fritos", "Productos fritos y apanados");
        _categoryRepoMock.Setup(r => r.GetAll()).Returns([_existingCategory, second]);

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(c => c.Name == "Pastas"));
        Assert.IsTrue(result.Any(c => c.Name == "Fritos"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyList_WhenNoCategoriesExist()
    {
        _categoryRepoMock.Setup(r => r.GetAll()).Returns([]);

        var result = _service.GetAll();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Create_ReturnsDto_WhenValid()
    {
        _categoryRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<ProductCategory, bool>>>())).Returns(false);

        var result = _service.Create(_createDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Pastas", result.Name);
        Assert.AreEqual("Italian pasta dishes", result.Description);
        _categoryRepoMock.Verify(r => r.Add(It.IsAny<ProductCategory>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_Throws_WhenNameAlreadyExists()
    {
        _categoryRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<ProductCategory, bool>>>())).Returns(true);

        _service.Create(_createDto);
    }

    [TestMethod]
    public void Update_UpdatesCategory_WhenValid()
    {
        _categoryRepoMock.Setup(r => r.GetById(_existingCategory.CategoryId)).Returns(_existingCategory);
        _categoryRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<ProductCategory, bool>>>())).Returns(false);

        var result = _service.Update(_existingCategory.CategoryId, _createDto);

        _categoryRepoMock.Verify(r => r.Update(_existingCategory), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual(_existingCategory.CategoryId, result.Id);
        Assert.AreEqual(_createDto.Name, result.Name);
        Assert.AreEqual(_createDto.Description, result.Description);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_Throws_WhenCategoryDoesNotExist()
    {
        _categoryRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((ProductCategory?)null);

        _service.Update(Guid.NewGuid(), _createDto);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Update_Throws_WhenNameAlreadyExistsOnAnotherCategory()
    {
        _categoryRepoMock.Setup(r => r.GetById(_existingCategory.CategoryId)).Returns(_existingCategory);
        _categoryRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<ProductCategory, bool>>>())).Returns(true);

        _service.Update(_existingCategory.CategoryId, _createDto);
    }

    [TestMethod]
    public void Delete_CallsRepository_WhenNoProductsAssociated()
    {
        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _service.Delete(_existingCategory.CategoryId);

        _categoryRepoMock.Verify(r => r.Delete(_existingCategory.CategoryId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Delete_Throws_WhenCategoryHasAssociatedProducts()
    {
        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(true);

        _service.Delete(_existingCategory.CategoryId);
    }
}

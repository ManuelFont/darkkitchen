using System.Linq.Expressions;
using DarkKitchen.Application.Services.DeliveryTypes;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class DeliveryTypeServiceTests
{
    private Mock<IDeliveryTypeRepository> _repositoryMock = null!;
    private IDeliveryTypeService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _repositoryMock = new Mock<IDeliveryTypeRepository>();
        _service = new DeliveryTypeService(_repositoryMock.Object);
    }

    [TestMethod]
    public void Create_WithValidData_ReturnsCreatedDeliveryType()
    {
        var dto = new CreateDeliveryTypeDto { Name = "Envio express", Cost = 250m };

        var result = _service.Create(dto);

        Assert.AreEqual("Envio express", result.Name);
        Assert.AreEqual(250m, result.Cost);
        _repositoryMock.Verify(r => r.Add(It.IsAny<DeliveryType>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_WithDuplicateName_ThrowsDuplicateResourceException()
    {
        _repositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns(true);

        _service.Create(new CreateDeliveryTypeDto { Name = "Envio express", Cost = 250m });
    }

    [TestMethod]
    public void Update_WithValidData_UpdatesDeliveryType()
    {
        var deliveryType = new DeliveryType("Envio express", 250m);
        var dto = new CreateDeliveryTypeDto { Name = "Envio en el dia", Cost = 200m };

        _repositoryMock.Setup(r => r.GetById(deliveryType.Id)).Returns(deliveryType);

        _service.Update(deliveryType.Id, dto);

        Assert.AreEqual("Envio en el dia", deliveryType.Name);
        Assert.AreEqual(200m, deliveryType.Cost);
        _repositoryMock.Verify(r => r.Update(deliveryType), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WithMissingDeliveryType_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((DeliveryType?)null);

        _service.Update(id, new CreateDeliveryTypeDto { Name = "Envio express", Cost = 250m });
    }
}

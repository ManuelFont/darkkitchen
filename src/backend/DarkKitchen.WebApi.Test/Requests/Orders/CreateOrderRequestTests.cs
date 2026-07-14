using DarkKitchen.WebApi.Requests.Orders;

namespace DarkKitchen.WebApi.Test.Requests.Orders;

[TestClass]
public sealed class CreateOrderRequestTests
{
    [TestMethod]
    public void ToDto_WithValidRequest_MapsClientIdAndOrderData()
    {
        var clientId = Guid.NewGuid();
        var deliveryTypeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateOrderRequest
        {
            DeliveryTypeId = deliveryTypeId,
            Address = new AddressRequest
            {
                Street = "Main",
                DoorNumber = 123,
                Apartment = "A"
            },
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 2
                }

            ]
        };

        var dto = request.ToDto(clientId);

        Assert.AreEqual(clientId, dto.ClientId);
        Assert.AreEqual(deliveryTypeId, dto.DeliveryTypeId);
        Assert.AreEqual("Main", dto.Street);
        Assert.AreEqual(123, dto.DoorNumber);
        Assert.AreEqual("A", dto.Apartment);
        Assert.AreEqual(1, dto.Items.Count);
        Assert.AreEqual(productId, dto.Items[0].ProductId);
        Assert.AreEqual(2, dto.Items[0].Quantity);
    }
}

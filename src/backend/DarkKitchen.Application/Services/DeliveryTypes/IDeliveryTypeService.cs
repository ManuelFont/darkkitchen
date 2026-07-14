namespace DarkKitchen.Application.Services.DeliveryTypes;

public interface IDeliveryTypeService
{
    IReadOnlyList<DeliveryTypeReadDto> GetAll();
    DeliveryTypeReadDto GetById(Guid id);
    DeliveryTypeReadDto Create(CreateDeliveryTypeDto dto);
    void Update(Guid id, CreateDeliveryTypeDto dto);
}

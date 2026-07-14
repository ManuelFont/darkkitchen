using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.DeliveryTypes;

public sealed class DeliveryTypeService(IDeliveryTypeRepository deliveryTypeRepository) : IDeliveryTypeService
{
    private readonly IDeliveryTypeRepository _deliveryTypeRepository = deliveryTypeRepository;

    public IReadOnlyList<DeliveryTypeReadDto> GetAll()
    {
        return _deliveryTypeRepository.GetAll()
            .Select(ToDto)
            .ToList();
    }

    public DeliveryTypeReadDto GetById(Guid id)
    {
        var deliveryType = GetDeliveryTypeOrThrow(id);
        return ToDto(deliveryType);
    }

    public DeliveryTypeReadDto Create(CreateDeliveryTypeDto dto)
    {
        if(_deliveryTypeRepository.Exists(d => d.Name == dto.Name))
        {
            throw new DuplicateResourceException("DeliveryType", "name", dto.Name);
        }

        var deliveryType = new DeliveryType(dto.Name, dto.Cost);
        _deliveryTypeRepository.Add(deliveryType);
        return ToDto(deliveryType);
    }

    public void Update(Guid id, CreateDeliveryTypeDto dto)
    {
        var deliveryType = GetDeliveryTypeOrThrow(id);

        if(_deliveryTypeRepository.Exists(d => d.Name == dto.Name && d.Id != id))
        {
            throw new DuplicateResourceException("DeliveryType", "name", dto.Name);
        }

        deliveryType.Update(dto.Name, dto.Cost);
        _deliveryTypeRepository.Update(deliveryType);
    }

    private DeliveryType GetDeliveryTypeOrThrow(Guid id) =>
        _deliveryTypeRepository.GetById(id)
            ?? throw new ResourceNotFoundException("DeliveryType", id);

    private static DeliveryTypeReadDto ToDto(DeliveryType deliveryType) => new()
    {
        Id = deliveryType.Id,
        Name = deliveryType.Name,
        Cost = deliveryType.Cost
    };
}

using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.Promotions;

public class PromotionService(IPromotionRepository promotionRepo, IAuditService audit) : IPromotionService
{
    private readonly IPromotionRepository _promotionRepo = promotionRepo;
    private readonly IAuditService _audit = audit;

    public PromotionReadDto GetById(Guid id)
    {
        var promotion = _promotionRepo.GetById(id);
        if(promotion == null)
        {
            throw new ResourceNotFoundException("Promotion", id);
        }

        return PromotionToDto(promotion);
    }

    public IReadOnlyList<PromotionReadDto> GetAll()
    {
        return _promotionRepo.GetAll()
            .Select(PromotionToDto)
            .ToList();
    }

    public IReadOnlyList<PromotionReadDto> GetByProduct(Guid productId)
    {
        return _promotionRepo.GetByProduct(productId)
            .Select(PromotionToDto)
            .ToList();
    }

    public IReadOnlyList<PromotionReadDto> GetByCategory(Guid categoryId)
    {
        return _promotionRepo.GetByCategory(categoryId)
            .Select(PromotionToDto)
            .ToList();
    }

    public PromotionReadDto Create(PromotionCreateDto dto)
    {
        if(_promotionRepo.Exists(p => p.Name == dto.Name))
        {
            throw new DuplicateResourceException("Promotion", "name", dto.Name);
        }

        var product = DtoToPromotion(dto);
        _promotionRepo.Add(product);
        _audit.Record(AuditAction.Created, AuditedEntityNames.Promotion, product.Id, $"Promotion created: {product.Name}");
        return PromotionToDto(product);
    }

    public PromotionReadDto Update(Guid id, PromotionCreateDto dto)
    {
        var promotion = _promotionRepo.GetById(id);
        if(promotion == null)
        {
            throw new ResourceNotFoundException("Promotion", id);
        }

        promotion.Update(DtoToPromotion(dto));
        _promotionRepo.Update(promotion);
        _audit.Record(AuditAction.Updated, AuditedEntityNames.Promotion, promotion.Id, $"Promotion updated: {promotion.Name}");
        return PromotionToDto(promotion);
    }

    public void Delete(Guid id)
    {
        var promotion = _promotionRepo.GetById(id);
        if(promotion == null)
        {
            throw new ResourceNotFoundException("Promotion", id);
        }

        _promotionRepo.Delete(id);
        _audit.Record(AuditAction.Deleted, AuditedEntityNames.Promotion, id, $"Promotion deleted: {promotion.Name}");
    }

    private Promotion DtoToPromotion(PromotionCreateDto dto)
    {
        return new Promotion(dto.Name, dto.DiscountPercentage, dto.StartDate, dto.EndDate);
    }

    private PromotionReadDto PromotionToDto(Promotion promotion)
    {
        var dto = new PromotionReadDto()
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate
        };

        return dto;
    }
}

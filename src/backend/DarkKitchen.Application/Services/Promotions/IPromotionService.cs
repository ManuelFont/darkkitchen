namespace DarkKitchen.Application.Services.Promotions;

public interface IPromotionService
{
    PromotionReadDto GetById(Guid id);
    IReadOnlyList<PromotionReadDto> GetAll();
    IReadOnlyList<PromotionReadDto> GetByProduct(Guid productId);
    IReadOnlyList<PromotionReadDto> GetByCategory(Guid categoryId);
    PromotionReadDto Create(PromotionCreateDto dto);
    PromotionReadDto Update(Guid id, PromotionCreateDto dto);
    void Delete(Guid id);
}

namespace DarkKitchen.Application.Services.Products;

public interface IProductService
{
    ProductReadDto GetById(Guid id);
    IReadOnlyList<ProductReadDto> GetByName(string name);
    IReadOnlyList<ProductReadDto> GetAll();
    IReadOnlyList<ProductReadDto> GetByCategory(Guid categoryId);
    ProductReadDto Create(ProductCreateDto dto);
    void AddPromotion(Guid productId, Guid promotionId);
    void RemovePromotion(Guid productId, Guid promotionId);
    void Update(Guid id, ProductCreateDto dto);
    void Delete(Guid id);
}

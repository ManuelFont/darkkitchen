namespace DarkKitchen.Application.Services.Categories;

public interface ICategoryService
{
    CategoryReadDto GetById(Guid id);
    IReadOnlyList<CategoryReadDto> GetAll();
    CategoryReadDto Create(CategoryCreateDto dto);
    CategoryReadDto Update(Guid id, CategoryCreateDto dto);
    void Delete(Guid id);
}

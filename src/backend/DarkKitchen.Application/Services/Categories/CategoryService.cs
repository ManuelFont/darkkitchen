using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepo, IProductRepository productRepo) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo = categoryRepo;
    private readonly IProductRepository _productRepo = productRepo;

    public CategoryReadDto GetById(Guid id)
    {
        var category = _categoryRepo.GetById(id);
        if(category == null)
        {
            throw new ResourceNotFoundException("Category", id);
        }

        return ToDto(category);
    }

    public IReadOnlyList<CategoryReadDto> GetAll()
    {
        return _categoryRepo.GetAll().Select(ToDto).ToList();
    }

    public CategoryReadDto Create(CategoryCreateDto dto)
    {
        if(_categoryRepo.Exists(c => c.Name == dto.Name))
        {
            throw new DuplicateResourceException("Category", "name", dto.Name);
        }

        var category = new ProductCategory(dto.Name, dto.Description);
        _categoryRepo.Add(category);
        return ToDto(category);
    }

    public CategoryReadDto Update(Guid id, CategoryCreateDto dto)
    {
        var category = _categoryRepo.GetById(id);
        if(category == null)
        {
            throw new ResourceNotFoundException("Category", id);
        }

        if(_categoryRepo.Exists(c => c.Name == dto.Name && c.CategoryId != id))
        {
            throw new DuplicateResourceException("Category", "name", dto.Name);
        }

        category.Update(dto.Name, dto.Description);
        _categoryRepo.Update(category);
        return ToDto(category);
    }

    public void Delete(Guid id)
    {
        if(_productRepo.Exists(p => p.Category.CategoryId == id))
        {
            throw new InvalidArgumentException("Cannot delete a category that has associated products.");
        }

        _categoryRepo.Delete(id);
    }

    private static CategoryReadDto ToDto(ProductCategory category) =>
        new()
        {
            Id = category.CategoryId,
            Name = category.Name,
            Description = category.Description
        };
}

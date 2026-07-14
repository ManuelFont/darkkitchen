using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Application.Services.Promotions;
using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.Products;

public class ProductService(IProductRepository productRepo, ICategoryRepository categoryRepo, IPromotionRepository promoRepo, IAuditService audit) : IProductService
{
    private readonly IProductRepository _productRepo = productRepo;
    private readonly ICategoryRepository _categoryRepo = categoryRepo;
    private readonly IPromotionRepository _promotionRepo = promoRepo;
    private readonly IAuditService _audit = audit;
    public ProductReadDto GetById(Guid id)
    {
        var product = _productRepo.GetById(id);
        if(product == null)
        {
            throw new ResourceNotFoundException("Product", id);
        }

        return ProductToDto(product);
    }

    public IReadOnlyList<ProductReadDto> GetByName(string name)
    {
        return _productRepo.GetByName(name)
            .Select(ProductToDto)
            .ToList();
    }

    public IReadOnlyList<ProductReadDto> GetAll()
    {
        return _productRepo.GetAll()
            .Select(ProductToDto)
            .ToList();
    }

    public IReadOnlyList<ProductReadDto> GetByCategory(Guid categoryId)
    {
        return _productRepo.GetByCategory(categoryId)
            .Select(ProductToDto)
            .ToList();
    }

    public ProductReadDto Create(ProductCreateDto dto)
    {
        if(_productRepo.Exists(p => p.Name == dto.Name))
        {
            throw new DuplicateResourceException("Product", "name", dto.Name);
        }

        var product = DtoToProduct(dto);
        _productRepo.Add(product);
        _audit.Record(AuditAction.Created, AuditedEntityNames.Product, product.Id, $"Product created: {product.Name}");
        return ProductToDto(product);
    }

    public void Update(Guid id, ProductCreateDto dto)
    {
        var product = _productRepo.GetById(id);
        if(product == null)
        {
            throw new ResourceNotFoundException("Product", id);
        }

        if(_productRepo.Exists(p => p.Name == dto.Name && p.Id != id))
        {
            throw new DuplicateResourceException("Product", "Name", dto.Name);
        }

        product.Update(DtoToProduct(dto));
        _productRepo.Update(product);
        _audit.Record(AuditAction.Updated, AuditedEntityNames.Product, product.Id, $"Product updated: {product.Name}");
    }

    public void Delete(Guid id)
    {
        var product = _productRepo.GetById(id);
        if(product == null)
        {
            throw new ResourceNotFoundException("Product", id);
        }

        if(_productRepo.IsInOrder(id))
        {
            throw new DuplicateResourceException("Product", "order", id.ToString());
        }

        _productRepo.Delete(id);
        _audit.Record(AuditAction.Deleted, AuditedEntityNames.Product, id, $"Product deleted: {product.Name}");
    }

    public void AddPromotion(Guid productId, Guid promotionId)
    {
        var product = _productRepo.GetById(productId);
        if(product == null)
        {
            throw new ResourceNotFoundException("Product", productId);
        }

        var promotion = _promotionRepo.GetById(promotionId);
        if(promotion == null)
        {
            throw new ResourceNotFoundException("Promotion", promotionId);
        }

        product.AddPromotion(promotion);
        _productRepo.Update(product);
        _audit.Record(AuditAction.Updated, AuditedEntityNames.Product, product.Id, $"Promotion added to product: {product.Name}");
    }

    public void RemovePromotion(Guid productId, Guid promotionId)
    {
        var product = _productRepo.GetById(productId);
        if(product == null)
        {
            throw new ResourceNotFoundException("Product", productId);
        }

        var promotion = _promotionRepo.GetById(promotionId);
        if(promotion == null)
        {
            throw new ResourceNotFoundException("Promotion", promotionId);
        }

        product.RemovePromotion(promotion);
        _productRepo.Update(product);
        _audit.Record(AuditAction.Updated, AuditedEntityNames.Product, product.Id, $"Promotion removed from product: {product.Name}");
    }

    private Product DtoToProduct(ProductCreateDto dto)
    {
        var category = _categoryRepo.GetById(dto.CategoryId);
        if(category == null)
        {
            throw new ResourceNotFoundException("Category not found", dto.CategoryId);
        }

        return new Product(dto.Name, dto.Description, dto.Price, category, dto.ImagesUrls);
    }

    private ProductReadDto ProductToDto(Product product)
    {
        var activePromotion = product.ActivePromotion();

        var dto = new ProductReadDto()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImagesUrls = product.ImagesUrls,
            Price = product.Price,
            CategoryId = product.Category.CategoryId,
            Category = CategoryToDto(product.Category),
            ActivePromotion = activePromotion is not null ? PromotionToDto(activePromotion) : null,
            Promotions = product.Promotions.Select(PromotionToDto).ToList()
        };

        return dto;
    }

    private static CategoryReadDto CategoryToDto(ProductCategory category)
    {
        return new CategoryReadDto
        {
            Id = category.CategoryId,
            Name = category.Name,
            Description = category.Description
        };
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

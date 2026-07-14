using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyList<string> ImagesUrls => _images
        .OrderBy(image => image.Position)
        .Select(image => image.Url)
        .ToList()
        .AsReadOnly();
    public IReadOnlyList<Promotion> Promotions => _promotions.AsReadOnly();
    public decimal Price { get; private set; }
    public ProductCategory Category { get; private set; }
    private readonly List<Image> _images = [];
    private readonly List<Promotion> _promotions = [];

    public Product(
        string name,
        string description,
        decimal price,
        ProductCategory category,
        IReadOnlyList<string> imagesUrls)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateImagesUrls(imagesUrls);
        if(category == null)
        {
            throw new InvalidArgumentException("Category cannot be null.");
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Description = description;
        Price = price;
        Category = category;
        ReplaceImages(imagesUrls);
    }

    public void Update(
        string name,
        string description,
        decimal price,
        ProductCategory category,
        IReadOnlyList<string> imagesUrls)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateImagesUrls(imagesUrls);
        if(category == null)
        {
            throw new InvalidArgumentException("Category cannot be null.");
        }

        Name = name.Trim();
        Description = description;
        Price = price;
        Category = category;
        ReplaceImages(imagesUrls);
    }

    public void Update(Product product)
    {
        ValidateName(product.Name);
        ValidateDescription(product.Description);
        ValidatePrice(product.Price);
        ValidateImagesUrls(product.ImagesUrls);
        ArgumentNullException.ThrowIfNull(product.Category);

        Name = product.Name.Trim();
        Description = product.Description;
        Price = product.Price;
        Category = product.Category;
        ReplaceImages(product.ImagesUrls);
    }

    public void AddPromotion(Promotion promotion)
    {
        ValidateAddPromotion(promotion);
        _promotions.Add(promotion);
    }

    public void RemovePromotion(Promotion promotion)
    {
        ValidateRemovePromotion(promotion);
        _promotions.Remove(promotion);
    }

    private void ValidateAddPromotion(Promotion promotion)
    {
        if(promotion == null)
        {
            throw new InvalidArgumentException("Promotion cant be null");
        }

        if(PromotionAlreadyAdded(Promotions, promotion))
        {
            throw new InvalidArgumentException("This Promotion is already applied");
        }
    }

    private void ValidateRemovePromotion(Promotion promotion)
    {
        if(promotion == null)
        {
            throw new InvalidArgumentException("Promotion cant be null");
        }

        if(!PromotionAlreadyAdded(Promotions, promotion))
        {
            throw new InvalidArgumentException("That Promotion is not applied to this Product");
        }
    }

    private static bool PromotionAlreadyAdded(IEnumerable<Promotion> promotions, Promotion promotion)
    {
        return promotions.Any(p => p.Id == promotion.Id || p.Name == promotion.Name);
    }

    public Promotion? ActivePromotion()
    {
        Promotion? activePromotion = null;
        foreach(var promotion in _promotions)
        {
            if(promotion.IsActive(DateTime.Today))
            {
                if(activePromotion == null || promotion.DiscountPercentage > activePromotion.DiscountPercentage)
                {
                    activePromotion = promotion;
                }
            }
        }

        return activePromotion;
    }

    public decimal DiscountedPrice()
    {
        var discountedPrice = Price;
        var promotion = ActivePromotion();

        if(promotion != null)
        {
            discountedPrice *= 1 - promotion.DiscountPercentage;
        }

        return discountedPrice;
    }

    private static void ValidateName(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("Name cannot be null or whitespace.");
        }

        if(value.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Name cannot contain numbers");
        }

        if(value.Any(char.IsSymbol))
        {
            throw new InvalidArgumentException("Name name cannot contain symbols");
        }

        if(value.Any(char.IsPunctuation))
        {
            throw new InvalidArgumentException("Name cannot contain punctuation");
        }
    }

    private static void ValidateDescription(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("Description cannot be null or whitespace.");
        }

        if(value.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Description cannot contain numbers");
        }

        if(value.Any(char.IsSymbol))
        {
            throw new InvalidArgumentException("Description name cannot contain symbols");
        }

        if(value.Any(char.IsPunctuation))
        {
            throw new InvalidArgumentException("Description cannot contain punctuation");
        }
    }

    private static void ValidatePrice(decimal price)
    {
        if(price <= 0)
        {
            throw new InvalidArgumentException("Price must be greater than zero.");
        }
    }

    private static void ValidateImagesUrls(IReadOnlyList<string> imagesUrls)
    {
        if(imagesUrls == null || imagesUrls.Count is < 1 or > 3)
        {
            throw new InvalidArgumentException("A product must have between one and three image URLs.");
        }

        if(imagesUrls.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidArgumentException("Image URLs cannot be null or whitespace.");
        }
    }

    private void ReplaceImages(IReadOnlyList<string> imagesUrls)
    {
        _images.Clear();
        for(var position = 0; position < imagesUrls.Count; position++)
        {
            _images.Add(new Image(imagesUrls[position], position));
        }
    }

    private Product()
    {
        Name = null!;
        Description = null!;
        Category = null!;
    }
}

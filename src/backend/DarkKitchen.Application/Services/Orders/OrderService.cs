using DarkKitchen.Application.Services.Orders.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Application.Services.Orders;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IDeliveryTypeRepository deliveryTypeRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IDeliveryTypeRepository _deliveryTypeRepository = deliveryTypeRepository;

    public CreateOrderResultDto Create(CreateOrderDto dto)
    {
        Validate(dto);

        var order = BuildOrder(dto);

        _orderRepository.Add(order);

        return new CreateOrderResultDto
        {
            OrderId = order.Id,
            ClientId = order.ClientId,
            Subtotal = order.Subtotal,
            DeliveryCost = order.DeliveryCost,
            Total = order.Total
        };
    }

    public OrderDetailDto GetById(Guid id)
    {
        var order = GetOrderOrThrow(id);
        return ToOrderDetailDto(order);
    }

    public IReadOnlyList<ClientOrderSummaryDto> GetClientOrders(Guid clientId, GetClientOrdersDto dto)
    {
        if(clientId == Guid.Empty)
        {
            throw new InvalidArgumentException("Client id is required");
        }

        return _orderRepository
            .GetByClient(clientId, dto.DateFrom, dto.DateTo, dto.Status)
            .Select(ToClientOrderSummaryDto)
            .ToList();
    }

    public IReadOnlyList<OrderListItemDto> GetOrders(GetOrdersDto dto)
    {
        if(dto.DateFrom.HasValue && dto.DateTo.HasValue && dto.DateFrom > dto.DateTo)
        {
            throw new InvalidArgumentException("DateFrom must be before or equal to DateTo");
        }

        return _orderRepository
            .GetByFilters(dto.DateFrom, dto.DateTo, dto.Street, dto.Status)
            .Select(ToOrderListItemDto)
            .ToList();
    }

    public OrderStatusChangeDto MarkOrderAsReady(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        order.MarkAsReady();
        _orderRepository.Update(order);
        return ToOrderStatusChangeDto(order);
    }

    public OrderStatusChangeDto MarkOrderAsDelayed(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        order.MarkAsDelayed();
        _orderRepository.Update(order);
        return ToOrderStatusChangeDto(order);
    }

    public OrderStatusChangeDto CancelOrder(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        order.MarkAsCancelled();
        _orderRepository.Update(order);
        return ToOrderStatusChangeDto(order);
    }

    public OrderStatusChangeDto MarkOrderAsOnTheWay(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        order.MarkAsOnTheWay();
        _orderRepository.Update(order);
        return ToOrderStatusChangeDto(order);
    }

    public OrderStatusChangeDto DeliverOrder(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        order.MarkAsDelivered();
        _orderRepository.Update(order);
        return ToOrderStatusChangeDto(order);
    }

    public OrderStatusChangeDto MarkOrderAsNotDelivered(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        order.MarkAsNotDelivered();
        _orderRepository.Update(order);
        return ToOrderStatusChangeDto(order);
    }

    private Order GetOrderOrThrow(Guid orderId) =>
        _orderRepository.GetById(orderId)
            ?? throw new ResourceNotFoundException("Order", orderId);

    private static OrderStatusChangeDto ToOrderStatusChangeDto(Order order) => new()
    {
        OrderId = order.Id,
        Status = order.Status,
        StatusChangedAt = order.StatusChangedAt
    };

    private static OrderDetailDto ToOrderDetailDto(Order order) => new()
    {
        OrderId = order.Id,
        ClientFirstName = order.Client.FirstName,
        ClientLastName = order.Client.LastName,
        ClientEmail = order.Client.Email,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        Subtotal = order.Subtotal,
        DeliveryCost = order.DeliveryCost,
        Total = order.Total,
        DeliveryTypeId = order.DeliveryTypeId,
        DeliveryTypeName = order.DeliveryType.Name,
        Street = order.Address.Street,
        DoorNumber = order.Address.DoorNumber,
        Apartment = order.Address.Apartment,
        Items = order.Items
            .Select(i => new OrderDetailItemDto
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price,
                Subtotal = i.Subtotal
            })
            .ToList()
    };

    private static OrderListItemDto ToOrderListItemDto(Order order) => new()
    {
        OrderId = order.Id,
        ClientFirstName = order.Client.FirstName,
        ClientLastName = order.Client.LastName,
        ClientEmail = order.Client.Email,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        Total = order.Total,
        Items = order.Items
            .Select(i => new OrderListItemProductDto
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                Price = i.Product.Price
            })
            .ToList()
    };

    private static ClientOrderSummaryDto ToClientOrderSummaryDto(Order order) => new()
    {
        OrderId = order.Id,
        ClientFirstName = order.Client.FirstName,
        ClientLastName = order.Client.LastName,
        ClientEmail = order.Client.Email,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        Total = order.Total,
        ProductCount = order.Items.Sum(i => i.Quantity)
    };

    private Order BuildOrder(CreateOrderDto dto)
    {
        try
        {
            var address = new Address(dto.Street, dto.DoorNumber, dto.Apartment);
            var deliveryType = _deliveryTypeRepository.GetById(dto.DeliveryTypeId)
                ?? throw new ResourceNotFoundException("DeliveryType", dto.DeliveryTypeId);
            var items = dto.Items
                .Select(ToOrderItem)
                .ToList();

            return new Order(dto.ClientId, deliveryType, address, items);
        }
        catch(ArgumentException ex)
        {
            throw new InvalidArgumentException(ex.Message);
        }
    }

    private static void Validate(CreateOrderDto dto)
    {
        if(dto.ClientId == Guid.Empty)
        {
            throw new InvalidArgumentException("Client id is required");
        }

        if(dto.Items == null || dto.Items.Count == 0)
        {
            throw new InvalidArgumentException("Order must contain at least one item");
        }

        if(dto.DeliveryTypeId == Guid.Empty)
        {
            throw new InvalidArgumentException("Delivery type id is required");
        }
    }

    private OrderItem ToOrderItem(CreateOrderItemDto item)
    {
        if(item.Quantity <= 0)
        {
            throw new InvalidArgumentException("Quantity must be greater than zero");
        }

        var product = _productRepository.GetById(item.ProductId)
            ?? throw new ResourceNotFoundException("Product", item.ProductId);

        return new OrderItem(product, item.Quantity);
    }
}

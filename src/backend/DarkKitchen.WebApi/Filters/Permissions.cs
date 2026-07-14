namespace DarkKitchen.WebApi.Filters;

internal enum PermissionNames
{
    /// <summary>Permission to get products.</summary>
    CanGetProduct,

    /// <summary>Permission to create products.</summary>
    CanCreateProduct,

    /// <summary>Permission to update products.</summary>
    CanUpdateProduct,

    /// <summary>Permission to delete products.</summary>
    CanDeleteProduct,

    /// <summary>Permission to get categories.</summary>
    CanGetCategory,

    /// <summary>Permission to create categories.</summary>
    CanCreateCategory,

    /// <summary>Permission to update categories.</summary>
    CanUpdateCategory,

    /// <summary>Permission to delete categories.</summary>
    CanDeleteCategory,

    /// <summary>Permission to create orders.</summary>
    CanCreateOrder,

    /// <summary>Permission to get promotions.</summary>
    CanGetPromotion,

    /// <summary>Permission to create promotions.</summary>
    CanCreatePromotion,

    /// <summary>Permission to update promotions.</summary>
    CanUpdatePromotion,

    /// <summary>Permission to delete promotions.</summary>
    CanDeletePromotion,

    /// <summary>Permission for customers to query their own orders.</summary>
    CanGetClientOrders,

    /// <summary>Permission for dispatchers to query orders by filters.</summary>
    CanGetOrders,

    /// <summary>Permission to view order detail.</summary>
    CanGetOrderDetail,

    /// <summary>Permission to search users.</summary>
    CanGetUsers,

    /// <summary>Permission to create users.</summary>
    CanCreateUser,

    /// <summary>Permission to update users.</summary>
    CanUpdateUser,

    /// <summary>Permission to delete users.</summary>
    CanDeleteUser,

    /// <summary>Permission to mark an order as ready.</summary>
    CanMarkOrderReady,

    /// <summary>Permission to cancel an order.</summary>
    CanCancelOrder,

    /// <summary>Permission to mark an order as on the way.</summary>
    CanMarkOrderOnTheWay,

    /// <summary>Permission to mark an order as delivered.</summary>
    CanDeliverOrder,

    /// <summary>Permission to mark an order as not delivered.</summary>
    CanMarkOrderNotDelivered,

    /// <summary>Permission to mark an order as delayed.</summary>
    CanMarkOrderDelayed,

    /// <summary>Permission to query the top sold products report.</summary>
    CanGetTopProducts,

    /// <summary>Permission to query the grouped sales report.</summary>
    CanGetSalesReport,

    /// <summary>Permission to query the audit log.</summary>
    CanGetAuditLog,

    /// <summary>Permission to get delivery types.</summary>
    CanGetDeliveryType,

    /// <summary>Permission to create delivery types.</summary>
    CanCreateDeliveryType,

    /// <summary>Permission to update delivery types.</summary>
    CanUpdateDeliveryType,
}

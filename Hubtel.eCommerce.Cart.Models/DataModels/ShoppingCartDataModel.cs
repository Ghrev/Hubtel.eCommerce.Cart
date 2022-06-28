using System;

namespace Hubtel.eCommerce.Cart.Models.DataModels
{
    public class ShoppingCartDataModel
    {
        public int ShoppingCartId { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Quantity { get; set; }
        public string Owner { get; set; }
        public string ItemName { get; set; }
        public string PhoneNumber { get; set; }
    }
}

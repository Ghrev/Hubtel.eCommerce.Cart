using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hubtel.eCommerce.Cart.Models.ApiModels
{
    public class ShoppingCartApiModel
    {
        [Required, Range(1, int.MaxValue), DefaultValue(1)]
        public int ItemId { get; set; }

        [Required, Range(1, int.MaxValue), DefaultValue(1)]
        public int Quantity { get; set; }
    }
}

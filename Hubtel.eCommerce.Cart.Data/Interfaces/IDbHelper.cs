using System.Collections.Generic;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Hubtel.eCommerce.Cart.Models.DataModels;

namespace Hubtel.eCommerce.Cart.Data.Interfaces
{
    public interface IDbHelper
    {
        Task<int> AddItemToShoppingCart(ShoppingCartApiModel cart, int userId);
        Task<int> CreateUser(string username, string fullname, string phoneNumber, string password);
        Task<List<ShoppingCartDataModel>> GetAllItemsInShoppingCart(int userId);
        Task<List<AllShoppingCartDataModels>> GetAllShoppingCart();
        Task<List<ShoppingItemsDataModel>> GetAllShoppingItems();
        Task<ShoppingCartDataModel> GetItemFromCart(int userId, int itemId);
        Task<UserDataModel> GetUserById(int userId);
        Task<UserDataModel> GetUserByUserName(string username);
        Task<int> RemoveItemFromCart(int userId, int itemId);
        Task<int> UpdateShoppingCartItemQuantity(int itemId, int quantity);
    }
}

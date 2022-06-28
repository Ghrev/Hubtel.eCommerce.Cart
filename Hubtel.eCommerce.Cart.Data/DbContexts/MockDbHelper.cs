using System.Collections.Generic;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Data.Interfaces;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Hubtel.eCommerce.Cart.Models.DataModels;

namespace Hubtel.eCommerce.Cart.Data.DbContexts
{
    public class MockDbHelper : IDbHelper
    {
        private dynamic testUser = new UserDataModel
        {
            UserName = "Test",
            FullName = "Test User",
            UserId = 1000,
            PhoneNumber = "0000000000",
            Password = "8e3a40b54d9a3a0d915dfd50d5a750c408041029056d5b98b9b3b27faba42a66"
        };


        public Task<int> AddItemToShoppingCart(ShoppingCartApiModel cart, int userId)
        {
            if (cart == null) return Task.FromResult(0);

            if (cart.ItemId < 1 || cart.Quantity < 1) return Task.FromResult(0);

            return Task.FromResult(1);
        }

        public Task<int> CreateUser(string username, string fullname, string phoneNumber, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))
                return Task.FromResult(0);

            return Task.FromResult(1);
        }

        public Task<List<ShoppingCartDataModel>> GetAllItemsInShoppingCart(int userId)
        {
            return Task.FromResult(new List<ShoppingCartDataModel>());
        }

        public Task<List<AllShoppingCartDataModels>> GetAllShoppingCart()
        {
            return Task.FromResult(new List<AllShoppingCartDataModels>());
        }

        public Task<List<ShoppingItemsDataModel>> GetAllShoppingItems()
        {
            List<ShoppingItemsDataModel> items = new List<ShoppingItemsDataModel>
            {
                new ShoppingItemsDataModel { ItemId = 1, ItemName = "iPhone 13 Mini", UnitPrice = 5325 },
                new ShoppingItemsDataModel { ItemId = 2, ItemName = "Samsung Galaxy Note 20 Ultra", UnitPrice = 7325 },
                new ShoppingItemsDataModel { ItemId = 3, ItemName = "Apple Mac Book M1 Ultra", UnitPrice = 17325 },
                new ShoppingItemsDataModel { ItemId = 4, ItemName = "Sony PlayStation 5", UnitPrice = 8425 },
                new ShoppingItemsDataModel { ItemId = 5, ItemName = "Microsoft Xbox Series One X", UnitPrice = 7325 },
                new ShoppingItemsDataModel { ItemId = 6, ItemName = "Microsoft Xbox Series One S", UnitPrice = 4325 },
                new ShoppingItemsDataModel { ItemId = 7, ItemName = "2Tb Sata HDD (SanDisk)", UnitPrice = 725 },
                new ShoppingItemsDataModel { ItemId = 8, ItemName = "Studio Quality Microphone", UnitPrice = 325 },
                new ShoppingItemsDataModel { ItemId = 9, ItemName = "HDMI To Mini HDMI Converter", UnitPrice = 180 },
                new ShoppingItemsDataModel { ItemId = 10, ItemName = "Multi USB Hub to USB C", UnitPrice = 140 },
                new ShoppingItemsDataModel { ItemId = 11, ItemName = "3000mah Power bank ", UnitPrice = 280 }
            };

            return Task.FromResult(items);
        }

        public Task<ShoppingCartDataModel> GetItemFromCart(int userId, int itemId)
        {
            return Task.FromResult(new ShoppingCartDataModel());
        }

        public Task<UserDataModel> GetUserById(int userId)
        {
            return Task.FromResult(new UserDataModel
            {
                UserName = "Test",
                FullName = "Test User",
                UserId = 1000,
                PhoneNumber = "0000000000"
            });
        }

        public Task<UserDataModel> GetUserByUserName(string username)
        {
            if (username == "test") return Task.FromResult(testUser);

            var user = new UserDataModel();
            user = null;
            return Task.FromResult(user);
        }

        public Task<int> RemoveItemFromCart(int userId, int itemId)
        {
            return Task.FromResult(1);
        }

        public Task<int> UpdateShoppingCartItemQuantity(int itemId, int quantity)
        {
            if (quantity < 1 || itemId < 1) return Task.FromResult(0);

            return Task.FromResult(1);
        }
    }
}

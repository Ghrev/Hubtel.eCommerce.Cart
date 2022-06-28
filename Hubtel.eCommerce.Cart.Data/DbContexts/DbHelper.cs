using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Hubtel.eCommerce.Cart.Data.Interfaces;
using Hubtel.eCommerce.Cart.Data.Utility;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Hubtel.eCommerce.Cart.Models.DataModels;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Hubtel.eCommerce.Cart.Data.DbContexts
{
    public class DbHelper : IDbHelper
    {
        private readonly Logger _logger;
        public readonly string DbConnString;
        public readonly string DbSchema;



        public DbHelper(IConfiguration configuration, Logger logger)
        {
            _logger = logger;
            DbConnString = GetConnectionString(configuration);

            DbSchema = GetDbSchema(configuration);
        }

        private string GetConnectionString(IConfiguration configuration)
        {
            try
            {
                string dbCon = configuration.GetConnectionString("HubtelECommerceDB");

                return dbCon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return string.Empty;
            }
        }

        private string GetDbSchema(IConfiguration configuration)
        {
            try
            {
                string dbSchema = configuration.GetSection("AppSettings")["DbSchema"];

                return dbSchema;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return string.Empty;
            }
        }

        private NpgsqlConnection CreateConnection()
        {
            try
            {
                return new NpgsqlConnection(DbConnString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return null;
            }
        }

        private void DisposeConnection(NpgsqlConnection con)
        {
            try
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            catch
            {
                //Ignore exception
            }
        }

        private async Task<IEnumerable> QueryList<T>(string storedProcedureName, object parameters = null)
        {
            var table = new List<T>();
            var dbCon = CreateConnection();
            try
            {
                using (dbCon)
                {
                    if (parameters != null)
                    {
                        table = (await dbCon.QueryAsync<T>($"\"{storedProcedureName}\"", parameters,
                            commandType: CommandType.StoredProcedure)).ToList();
                    }
                    else
                        table = (await dbCon.QueryAsync<T>($"\"{storedProcedureName}\"",
                            commandType: CommandType.StoredProcedure)).ToList();
                }
                DisposeConnection(dbCon);
                return table;
            }
            catch (Exception ex)
            {
                DisposeConnection(dbCon);
                _logger.LogError(ex);
                return table;
            }
        }

        private async Task<T> QuerySingle<T>(string storedProcedureName, object parameters = null)
        {
            dynamic table = 0;
            var dbCon = CreateConnection();
            try
            {
                using (dbCon)
                {
                    if (parameters != null)
                    {
                        table = await dbCon.QueryFirstOrDefaultAsync<T>($"\"{storedProcedureName}\"", parameters,
                           commandType: CommandType.StoredProcedure);
                    }
                    else
                        table = await dbCon.QueryFirstOrDefaultAsync<T>($"\"{storedProcedureName}\"",
                            commandType: CommandType.StoredProcedure);
                }
                DisposeConnection(dbCon);
                return table;
            }
            catch (Exception ex)
            {
                DisposeConnection(dbCon);
                _logger.LogError(ex);
                Type t = typeof(T);
                if (t != typeof(int))
                    return default(T);
                return (T)table;
            }
        }


        public async Task<int> CreateUser(string username, string fullname, string phoneNumber, string password)
        {
            var result = await QuerySingle<int>("CreateUser", new
            {
                _userName = username,
                _fullName = fullname,
                _password = password,
                _msisdn = phoneNumber,
            });
            return result;
        }

        public async Task<UserDataModel> GetUserById(int userId)
        {
            var user = await QuerySingle<UserDataModel>("GetUserById", new { _userId = userId });
            return user;
        }

        public async Task<UserDataModel> GetUserByUserName(string username)
        {
            var user = await QuerySingle<UserDataModel>("GetUserByUsername", new { _username = username });
            return user;
        }

        public async Task<int> AddItemToShoppingCart(ShoppingCartApiModel cart, int userId)
        {
            var result = await QuerySingle<int>("AddItemToShoppingCart", new
            {
                _itemId = cart.ItemId,
                _userId = userId,
                _addedAt = DateTime.Now,
                _updatedAt = DateTime.Now,
                _quantity = cart.Quantity
            });

            return result;
        }

        public async Task<int> UpdateShoppingCartItemQuantity(int itemId, int quantity)
        {
            var result = await QuerySingle<int>("UpdateItemQuantity", new { _itemId = itemId, _quantity = quantity, _updatedAt = DateTime.Now });
            return result;
        }

        public async Task<List<ShoppingCartDataModel>> GetAllItemsInShoppingCart(int userId)
        {
            var item = await QueryList<ShoppingCartDataModel>("ListAllShoppingCartItems",
                new { _userId = userId });
            return (List<ShoppingCartDataModel>)item;
        }

        public async Task<ShoppingCartDataModel> GetItemFromCart(int userId, int itemId)
        {
            var item = await QuerySingle<ShoppingCartDataModel>("GetItemFromCart",
                new { _userId = userId, _itemId = itemId });
            return item ?? new ShoppingCartDataModel();
        }

        public async Task<int> RemoveItemFromCart(int userId, int itemId)
        {
            var response = await QuerySingle<int>("RemoveItemFromCart", new { _userId = userId, _itemId = itemId });
            return response;
        }

        public async Task<List<ShoppingItemsDataModel>> GetAllShoppingItems()
        {
            var items = await QueryList<ShoppingItemsDataModel>("ListAllShoppingItems") as List<ShoppingItemsDataModel>;
            return items;
        }

        public async Task<List<AllShoppingCartDataModels>> GetAllShoppingCart()
        {
            var shoppingCarts = await QueryList<AllShoppingCartDataModels>("GetAllShoppingCarts") as List<AllShoppingCartDataModels>;
            return shoppingCarts;
        }
    }
}

using System;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart;
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Data.DbContexts;
using Hubtel.eCommerce.Cart.Data.Utility;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Hubtel.eCommerce.UnitTests.Controllers
{
    public class ShoppingCartControllerTest : IntegrationTest
    {
        private MockDbHelper _dbHelper;
        private Logger _logger;
        private IConfiguration _configuration;
        private ShoppingCartApiController _cartApiController;
        private WebApplicationFactory<Startup> _webApplicationFactory;

        public ShoppingCartControllerTest() : base(out var webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }




        [SetUp]
        public void SetUp()
        {
            _logger = new Logger();
            _dbHelper = new MockDbHelper();
            _cartApiController = new ShoppingCartApiController(_dbHelper, _logger);
        }

        [Test]
        public void GetAllShoppingCart()
        {
            DateTime? time = null;
            string phoneNumber = "";
            int quantity = 0;
            string itemName = "";
            int pageSize = 10;
            int pageNo = 1;

            var result = _cartApiController.ShowAllShoppingCarts(time, phoneNumber, quantity, itemName, pageSize, pageNo).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task AddItemToCart_ReturnsOkResult()
        {
            var item = new ShoppingCartApiModel
            {
                ItemId = 1,
                Quantity = 2
            };

            var result = _cartApiController.AddItemToCart(item).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual("Successful", result.Value);

        }

        [Test]
        public void AddItemToCart_Error()
        {
            var item = new ShoppingCartApiModel
            {
                ItemId = 0,
                Quantity = 2
            };

            var result = _cartApiController.AddItemToCart(item).Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual("Invalid itemId or item quantity", result.Value);

        }

        [Test]
        public void RemoveItemFromCart_ReturnsOkResult()
        {
            var result = _cartApiController.RemoveItemFromCart(1).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, "Successful");
        }

        [Test]
        public void RemoveItemFromCart_ReturnsBadRequestResult()
        {
            var result = _cartApiController.RemoveItemFromCart(0).Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "Invalid itemId");
        }


        [Test]
        public void GetAllStockItems()
        {
            var result = _cartApiController.ListAllShopItems(20, 1).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [Test]
        public void GetSingleItemFromShoppingCartbyId_ReturnsItem()
        {
            var result = _cartApiController.GetShoppingCartItembyId(1).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.StatusCode, 200);
        }


        [Test]
        public void GetSingleItemFromShoppingCartbyId_ReturnsBadRequest()
        {
            var result = _cartApiController.GetShoppingCartItembyId(0).Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "Invalid itemId");
        }
    }
}

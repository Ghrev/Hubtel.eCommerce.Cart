using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Data.DbContexts;
using Hubtel.eCommerce.Cart.Data.Utility;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Hubtel.eCommerce.UnitTests.Controllers
{
    public class AuthControllerTest
    {
        private AuthenticationApiController _authenticationApiController;
        private MockDbHelper _dbHelper;
        private Logger _logger;
        private TestValues _configuration;

        [SetUp]
        public void SetUp()
        {
            _dbHelper = new MockDbHelper();
            _logger = new Logger();
            _configuration = new TestValues();
            _authenticationApiController = new AuthenticationApiController(_dbHelper, _logger, _configuration.StubConfiguration);
        }


        [Test]
        public void CreateUser_ReturnSuccessful()
        {
            var newUser = new NewUserApiModel { UserName = "roger33", FullName = "test test1", Msisdn = "0000000001", Password = "test" };
            var result = _authenticationApiController.CreateNewUser(newUser).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual("Successful", result.Value);
        }

        [Test]
        public void CreateUser_ReturnsBadrequest()
        {
            var newUser = new NewUserApiModel { UserName = "test", FullName = "test test", Msisdn = "0000000000", Password = "test" };
            var result = _authenticationApiController.CreateNewUser(newUser).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual("User already exists", result.Value);
        }


        [Test]
        public void Login_ReturnsJwtToken()
        {
            var user = new LoginApiModel { Username = "Test", Password = "power233" };

            var result = _authenticationApiController.Login(user).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Login Successful", result.Value.GetType().GetProperty("Message").GetValue(result.Value));
            Assert.AreEqual(200, result.StatusCode);
        }


        [Test]
        public void Login_ReturnsErrorMessage()
        {
            var user = new LoginApiModel { Username = "ssesasd", Password = "power233" };

            var result = _authenticationApiController.Login(user).Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Account does not exist", result.Value);
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}

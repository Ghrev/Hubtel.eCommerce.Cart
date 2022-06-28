using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Data.Interfaces;
using Hubtel.eCommerce.Cart.Data.Utility;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartApiController : ControllerBase
    {
        private readonly IDbHelper _dbHelper;
        private readonly Logger _logger;

        public ShoppingCartApiController(IDbHelper dbHelper, Logger logger)
        {
            _dbHelper = dbHelper;
            _logger = logger;
        }


        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItemToCart(ShoppingCartApiModel item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (item.Quantity < 1 || item.ItemId < 1) return BadRequest("Invalid itemId or item quantity");

                    var userId = GetCurrentUserId();
                    var cartItems = await _dbHelper.GetItemFromCart(userId, item.ItemId);

                    if (cartItems == null || cartItems.ItemId == 0)
                    {
                        int addItemsResult = await _dbHelper.AddItemToShoppingCart(item, userId);
                        if (addItemsResult < 1) return StatusCode(StatusCodes.Status500InternalServerError);

                        return Ok("Successful");
                    }

                    int newQuantity = cartItems.Quantity + item.Quantity;
                    int updateItemQuantity = await _dbHelper.UpdateShoppingCartItemQuantity(item.ItemId, newQuantity);

                    if (updateItemQuantity < 1) return StatusCode(StatusCodes.Status500InternalServerError);

                    return Ok("Successful");

                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("RemoveItem")]
        public async Task<IActionResult> RemoveItemFromCart(int itemId)
        {
            try
            {
                if (itemId < 1) return BadRequest("Invalid itemId");

                int userId = GetCurrentUserId();
                var response = await _dbHelper.RemoveItemFromCart(userId, itemId);

                if (response < 1) return StatusCode(StatusCodes.Status500InternalServerError);

                return Ok("Successful");
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetUserCartItems")]
        public async Task<IActionResult> ListAllItemsInShoppingCart()
        {
            try
            {
                int userId = GetCurrentUserId();
                var items = await _dbHelper.GetAllItemsInShoppingCart(userId);

                return Ok(items);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> ShowAllShoppingCarts(DateTime? time, string phoneNumber = "", int quantity = 0, string itemName = "", int pageSize = 10, int pageNo = 1)
        {
            try
            {
                var items = await _dbHelper.GetAllShoppingCart();
                if (!string.IsNullOrWhiteSpace(phoneNumber)) return Ok(items.Where(n => n.PhoneNumber == phoneNumber));
                if (time.HasValue) return Ok(items.Where(n => n.AddedAt == time));
                if (quantity > 0) return Ok(items.Where(n => n.Quantity == quantity));
                if (!string.IsNullOrWhiteSpace(itemName)) return Ok(items.FirstOrDefault(n => n.ItemName == itemName));

                return Ok(
                    items.OrderBy(n => n.PhoneNumber)
                        .ThenBy(n => n.AddedAt)
                    .ThenBy(n => n.Quantity)
                        .ThenBy(n => n.ItemName)
                        .Skip(pageSize * (pageNo - 1))
                        .Take(pageSize)
                );


            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetAllStock")]
        public async Task<IActionResult> ListAllShopItems(int pageSize = 10, int pageNo = 1)
        {
            try
            {
                var items = await _dbHelper.GetAllShoppingItems();
                return Ok(items.Skip(pageSize * (pageNo - 1))
                    .Take(pageSize));
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("GetShoppingCartItem")]
        public async Task<IActionResult> GetShoppingCartItembyId(int itemId)
        {
            try
            {
                if (itemId < 1) return BadRequest("Invalid itemId");
                var currentUserId = GetCurrentUserId();

                var item = await _dbHelper.GetItemFromCart(currentUserId, itemId);
                return Ok(item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private int GetCurrentUserId()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Sid).Value;
                return Int32.Parse(userId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                return 1;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Data.Crypto;
using Hubtel.eCommerce.Cart.Data.Interfaces;
using Hubtel.eCommerce.Cart.Data.Utility;
using Hubtel.eCommerce.Cart.Models.ApiModels;
using Hubtel.eCommerce.Cart.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationApiController : ControllerBase
	{
		private readonly IDbHelper _dbHelper;
		private readonly Logger _logger;
		private readonly string _issuer;
		private readonly string _audience;
		private readonly string _secret;

		private dynamic userClaims;
		private dynamic config;


		public AuthenticationApiController(IDbHelper dbHelper, Logger logger, IConfiguration configuration)
		{
			_dbHelper = dbHelper;
			_logger = logger;

			_issuer = configuration["Jwt:Issuer"];
			_audience = configuration["Jwt:Audience"];
			_secret = configuration["Jwt:Secret"];

			config = configuration;
		}

		[HttpPost("Createuser")]
		public async Task<IActionResult> CreateNewUser(NewUserApiModel user)
		{
			try
			{
				if (ModelState.IsValid)
				{
					int msisdn;
					var phone = int.TryParse(user.Msisdn, out msisdn);

					if (!phone) return BadRequest("Invalid Msisdn/Phone value");

					var existingUser = await _dbHelper.GetUserByUserName(user.UserName.ToLower());


					if (existingUser != null) return Ok("User already exists");

					user.Password = Cypher.EncryptSHA256(user.Password);
					_ = await _dbHelper.CreateUser(
						user.UserName.ToLower().Trim().Replace(" ", ""), user.FullName.Trim(),
						user.Msisdn, user.Password.Trim());
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

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginApiModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var user = await _dbHelper.GetUserByUserName(model.Username.ToLower().Trim());
					if (user == null) return Ok("Account does not exist");

					model.Password = Cypher.EncryptSHA256(model.Password);

					if (user.Password == model.Password)
					{
						var token = CompleteSignIn(user);
						return Ok(new { Token = token, Message = "Login Successful" });
					}

					return Ok("Invalid username or password");
				}

				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception e)
			{
				_logger.LogError(e);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[Authorize]
		[HttpGet("refresh-token")]
		public async Task<IActionResult> RefreshToken()
		{
			try
			{
				var userId = GetCurrentUserId();
				var user = await _dbHelper.GetUserById(userId);

				var jwtToken = CompleteSignIn(user);
				return Ok(new { Token = jwtToken, Message = "Successful" });
			}
			catch (Exception e)
			{
				_logger.LogError(e);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		private dynamic CompleteSignIn(UserDataModel model)
		{
			userClaims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.UniqueName, model.FullName),
				new Claim(ClaimTypes.Name, model.FullName),
				new Claim(JwtRegisteredClaimNames.Sid, model.UserId.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, model.UserId.ToString()),
				new Claim(ClaimTypes.Sid, model.UserId.ToString())
			};

			var jwt = GenerateJwtToken();

			return jwt;
		}


		private dynamic GenerateJwtToken()
		{
			try
			{
				var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
				var signingCredentials =
					new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

				var tt = signingCredentials.ToString();

				var tokenExpiry = DateTime.Now.AddHours(1);

				var tokenOptions = new JwtSecurityToken(
					issuer: _issuer,
					audience: _audience,
					notBefore: DateTime.Now,
					claims: userClaims,
					expires: tokenExpiry,
					signingCredentials: signingCredentials
				);

				var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
				var jwt = new { token = tokenString, expiry = tokenExpiry };

				return jwt;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				return null;
			}
		}


		private int GetCurrentUserId()
		{
			try
			{
				var userId = User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Sid).Value;
				return int.Parse(userId);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex);
				return 1;
			}
		}
	}
}
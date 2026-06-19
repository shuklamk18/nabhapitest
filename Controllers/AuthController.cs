using EFModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static APIsNABH.RequestDto;

namespace APIsNABH.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		public readonly IConfiguration _config;
		private nabhEntities _db = new nabhEntities();
		public AuthController(IConfiguration configuration) {
			_config = configuration;
 		} 
		[HttpPost]
		public IActionResult getToken(string ApiKey, string ApiSecret)
		{

			string secretHash = HashSecret(ApiSecret);

			//BCrypt.Net.BCrypt.HashPassword(ApiSecret);
			var keys = _db.tblApiKeys.Include(x=>x.ApiPermissions).Where(x => x.ApiKey == ApiKey).FirstOrDefault();

			if(keys != null)
			{
				bool isValid =
	secretHash == keys.ApiSecretHash; 

				if (isValid)
				{
					List<string> premissions = new List<string>();
					foreach(var i in keys.ApiPermissions)
					{
						premissions.Add(i.Permission);
					} 
					int expires_in = 3600;
					var token =
					GenerateToken(
					   ApiKey,
					   ApiSecret, keys.ApiUserId.ToString(), expires_in, premissions);

					if (token == null)
						return Unauthorized();

					return Ok(new
					{
						access_token = token,
						token_type = "Bearer",
						expires_in = expires_in
					});
				}
				else
				{
					return Unauthorized("Invalid ApiSecret!");

				}

			}
			else
			{
				return Unauthorized("Invalid ApiKey!");
			}
		}
		public static string HashSecret(string secret)
		{
			using (var sha = SHA256.Create())
			{
				var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(secret));
				return Convert.ToBase64String(bytes);
			}
		}
		private string GenerateToken(string ApiKey,string ApiSecret, string ApiUserId, int expires_in, List<string> premissions)
		{
			var claims = new List<Claim>
		{
			new("ApiKey", ApiKey.ToString()),
			new("ApiUserId", ApiUserId.ToString()),

			//new("ApiSecret", ApiSecret.ToString())
		};

			foreach (var pr in premissions) {
				claims.Add(new Claim("premissions", pr));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

			var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(issuer: _config["Jwt:issuer"], audience: _config["Jwt:audience"],claims : claims, expires: DateTime.Now.AddMinutes(expires_in), signingCredentials: cred);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

}

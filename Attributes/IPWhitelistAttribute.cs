using EFModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace APIsNABH.Attributes
{
    public class IPWhitelistAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var db = context.HttpContext.RequestServices
           .GetRequiredService<nabhEntities>();

            string currentIP = context.HttpContext.Connection.RemoteIpAddress?.ToString();


            string apiKey;
            string ApiSecret;

            var userIdClaim = context.HttpContext.User.FindFirst("ApiKey");
            

            if (context.HttpContext.Request.Path == "/api/Auth/getToken")
            {
                apiKey = context.HttpContext.Request.Query["ApiKey"];
                ApiSecret = context.HttpContext.Request.Query["ApiSecret"];
                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(ApiSecret))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var Keys = db.tblApiKeys.Include(x => x.ApiPermissions).Where(x => x.ApiKey == apiKey ).FirstOrDefault();
                if (Keys.ApiSecretHash != HashSecret(ApiSecret))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

            }
            else if (userIdClaim != null)
            {
                apiKey = userIdClaim.Value;
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            var Key = db.tblApiKeys.Include(x => x.ApiPermissions).Where(x => x.ApiKey == apiKey).FirstOrDefault();

            if (Key == null) {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool Allowed = Key.ApiPermissions.Where(x => x.IPAddress == currentIP).Any();

            if (!Allowed) {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "IP Not Whitelisted"
                };
                return;
            }

            await next();
        }

        public static string HashSecret(string secret)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(secret));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}




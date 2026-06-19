using EFModels;

namespace APIsNABH.Middleware
{
	public class AuditMiddleware
	{
		private readonly RequestDelegate _next;
		public AuditMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context,IServiceScopeFactory scopeFactory)
		{
			await _next(context);

			using var scope = scopeFactory.CreateScope();

			var db = scope.ServiceProvider.GetRequiredService<nabhEntities>();
			
			var log = new APIAuditLog
			{
				Url = context.Request.Path,
				HttpMethod = context.Request.Method,
				IPAddress = context.Connection.RemoteIpAddress?.ToString(),
				StatusCode = context.Response.StatusCode,
				RequestTime = DateTime.UtcNow
			};

			var claim = context.User.FindFirst("ApiUserId");

			if (claim != null)
			{
				log.ApiUserId = Convert.ToInt32(claim.Value);
			}

			db.APIAuditLogs.Add(log);
			await db.SaveChangesAsync();
		}
	}
}

using APIsNABH.Attributes;
using APIsNABH.Middleware;
using EFModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRateLimiter(option =>
{
	option.AddFixedWindowLimiter("ApiPolicy", policy =>
	{
		policy.PermitLimit = 10;
		policy.Window = TimeSpan.FromSeconds(30);
		policy.QueueLimit = 0;
		policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
	});
	option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddScoped<nabhEntities>();
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler =
			System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	});

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer("Bearer", options =>
{
	options.TokenValidationParameters =
		new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer =
				builder.Configuration["Jwt:Issuer"],

			ValidAudience =
				builder.Configuration["Jwt:Audience"],

			IssuerSigningKey =
				new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(
						builder.Configuration["Jwt:Key"]))
		};
});
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "NABH API",
		Version = "v1"
	});

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter JWT Token"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider,PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IPWhitelistAttribute>();
builder.Services.AddControllers(options =>
{
	options.Filters.AddService<IPWhitelistAttribute>();
});

var app = builder.Build();



	app.UseSwagger();
	app.UseSwaggerUI();

app.UseMiddleware<AuditMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();

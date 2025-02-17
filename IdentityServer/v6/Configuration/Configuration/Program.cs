using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Configuration.EntityFramework;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServerConfiguration(opt => {})
    .AddClientConfigurationStore();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddConfigurationDbContext<ConfigurationDbContext>();

builder.Services.AddAuthentication("token")
    .AddJwtBearer("token", options =>
    {
        options.Authority = "https://localhost:5001";
        options.MapInboundClaims = false;
        
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("DCR", policy =>
    {
        policy.RequireClaim("scope", "IdentityServer.Configuration");
    });
});

var app = builder.Build();

app.MapDynamicClientRegistration().RequireAuthorization("DCR");

app.Run();

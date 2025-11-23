using Identity.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.UseOpenIddict();
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// OpenIddict configuration
builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(opt =>
    {
        opt.AllowPasswordFlow();
        opt.AllowRefreshTokenFlow();
        opt.SetAccessTokenLifetime(TimeSpan.FromHours(1));
        opt.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
        opt.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough();
    })
    .AddValidation(opt =>
    {
        opt.SetIssuer("https://localhost:5001/");
        opt.AddAudiences(builder.Configuration.GetSection("Audience").Get<string[]>()!);
        opt.UseSystemNetHttp();
        opt.UseAspNetCore();
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// add middleware for global exception handling
app.UseMiddleware<Identity.Api.Middlewares.ExceptionHandler>();

// Handle database seeding
using var scope = app.Services.CreateScope();
await DatabaseSeed.SeedRolesAndClients(scope.ServiceProvider);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

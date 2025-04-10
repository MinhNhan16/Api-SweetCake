using ApexCharts;
using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Service;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ cho Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<HttpClient>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApexCharts();
builder.Services.AddScoped<StatisticsService>();
// Thêm dịch vụ DbContext
builder.Services.AddDbContext<CakeShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddScoped<IUserService, UserService>();

// Thêm dịch vụ Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CakeShop API",
        Version = "v1",
        Description = "API for CakeShop application"
    });
});

// Thêm dịch vụ Authentication với JWT
var key = Encoding.UTF8.GetBytes("ThisIsAReallyStrongSecretKeyForJWT123!nhanptmps40527@gmail.com");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});



builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Cấu hình Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CakeShop API V1");
    });
}

app.UseCors("AllowBlazorClient");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.MapBlazorHub(); // Cần cho Blazor Server
app.MapFallbackToPage("/_Host"); // Định tuyến đến _Host.cshtml


app.Run();

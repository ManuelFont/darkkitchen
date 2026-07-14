using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Application.Services.DeliveryTypes;
using DarkKitchen.Application.Services.Orders;
using DarkKitchen.Application.Services.Permissions;
using DarkKitchen.Application.Services.ProductImports;
using DarkKitchen.Application.Services.Products;
using DarkKitchen.Application.Services.Promotions;
using DarkKitchen.Application.Services.Reports;
using DarkKitchen.Application.Services.Roles;
using DarkKitchen.Application.Services.Sessions;
using DarkKitchen.Application.Services.Users;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Repositories.Reports;
using DarkKitchen.Domain.Repositories.Sessions;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Infrastructure;
using DarkKitchen.Infrastructure.Repositories;
using DarkKitchen.WebApi.Configuration;
using DarkKitchen.WebApi.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = ModelStateResponseFactory.Handle;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if(string.IsNullOrEmpty(connectionString))
{
    throw new Exception("connectionString is not found");
}

builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IProductImporterProvider, ProductImporterProvider>();
builder.Services.AddScoped<IProductImportService, ProductImportService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDeliveryTypeRepository, DeliveryTypeRepository>();
builder.Services.AddScoped<IDeliveryTypeService, DeliveryTypeService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionService, PromotionService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPhoneValidator, UruguayPhoneValidator>();

builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<AuthenticationFilter>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

new AuditConfig().AddServices(builder.Services);

var corsConfig = new CorsConfig();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
corsConfig.AddPolicies(builder.Services);

var app = builder.Build();

corsConfig.UsePolicies(app);

if(app.Environment.IsDevelopment())
{
    var scalarConfig = new ScalarConfig();
    scalarConfig.Config(app);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

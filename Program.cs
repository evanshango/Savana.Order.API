using System.Text.Json.Serialization;
using log4net;
using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Savana.Order.API.Consumers;
using Savana.Order.API.Data;
using Savana.Order.API.Interfaces;
using Savana.Order.API.Services;
using Treasures.Common.Extensions;
using Treasures.Common.Interfaces;
using Treasures.Common.Middlewares;
using Treasures.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}).ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!,
        c => c.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});

//Inject services from Treasures.Common library
builder.Services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>))
    .AddScoped<IUnitOfWork>(s => new UnitOfWork(s.GetService<StoreContext>()!))
    .AddErrorResponse<ApiBehaviorOptions>().AddJwtAuthentication(
        builder.Configuration["Token:Key"], builder.Configuration["Token:Issuer"]
    ).AddSwaggerAuthenticated("Savana Order API Service", "v1");

builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();

builder.Services.AddRouting(opt => {
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

// Add MassTransit
builder.Services.AddMassTransit(config => {
    config.AddConsumer<ProductConsumer>();
    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h => {
                h.Username(builder.Configuration["RabbitMQ:Username"]);
                h.Password(builder.Configuration["RabbitMQ:Password"]);
            }
        );
        cfg.ReceiveEndpoint("product-events", c =>
            c.ConfigureConsumer<ProductConsumer>(ctx)
        );
    });
});

builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = long.MaxValue; });

builder.Services.AddLogging(c => c.ClearProviders());
builder.Logging.AddLog4Net();

GlobalContext.Properties["pid"] = Environment.ProcessId;
GlobalContext.Properties["appName"] = builder.Configuration["Properties:Name"];

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope()) {
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
    } catch (Exception ex) {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred during Order DB migration");
    }
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
    { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }
);
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.UseSavanaSwaggerDoc("Savana Order API Service v1");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var address = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(";").First();
app.Logger.LogInformation("Savana Order.API started on {Addr}", address);

app.Run();
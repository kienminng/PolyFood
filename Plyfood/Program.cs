using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plyfood.Config;
using Plyfood.Context;
using Plyfood.Helper;
using Plyfood.Helper.ResponseMessage;
using Plyfood.Service.Impl;
using Plyfood.Service.IService;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = builder.Configuration;
    var service = builder.Services;

    var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
    var JwtConfig = builder.Configuration.GetSection("JWT").Get<AppSettings>();
    var emailUrl = builder.Configuration.GetSection("EmailContent").Get<EmailContent>();
    var Html = builder.Configuration.GetSection("Html").Get<Html>();
    var Vnpay = builder.Configuration.GetSection("VnPay").Get<VnPay>();
    var ProductMessage = builder.Configuration.GetSection("ProductMessage").Get<ProductMessage>();
    var accountMessage = builder.Configuration.GetSection("UserAccountMessage").Get<AccountMessage>();
    var status = builder.Configuration.GetSection("StatusType").Get<Status>();
    var orderMessage = configuration.GetSection("Orders").Get<OrderMessage>();
    
    service.AddSingleton(orderMessage);
    service.AddSingleton(status);
    service.AddSingleton(accountMessage);
    service.AddSingleton(ProductMessage);
    service.AddSingleton(Vnpay);
    service.AddSingleton(Html);
    service.AddSingleton(emailUrl);
    service.AddSingleton(JwtConfig);
    service.AddSingleton(emailConfig);
    
    service.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] { }
            }
        });
    });

    service.AddDbContext<AppDbContext>(
        option
            => option.UseSqlServer(configuration.GetSection("ConnectionStrings").GetSection("data").Value)
    );

    service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configuration.GetSection("JWT").GetSection("ValidIssuer").Value,
                ValidAudience = (configuration.GetSection("JWT").GetSection("ValidAudience").Value),
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetSection("JWT").GetSection("Secrets").Value))
            };
        });

    service.AddScoped<IAccountService, AccountService>();
    service.AddScoped<IMailSender, MailSender>();
    service.AddScoped<IJwtService, JwtService>();
    service.AddScoped<IEnCodeService, EncodeService>();
    service.AddScoped<ITokenService, TokenService>();
    service.AddScoped<IProductService, ProductService>();
    service.AddScoped<IProductTypeService, ProductTypeService>();
    service.AddScoped<IOrderService, OrderService>();
    service.AddScoped<IOrderStatusService, OrderStatusService>();
    service.AddScoped<IProductReviewService, ProductReviewService>();
    service.AddScoped<ICartItemService, CartItemService>();
    service.AddScoped<ICartService, CartService>();
}


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(setup =>
{
    setup.AddPolicy("Default", corsPolicyBuilder =>
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandlingMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors("Default");

app.Run();
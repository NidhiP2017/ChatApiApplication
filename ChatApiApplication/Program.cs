using AutoMapper;
using ChatApiApplication;
using ChatApiApplication.Automapper;
using ChatApiApplication.Data;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using ChatApiApplication.CustomMiddleware;
using ChatApiApplication.Hubs;
using ChatApiApplication.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfileConfiguration());
});
IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

});

builder.Services.AddDbContext<ChatAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatAPIConnectionString")));

builder.Services.AddScoped<IChatUserService, ChatUserService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my 128 bits very long secret key.......")),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

// Add HttpContextAccessor
builder.Services.AddSingleton<IUrlHelper>(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    return new UrlHelper(actionContext);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger",
        builder => builder.
         WithOrigins("http://localhost:7157")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7157") // URL of the client project
         .AllowAnyHeader()
         .AllowAnyMethod());
});
builder.Services.AddSingleton<IConnection<string>, connection<string>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSwagger");

/*app.MapPost("Broadcast", async (string message, ChatHub context) =>
{
    await context.Clients.All.SendAsync("ReceiveMessage", message);
    return Results.NoContent();
});*/

app.UseHttpsRedirection();
app.UseMiddleware<CustomMiddleware>();
app.UseCors("AllowSwagger");
app.MapControllers();

app.Run();

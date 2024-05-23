/*using AutoMapper;
using ChatApiApplication.Automapper;
using ChatApiApplication.CustomMiddleware;
using ChatApiApplication.Data;
using ChatApiApplication.Hubs;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddHttpContextAccessor();
        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfileConfiguration());
        });
        IMapper mapper = mapperConfiguration.CreateMapper();
        builder.Services.AddSingleton(mapper);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
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

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        builder.Services.AddAuthentication().AddGoogle(options =>
        {
            IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = "1055043777002-t3albpi7if9q1537k5abk78q2reqp6ic.apps.googleusercontent.com"; // googleAuthNSection["ClientId"];
            options.ClientSecret = "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";//googleAuthNSection["ClientSecret"];
            //options.ClientId = googleAuthNSection["ClientId"];
            //options.ClientSecret = googleAuthNSection["ClientSecret"];
            options.CallbackPath = "/signin-google";
            options.Scope.Add("openid");
            options.Scope.Add("profile");
        });

        builder.Services.AddMvc();
        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<ChatAPIDbContext>
            (options => options.UseSqlServer
            (builder.Configuration.GetConnectionString("ChatAPIConnectionString")));

        builder.Services.AddScoped<IChatUserService, ChatUserService>();
        builder.Services.AddScoped<IMessagesService, MessagesService>();
        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ChatAPIDbContext>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseclassWithNoImplementationMiddleware();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                c.OAuthClientId("1055043777002-t3albpi7if9q1537k5abk78q2reqp6ic.apps.googleusercontent.com");
                c.OAuthAppName("ChatAPI - Swagger");
                c.OAuthUsePkce();
            });
        }

        app.MapHub<ChatHub>("/chathub");

        app.UseMiddleware<CustomMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }

}*/
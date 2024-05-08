using AutoMapper;
using ChatApiApplication;
using ChatApiApplication.CustomMiddleware;
using ChatApiApplication.Data;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //builder.Services.AddTransient<CustomMiddleware>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

        builder.Services.AddDbContext<ChatAPIDbContext>
            (options => options.UseSqlServer
            (builder.Configuration.GetConnectionString("ChatAPIConnectionString")));

        builder.Services.AddScoped<IChatUserService, ChatUserService>();    
        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ChatAPIDbContext>();

        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfiles());
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
           // app.UseclassWithNoImplementationMiddleware();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }
    /*public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseMiddleware<CustomMiddleware>();
        // Configure logging
        var configuration = builder.Configuration;
        var loggingSection = configuration.GetSection("Logging");
        // Configure logging
        loggerFactory.AddConfiguration(loggingSection);
        loggerFactory.AddConsole();
        loggerFactory.AddFile("Logs/mylog-{Date}.txt");

    }*/

}
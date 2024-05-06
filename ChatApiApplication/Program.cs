using AutoMapper;
using ChatApiApplication;
using ChatApiApplication.Data;
using ChatApiApplication.Services;
using Microsoft.EntityFrameworkCore;

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
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    /*public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }*/
}
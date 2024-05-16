using AutoMapper;
using ChatApiApplication;
using ChatApiApplication.CustomMiddleware;
using ChatApiApplication.Data;
using ChatApiApplication.Hubs;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        //builder.Services.AddSignalR();
        builder.Services.AddHttpContextAccessor();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter your JWT token into the textbox below",
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
                     Type=ReferenceType.SecurityScheme,
                     Id="Bearer"
                 }
             },
             new string[]{}
        }
        });
            var oauth2Scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                        TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect" },
                            { "profile", "User profile" },
                            { "email", "User email" }
                        }
                    }
                }
                
            };
            option.AddSecurityDefinition("oauth2", oauth2Scheme);
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { oauth2Scheme, new string[] { "openid", "profile", "email" } }
        });
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],           // Replace with your token issuer
                //ValidAudience = builder.Configuration["Jwt:Audience"],       // Replace with your token audience
                ValidAudience = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Replace with your secret key
            };
        });
        builder.Services.AddMvc();
        /*MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfileConfiguration());
        });
        IMapper mapper = mapperConfiguration.CreateMapper();*/
        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<ChatAPIDbContext>
            (options => options.UseSqlServer
            (builder.Configuration.GetConnectionString("ChatAPIConnectionString")));

        builder.Services.AddScoped<IChatUserService, ChatUserService>();  
        builder.Services.AddScoped<IMessagesService, MessagesService>();
        /*builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ChatAPIDbContext>()
        .AddDefaultTokenProviders();*/
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
       /* app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ChatHub>("/chathub");
        });*/

        app.UseMiddleware<CustomMiddleware>();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }
    
}
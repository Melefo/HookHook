using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HookHook.Backend.Services;
using System.Text.Json.Serialization;

namespace HookHook.Backend
{
    /// <summary>
    /// Configuration webapp
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// List of configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Startup constructor
        /// </summary>
        /// <param name="configuration">Base configuration</param>
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        /// <summary>
        /// Configure and add list services
        /// </summary>
        /// <param name="services">Where to register services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MongoService>();
            services.AddSingleton<TwitterService>();
            services.AddSingleton<DiscordService>();
            services.AddSingleton<GoogleService>();
            services.AddSingleton<TwitchService>();
            services.AddSingleton<SpotifyService>();
            services.AddScoped<UserService>();

            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HookHook", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Using the Authorization header with the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  { securitySchema, new[] { "Bearer" } }
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddAuthentication(x =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

            });
            services.AddRouting(x => x.LowercaseUrls = true);
            services.AddSingleton<MongoService>();
        }

        /// <summary>
        /// Enable services
        /// </summary>
        /// <param name="app">Our application</param>
        /// <param name="env">Environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseWebSockets();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
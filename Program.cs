using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WebApiDia02.Data;
using WebApiDia2.Contract;
using WebApiDia2.Data;
using WebApiDia2.Mapping;
using WebApiDia2.NewFolder;
using WebApiDia2.NewFolder1;
using WebApiDia2.Repositories;
using WebApiDia2.Services;

namespace WebApiDia2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 2 Configuración de servicios
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // Configurar Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            // Agrega un registro de prueba
            Log.Information("Aplicación iniciada.");
            try
            {

                // Usa SeriLog como logger
                builder.Host.UseSerilog();

                var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();


                // 1 Para creacion del token, previo creacion de la clase
                builder.Services.AddSingleton(jwtSettings);
                builder.Services.AddSingleton<JwtTokenService>();

                // 3 Agregar autenticación JWT
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero // Opcional: Elimina el margen de 5 minutos en la expiración de tokens
                    };
                });
                builder.Services.AddControllers();

                // Configura el servicio de caching
                builder.Services.AddMemoryCache(); // Configura el servicio de caching
                builder.Services.AddSingleton<CacheService>(); // Registra el servicio de cache

                //Registrar DbContext 
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                      .AddInterceptors(new CustomDbCommandInterceptor())
                      .AddInterceptors(new PerformanceInterceptor()));
                //Registrar repositorios
                //ciclo peticio http
                builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                builder.Services.AddScoped<IProductRepository, ProductRepository>();

                builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

                //0. Add AutoMapper with the general Profile 
                builder.Services.AddAutoMapper(typeof(MappingProfile));

                // 4 Agregar swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtExample API", Version = "v1" });
                });
                var app = builder.Build();
                //5 Configuración del middleware  para swagger
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtExample API v1");
                    });
                }

                //6 Habilitar autenticación y autorización
                app.UseAuthentication();
                app.UseAuthorization();
                // Configure the HTTP request pipeline.

                app.UseHttpsRedirection();



                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación falló al iniciar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
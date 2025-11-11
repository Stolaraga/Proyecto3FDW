using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Api.Infrastructure.Repositories;
using Veterinaria.Api.Infrastructure.RepositoriesSql;
using Veterinaria.Api.Infrastructure.Seed;
using Veterinaria.Domain.Abstractions;
using Veterinaria.Domain.Entities;           
using Veterinaria.Domain.Services;
using Veterinaria.Api.Services;



namespace Veterinaria.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            SqlMapper.AddTypeHandler(new DateOnlyHandler());


            builder.Services.AddSingleton<IClock, SystemClock>();

            // InMemory store y repos
            builder.Services.AddSingleton<InMemoryStore>();
            builder.Services.AddSingleton<ClientesRepository>();
            builder.Services.AddSingleton<MascotasRepository>();
            builder.Services.AddSingleton<EmpleadosRepository>();
            builder.Services.AddSingleton<AtencionesRepository>();

            


            builder.Services.AddSingleton<IConnectionFactory, DapperConnectionFactory>();
            builder.Services.AddScoped<ICrudRepository<Cliente>, ClientesRepository>();            
            builder.Services.AddScoped<IClientesSqlRepository, ClientesSqlRepository>();
            builder.Services.AddScoped<IMascotasSqlRepository, MascotasSqlRepository>();
            builder.Services.AddScoped<IEmpleadosSqlRepository, EmpleadosSqlRepository>();
            builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
            builder.Services.AddScoped<IClienteService, ClienteService>();
            builder.Services.AddScoped<IMascotaService, MascotaService>();



            //builder.Services.AddSingleton<IConnectionFactory, DapperConnectionFactory>();





            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                var xml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(xmlPath))
                    opt.IncludeXmlComments(xmlPath);

                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Veterinaria API (Proyecto 2)",
                    Version = "v1.5 En conexion a la base de datos",
                    Description = "API REST en memoria para Clientes, Mascotas, Empleados, Atenciones y Reportes."
                });
            });


            // CORS (ajusta origen cuando tengas el MVC)
            const string CorsPolicy = "AllowLocalhost";
            builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
                p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            var app = builder.Build();

            // Semillas
            Seed.SeedData(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(CorsPolicy);
            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();



        }
    }
}

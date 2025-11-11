using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;            
using Microsoft.Extensions.Configuration;  
using Microsoft.Extensions.Logging;



namespace Veterinaria.Api.Infrastructure
{

    /// <summary>
    /// Fábrica de conexiones para SQL Server (SQL Express).
    /// Lee la cadena "ConnectionStrings:VeterinariaDb" desde appsettings.json.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Crea una conexión cerrada. Se usa con using 'using' y abre según necesidad.
        /// </summary>
        IDbConnection Create();

        /// <summary>
        /// Crea y abre la conexión de forma asíncrona.
        /// </summary>
        Task<IDbConnection> CreateOpenAsync(CancellationToken ct = default);
    }

    public sealed class DapperConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;
        private readonly ILogger<DapperConnectionFactory> _logger;

        public DapperConnectionFactory(IConfiguration configuration,
                                       ILogger<DapperConnectionFactory> logger)
        {
            _connectionString = configuration.GetConnectionString("VeterinariaDb")
                ?? throw new InvalidOperationException(
                    "No se encontró ConnectionStrings:VeterinariaDb en appsettings.json");

            _logger = logger;
        }

        public IDbConnection Create()
        {
            // Devuelve la conexión cerrada; quien llame decide cuándo abrir/cerrar.
            return new SqlConnection(_connectionString);
        }

        public async Task<IDbConnection> CreateOpenAsync(CancellationToken ct = default)
        {
            var cn = new SqlConnection(_connectionString);
            try
            {
                await cn.OpenAsync(ct).ConfigureAwait(false);
                return cn;
            }
            catch
            {
                cn.Dispose();
                _logger.LogError("No se pudo abrir la conexión a la BD con la cadena configurada.");
                throw;
            }
        }
    }



}

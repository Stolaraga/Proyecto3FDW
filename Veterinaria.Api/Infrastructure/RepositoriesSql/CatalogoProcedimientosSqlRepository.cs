using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface ICatalogoProcedimientosSqlRepository
    {
        /// <summary>
        /// Devuelve el catálogo de procedimientos. Por defecto solo activos.
        /// </summary>
        Task<IReadOnlyList<CatalogoProcedimientoReadDto>> GetAllAsync(bool incluirInactivos = false);

        /// <summary>
        /// Devuelve un ítem del catálogo por su código (p.ej. "CONSULTA").
        /// </summary>
        Task<CatalogoProcedimientoReadDto?> GetByCodigoAsync(string codigo);
    }

    public sealed class CatalogoProcedimientosSqlRepository : ICatalogoProcedimientosSqlRepository
    {
        private readonly IConnectionFactory _factory;

        public CatalogoProcedimientosSqlRepository(IConnectionFactory factory)
            => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public async Task<IReadOnlyList<CatalogoProcedimientoReadDto>> GetAllAsync(bool incluirInactivos = false)
        {
            // Usamos la tabla directamente con un predicado paramétrico para evitar duplicar SQL.
            const string sql = @"
SELECT 
    c.Codigo,
    c.Nombre,
    c.Incluye,
    c.Precio,
    c.Activo
FROM dbo.CatalogoProcedimientosMascota c
WHERE (@IncluirInactivos = 1 OR c.Activo = 1)
ORDER BY c.Nombre;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<CatalogoProcedimientoReadDto>(sql, new
            {
                IncluirInactivos = incluirInactivos ? 1 : 0
            });

            return rows.AsList();
        }

        public async Task<CatalogoProcedimientoReadDto?> GetByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Debe indicar un código de catálogo.", nameof(codigo));

            const string sql = @"
SELECT 
    c.Codigo,
    c.Nombre,
    c.Incluye,
    c.Precio,
    c.Activo
FROM dbo.CatalogoProcedimientosMascota c
WHERE c.Codigo = @Codigo;";

            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<CatalogoProcedimientoReadDto>(sql, new { Codigo = codigo });
        }
    }
}

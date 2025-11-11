using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface IClientesSqlRepository
    {
        Task<List<ClienteReadDto>> GetAllAsync();
        Task<ClienteReadDto?> GetAsync(Guid id);                 
        Task<ClienteReadDto> AddAsync(ClienteCreateDto dto);
        Task<bool> UpdateAsync(Guid id, ClienteUpdateDto dto);   
        Task<bool> DeleteAsync(Guid id);                         
    }

    public sealed class ClientesSqlRepository : IClientesSqlRepository
    {
        private readonly IConnectionFactory _factory;
        private readonly ILogger<ClientesSqlRepository> _logger;

        public ClientesSqlRepository(IConnectionFactory factory, ILogger<ClientesSqlRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<List<ClienteReadDto>> GetAllAsync()
        {
            const string sql = @"
SELECT 
    c.Id                      AS Id,               -- GUID de app
    c.Cedula,
    (c.Nombre + N' ' + c.Apellidos) AS NombreCompleto,
    c.Email,
    c.Telefono,
    c.CanalPreferido,
    c.Direccion,
    c.Activo,
    CAST(c.FechaRegistro AS date) AS FechaRegistro
FROM dbo.Clientes c
ORDER BY c.ClienteId; -- seguimos ordenando por identity si quieres";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<ClienteReadDto>(sql);
            return rows.ToList();
        }

        public async Task<ClienteReadDto?> GetAsync(Guid id)
        {
            const string sql = @"
SELECT 
    c.Id                      AS Id,
    c.Cedula,
    (c.Nombre + N' ' + c.Apellidos) AS NombreCompleto,
    c.Email,
    c.Telefono,
    c.CanalPreferido,
    c.Direccion,
    c.Activo,
    CAST(c.FechaRegistro AS date) AS FechaRegistro
FROM dbo.Clientes c
WHERE c.Id = @Id;";

            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<ClienteReadDto>(sql, new { Id = id });
        }

        public async Task<ClienteReadDto> AddAsync(ClienteCreateDto dto)
        {
            // Usamos OUTPUT inserted.Id para recuperar el GUID generado por DEFAULT
            const string insertSql = @"
INSERT INTO dbo.Clientes (Cedula, Nombre, Apellidos, Telefono, Email, Direccion, CanalPreferido)
OUTPUT inserted.Id
VALUES (@Cedula, @Nombre, @Apellidos, @Telefono, @Email, @Direccion, @CanalPreferido);";

            const string selectSql = @"
SELECT 
    c.Id                      AS Id,
    c.Cedula,
    (c.Nombre + N' ' + c.Apellidos) AS NombreCompleto,
    c.Email,
    c.Telefono,
    c.CanalPreferido,
    c.Direccion,
    c.Activo,
    CAST(c.FechaRegistro AS date) AS FechaRegistro
FROM dbo.Clientes c
WHERE c.Id = @Id;";

            using var cn = _factory.Create();
            var newId = await cn.ExecuteScalarAsync<Guid>(insertSql, new
            {
                dto.Cedula,
                dto.Nombre,
                dto.Apellidos,
                dto.Telefono,
                dto.Email,
                dto.Direccion,
                dto.CanalPreferido
            });

            var creado = await cn.QueryFirstAsync<ClienteReadDto>(selectSql, new { Id = newId });
            return creado;
        }

        public async Task<bool> UpdateAsync(Guid id, ClienteUpdateDto dto)
        {
            const string sql = @"
UPDATE dbo.Clientes
SET Cedula         = @Cedula,
    Nombre         = @Nombre,
    Apellidos      = @Apellidos,
    Telefono       = @Telefono,
    Email          = @Email,
    Direccion      = @Direccion,
    CanalPreferido = @CanalPreferido,
    Activo         = @Activo
WHERE Id           = @Id;";

            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                dto.Cedula,
                dto.Nombre,
                dto.Apellidos,
                dto.Telefono,
                dto.Email,
                dto.Direccion,
                dto.CanalPreferido,
                dto.Activo
            });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = @"DELETE FROM dbo.Clientes WHERE Id = @Id;";
            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}

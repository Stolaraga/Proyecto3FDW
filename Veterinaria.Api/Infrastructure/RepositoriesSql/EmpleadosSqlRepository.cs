using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Enums; // ← RolEmpleado

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface IEmpleadosSqlRepository
    {
        Task<List<EmpleadoReadDto>> GetAllAsync();
        Task<EmpleadoReadDto?> GetAsync(Guid id);
        Task<EmpleadoReadDto> AddAsync(EmpleadoCreateDto dto);
        Task<bool> UpdateAsync(Guid id, EmpleadoUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public sealed class EmpleadosSqlRepository : IEmpleadosSqlRepository
    {
        private readonly IConnectionFactory _factory;
        public EmpleadosSqlRepository(IConnectionFactory factory) => _factory = factory;

        public async Task<List<EmpleadoReadDto>> GetAllAsync()
        {
            const string sql = @"
SELECT 
    e.Id,
    (e.Nombre + N' ' + e.Apellidos)      AS NombreCompleto,
    e.Email,
    e.Telefono,
    e.Rol,                                -- int → enum (Dapper lo mapea)
    CAST(e.FechaIngreso AS date)          AS FechaIngreso,
    e.Activo
FROM dbo.Empleados e
ORDER BY e.EmpleadoId;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<EmpleadoReadDto>(sql);
            return rows.ToList();
        }

        public async Task<EmpleadoReadDto?> GetAsync(Guid id)
        {
            const string sql = @"
SELECT 
    e.Id,
    (e.Nombre + N' ' + e.Apellidos)      AS NombreCompleto,
    e.Email,
    e.Telefono,
    e.Rol,
    CAST(e.FechaIngreso AS date)          AS FechaIngreso,
    e.Activo
FROM dbo.Empleados e
WHERE e.Id = @Id;";

            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<EmpleadoReadDto>(sql, new { Id = id });
        }

        public async Task<EmpleadoReadDto> AddAsync(EmpleadoCreateDto dto)
        {
            // Opcional: mantener "Puesto" consistente con el Rol (si la columna existe y es NOT NULL en tu tabla original)
            string puestoTexto = dto.Rol.ToString(); // p.ej. "Veterinario", "Recepcionista", etc.

            // Si tu columna Puesto es NOT NULL, inclúyela en el INSERT.
            const string insertSql = @"
INSERT INTO dbo.Empleados (Nombre, Apellidos, Email, Telefono, Rol, FechaIngreso, Activo, Puesto)
OUTPUT inserted.Id
VALUES (@Nombre, @Apellidos, @Email, @Telefono, @Rol, @FechaIngreso, 1, @Puesto);";

            const string selectSql = @"
SELECT 
    e.Id,
    (e.Nombre + N' ' + e.Apellidos)      AS NombreCompleto,
    e.Email,
    e.Telefono,
    e.Rol,
    CAST(e.FechaIngreso AS date)          AS FechaIngreso,
    e.Activo
FROM dbo.Empleados e
WHERE e.Id = @Id;";

            using var cn = _factory.Create();
            var newId = await cn.ExecuteScalarAsync<Guid>(insertSql, new
            {
                dto.Nombre,
                dto.Apellidos,
                dto.Email,
                dto.Telefono,
                dto.Rol,              // enum → int (ok)
                FechaIngreso = dto.FechaIngreso, // DateOnly? (ya tienes DateOnlyHandler)
                Puesto = puestoTexto
            });

            var creado = await cn.QueryFirstAsync<EmpleadoReadDto>(selectSql, new { Id = newId });
            return creado;
        }

        public async Task<bool> UpdateAsync(Guid id, EmpleadoUpdateDto dto)
        {
            string puestoTexto = dto.Rol.ToString();

            const string sql = @"
UPDATE dbo.Empleados
SET Nombre       = @Nombre,
    Apellidos    = @Apellidos,
    Email        = @Email,
    Telefono     = @Telefono,
    Rol          = @Rol,
    FechaIngreso = @FechaIngreso,
    Activo       = @Activo,
    Puesto       = @Puesto   -- opcional, para mantener coherencia visual en la BD
WHERE Id = @Id;";

            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                dto.Nombre,
                dto.Apellidos,
                dto.Email,
                dto.Telefono,
                dto.Rol,
                FechaIngreso = dto.FechaIngreso,
                dto.Activo,
                Puesto = puestoTexto
            });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = @"DELETE FROM dbo.Empleados WHERE Id = @Id;";
            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}

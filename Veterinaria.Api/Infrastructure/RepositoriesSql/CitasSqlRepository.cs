using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Domain.DTOs;


namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{

    public interface ICitasSqlRepository
    {
        Task<List<CitaReadDto>> GetAllAsync(Guid? mascotaId = null, string? estado = null, DateTime? desde = null, DateTime? hasta = null);
        Task<CitaReadDto?> GetAsync(Guid id);
        Task<CitaReadDto> AddAsync(CitaCreateDto dto);
        Task<bool> UpdateAsync(Guid id, CitaUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public sealed class CitasSqlRepository : ICitasSqlRepository
    {
        private readonly IConnectionFactory _factory;
        public CitasSqlRepository(IConnectionFactory factory) => _factory = factory;

        public async Task<List<CitaReadDto>> GetAllAsync(Guid? mascotaId = null, string? estado = null, DateTime? desde = null, DateTime? hasta = null)
        {
            const string sql = @"
SELECT
    ci.Id                                           AS Id,
    m.Id                                            AS MascotaId,        -- GUID (app)
    ci.ServicioId                                   AS ServicioId,       -- INT
    e.Id                                            AS VeterinarioId,    -- GUID (app)
    ci.FechaHora,
    ci.Estado,
    ci.Notas,
    m.Nombre                                        AS NombreMascota,
    s.Nombre                                        AS Servicio,
    (emp.Nombre + N' ' + emp.Apellidos)             AS Veterinario
FROM dbo.Citas ci
JOIN dbo.Mascotas   m   ON m.MascotaId   = ci.MascotaId
JOIN dbo.Servicios  s   ON s.ServicioId  = ci.ServicioId
JOIN dbo.Empleados  emp ON emp.EmpleadoId= ci.VeterinarioId
JOIN dbo.Empleados  e   ON e.EmpleadoId  = ci.VeterinarioId
WHERE (@MascotaGuid IS NULL OR m.Id = @MascotaGuid)
  AND (@Estado IS NULL OR ci.Estado = @Estado)
  AND (@Desde  IS NULL OR ci.FechaHora >= @Desde)
  AND (@Hasta  IS NULL OR ci.FechaHora <  @Hasta)
ORDER BY ci.FechaHora DESC;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<CitaReadDto>(sql, new
            {
                MascotaGuid = mascotaId,
                Estado = estado,
                Desde = desde,
                Hasta = hasta
            });
            return rows.ToList();
        }

        public async Task<CitaReadDto?> GetAsync(Guid id)
        {
            const string sql = @"
SELECT
    ci.Id                                           AS Id,
    m.Id                                            AS MascotaId,
    ci.ServicioId                                   AS ServicioId,
    e.Id                                            AS VeterinarioId,
    ci.FechaHora,
    ci.Estado,
    ci.Notas,
    m.Nombre                                        AS NombreMascota,
    s.Nombre                                        AS Servicio,
    (emp.Nombre + N' ' + emp.Apellidos)             AS Veterinario
FROM dbo.Citas ci
JOIN dbo.Mascotas   m   ON m.MascotaId   = ci.MascotaId
JOIN dbo.Servicios  s   ON s.ServicioId  = ci.ServicioId
JOIN dbo.Empleados  emp ON emp.EmpleadoId= ci.VeterinarioId
JOIN dbo.Empleados  e   ON e.EmpleadoId  = ci.VeterinarioId
WHERE ci.Id = @Id;";

            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<CitaReadDto>(sql, new { Id = id });
        }

        public async Task<CitaReadDto> AddAsync(CitaCreateDto dto)
        {
            const string insertSql = @"
DECLARE @MascotaIdInt     int = (SELECT MascotaId   FROM dbo.Mascotas  WHERE Id = @MascotaGuid);
IF @MascotaIdInt IS NULL THROW 60001, 'Mascota GUID no existe', 1;

DECLARE @VeterinarioIdInt int = (SELECT EmpleadoId  FROM dbo.Empleados WHERE Id = @VeterinarioGuid);
IF @VeterinarioIdInt IS NULL THROW 60002, 'Veterinario GUID no existe', 1;

-- NO enviamos Id; lo genera DF_Citas_Id (newsequentialid())
INSERT INTO dbo.Citas (MascotaId, ServicioId, VeterinarioId, FechaHora, Estado, Notas)
OUTPUT inserted.Id
VALUES (@MascotaIdInt, @ServicioId, @VeterinarioIdInt, @FechaHora, @Estado, @Notas);";

            const string selectSql = @"
SELECT
    ci.Id                                           AS Id,
    m.Id                                            AS MascotaId,
    ci.ServicioId                                   AS ServicioId,
    e.Id                                            AS VeterinarioId,
    ci.FechaHora,
    ci.Estado,
    ci.Notas,
    m.Nombre                                        AS NombreMascota,
    s.Nombre                                        AS Servicio,
    (emp.Nombre + N' ' + emp.Apellidos)             AS Veterinario
FROM dbo.Citas ci
JOIN dbo.Mascotas   m   ON m.MascotaId   = ci.MascotaId
JOIN dbo.Servicios  s   ON s.ServicioId  = ci.ServicioId
JOIN dbo.Empleados  emp ON emp.EmpleadoId= ci.VeterinarioId
JOIN dbo.Empleados  e   ON e.EmpleadoId  = ci.VeterinarioId
WHERE ci.Id = @Id;";

            using var cn = _factory.Create();
            var newId = await cn.ExecuteScalarAsync<Guid>(insertSql, new
            {
                MascotaGuid = dto.MascotaId,
                dto.ServicioId,
                VeterinarioGuid = dto.VeterinarioId,
                dto.FechaHora,
                dto.Estado,
                dto.Notas
            });

            var creado = await cn.QueryFirstAsync<CitaReadDto>(selectSql, new { Id = newId });
            return creado;
        }

        public async Task<bool> UpdateAsync(Guid id, CitaUpdateDto dto)
        {
            const string sql = @"
DECLARE @MascotaIdInt     int = (SELECT MascotaId   FROM dbo.Mascotas  WHERE Id = @MascotaGuid);
IF @MascotaIdInt IS NULL THROW 60001, 'Mascota GUID no existe', 1;

DECLARE @VeterinarioIdInt int = (SELECT EmpleadoId  FROM dbo.Empleados WHERE Id = @VeterinarioGuid);
IF @VeterinarioIdInt IS NULL THROW 60002, 'Veterinario GUID no existe', 1;

UPDATE dbo.Citas
SET MascotaId     = @MascotaIdInt,
    ServicioId    = @ServicioId,
    VeterinarioId = @VeterinarioIdInt,
    FechaHora     = @FechaHora,
    Estado        = @Estado,
    Notas         = @Notas
WHERE Id = @Id;";

            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                MascotaGuid = dto.MascotaId,
                dto.ServicioId,
                VeterinarioGuid = dto.VeterinarioId,
                dto.FechaHora,
                dto.Estado,
                dto.Notas
            });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = @"DELETE FROM dbo.Citas WHERE Id = @Id;";
            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }



}

using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface IProcedimientosMascotasSqlRepository
    {


        Task<ProcedimientoMascotaReadDto?> GetAsync(Guid id);

        Task<IReadOnlyList<ProcedimientoMascotaReadDto>> GetAllAsync(Guid? mascotaId = null);

        Task<ProcedimientoMascotaReadDto> AddAsync(ProcedimientoMascotaCreateDto dto);
        Task<bool> UpdateAsync(Guid id, ProcedimientoMascotaUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);

    }



    public sealed class ProcedimientosMascotasSqlRepository : IProcedimientosMascotasSqlRepository
    {
        private readonly IConnectionFactory _factory;
        public ProcedimientosMascotasSqlRepository(IConnectionFactory factory) => _factory = factory;

        public async Task<ProcedimientoMascotaReadDto?> GetAsync(Guid id)
        {
            const string sql = @"
SELECT p.Id,
       m.Id              AS MascotaId,
       c.Id              AS ClienteId,
       e.Id              AS EmpleadoId,
       p.Tipo,
       CAST(p.Fecha AS date) AS Fecha,
       p.Notas
FROM dbo.ProcedimientoMascotas p
JOIN dbo.Mascotas  m ON p.MascotaId  = m.MascotaId
JOIN dbo.Clientes  c ON m.ClienteId  = c.ClienteId
LEFT JOIN dbo.Empleados e ON e.EmpleadoId = p.EmpleadoId
WHERE p.Id = @Id;";
            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<ProcedimientoMascotaReadDto>(sql, new { Id = id });
        }

        // Cambia a IReadOnlyList
        public async Task<IReadOnlyList<ProcedimientoMascotaReadDto>> GetAllAsync(Guid? mascotaId = null)
        {
            const string sql = @"
SELECT p.Id,
       m.Id              AS MascotaId,
       c.Id              AS ClienteId,
       e.Id              AS EmpleadoId,
       p.Tipo,
       CAST(p.Fecha AS date) AS Fecha,
       p.Notas
FROM dbo.ProcedimientoMascotas p
JOIN dbo.Mascotas  m ON p.MascotaId  = m.MascotaId
JOIN dbo.Clientes  c ON m.ClienteId  = c.ClienteId
LEFT JOIN dbo.Empleados e ON e.EmpleadoId = p.EmpleadoId
WHERE (@MascotaGuid IS NULL OR m.Id = @MascotaGuid)
ORDER BY p.Fecha DESC, p.ProcId DESC;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<ProcedimientoMascotaReadDto>(sql, new { MascotaGuid = mascotaId });
            return rows.AsList(); // IList<T> sirve como IReadOnlyList<T>
        }

        // Firma 100% Procedimiento*
        public async Task<ProcedimientoMascotaReadDto> AddAsync(ProcedimientoMascotaCreateDto dto)
        {
            const string insertSql = @"
DECLARE @MascotaIdInt int;
SELECT @MascotaIdInt = MascotaId FROM dbo.Mascotas WHERE Id = @MascotaGuid;
IF @MascotaIdInt IS NULL
BEGIN
    DECLARE @msg1 nvarchar(200);
    SET @msg1 = N'Mascota inexistente: ' + CONVERT(nvarchar(36), @MascotaGuid);
    RAISERROR(@msg1, 16, 1);
    RETURN;
END;

DECLARE @EmpleadoIdInt int = NULL;
IF @EmpleadoGuid IS NOT NULL
    SELECT @EmpleadoIdInt = EmpleadoId FROM dbo.Empleados WHERE Id = @EmpleadoGuid;

INSERT INTO dbo.ProcedimientoMascotas(MascotaId, EmpleadoId, Tipo, Fecha, Notas)
OUTPUT inserted.Id
VALUES (@MascotaIdInt, @EmpleadoIdInt, @Tipo, @Fecha, @Notas);";



            using var cn = _factory.Create();
            var newId = await cn.ExecuteScalarAsync<Guid>(insertSql, new
            {
                MascotaGuid = dto.MascotaId,
                ClienteGuid = dto.ClienteId == null || dto.ClienteId == Guid.Empty ? (Guid?)null : dto.ClienteId,
                EmpleadoGuid = dto.EmpleadoId == null || dto.EmpleadoId == Guid.Empty ? (Guid?)null : dto.EmpleadoId,
                Tipo = dto.Tipo.ToString(),
                Fecha = dto.Fecha.Date,   // DateTime -> date
                Notas = dto.Notas
            });

            return await GetAsync(newId) ?? throw new InvalidOperationException("No se pudo leer el procedimiento insertado.");
        }

        // Firma 100% Procedimiento*
        public async Task<bool> UpdateAsync(Guid id, ProcedimientoMascotaUpdateDto dto)
        {
            const string sql = @"
DECLARE @MascotaIdInt int = (SELECT MascotaId FROM dbo.Mascotas WHERE Id = @MascotaGuid);
IF @MascotaIdInt IS NULL 
    THROW 70001, ('Mascota inexistente: ' + CONVERT(varchar(36), @MascotaGuid)), 1;

IF @ClienteGuid IS NOT NULL AND @ClienteGuid <>
   (SELECT c.Id FROM dbo.Clientes c JOIN dbo.Mascotas m ON c.ClienteId = m.ClienteId WHERE m.MascotaId = @MascotaIdInt)
    THROW 70002, 'La mascota no pertenece al cliente indicado', 1;

DECLARE @EmpleadoIdInt int = NULL;
IF @EmpleadoGuid IS NOT NULL
    SELECT @EmpleadoIdInt = EmpleadoId FROM dbo.Empleados WHERE Id = @EmpleadoGuid;

UPDATE dbo.ProcedimientoMascotas
   SET MascotaId  = @MascotaIdInt,
       EmpleadoId = @EmpleadoIdInt,
       Tipo       = @Tipo,
       Fecha      = @Fecha,
       Notas      = @Notas
 WHERE Id = @Id;";


            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                MascotaGuid = dto.MascotaId,
                ClienteGuid = dto.ClienteId == null || dto.ClienteId == Guid.Empty ? (Guid?)null : dto.ClienteId,
                EmpleadoGuid = dto.EmpleadoId == null || dto.EmpleadoId == Guid.Empty ? (Guid?)null : dto.EmpleadoId,
                Tipo = dto.Tipo.ToString(),
                Fecha = dto.Fecha.Date,   // DateTime -> date
                Notas = dto.Notas
            });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = "DELETE FROM dbo.ProcedimientoMascotas WHERE Id = @Id;";
            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}



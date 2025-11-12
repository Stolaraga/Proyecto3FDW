using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Enums;

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

        // Row cruda desde SQL (Tipo como string), luego la mapeamos a DTO con enum
        private sealed record ProcRow
        {
            public Guid Id { get; init; }
            public Guid MascotaId { get; init; }
            public Guid? ClienteId { get; init; }
            public Guid? EmpleadoId { get; init; }
            public string Tipo { get; init; } = "";
            public DateTime Fecha { get; init; }
            public string? Notas { get; init; }
            public string? NombreMascota { get; init; }
            public decimal IvaPorcentaje { get; init; }
            public string Estado { get; init; } = "Agendado";
        }

        private static ProcedimientoMascotaReadDto Map(ProcRow r)
        {
            // Convertimos el string de BD al enum (acepta nombres como "Consulta", "Cirugia", etc.)
            if (!Enum.TryParse<TipoProcedimientoMascota>(r.Tipo, ignoreCase: true, out var tipoEnum))
                tipoEnum = TipoProcedimientoMascota.Consulta;

            return new ProcedimientoMascotaReadDto
            {
                Id = r.Id,
                MascotaId = r.MascotaId,
                ClienteId = r.ClienteId,
                EmpleadoId = r.EmpleadoId,
                Tipo = tipoEnum,
                Fecha = r.Fecha,          // preserva hora
                Notas = r.Notas,
                // Nuevas columnas de la tabla
                NombreMascota = r.NombreMascota,
                IvaPorcentaje = r.IvaPorcentaje,
                Estado = r.Estado
            };
        }

        public async Task<ProcedimientoMascotaReadDto?> GetAsync(Guid id)
        {
            const string sql = @"
SELECT p.Id,
       p.MascotaId,
       p.ClienteId,
       p.EmpleadoId,
       p.Tipo,
       p.Fecha,
       p.Notas,
       p.NombreMascota,
       p.IvaPorcentaje,
       p.Estado
FROM dbo.ProcedimientoMascotas p
WHERE p.Id = @Id;";

            using var cn = _factory.Create();
            var row = await cn.QueryFirstOrDefaultAsync<ProcRow>(sql, new { Id = id });
            return row is null ? null : Map(row);
        }

        public async Task<IReadOnlyList<ProcedimientoMascotaReadDto>> GetAllAsync(Guid? mascotaId = null)
        {
            const string sql = @"
SELECT p.Id,
       p.MascotaId,
       p.ClienteId,
       p.EmpleadoId,
       p.Tipo,
       p.Fecha,
       p.Notas,
       p.NombreMascota,
       p.IvaPorcentaje,
       p.Estado
FROM dbo.ProcedimientoMascotas p
WHERE (@MascotaGuid IS NULL OR p.MascotaId = @MascotaGuid)
ORDER BY p.Fecha DESC, p.ProcId DESC;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<ProcRow>(sql, new { MascotaGuid = mascotaId });
            var list = new List<ProcedimientoMascotaReadDto>();
            foreach (var r in rows) list.Add(Map(r));
            return list;
        }

        public async Task<ProcedimientoMascotaReadDto> AddAsync(ProcedimientoMascotaCreateDto dto)
        {
            if (dto.MascotaId == Guid.Empty)
                throw new InvalidOperationException("MascotaId es obligatorio.");
            if (dto.ClienteId is null || dto.ClienteId == Guid.Empty)
                throw new InvalidOperationException("ClienteId es obligatorio para insertar el procedimiento.");

            // Nota: dejamos que BD asigne defaults (IvaPorcentaje=13.00, Estado='Agendado').
            const string insertSql = @"
INSERT INTO dbo.ProcedimientoMascotas
    (MascotaId, ClienteId, EmpleadoId, Tipo, Fecha, Notas, NombreMascota)
OUTPUT inserted.Id
VALUES
    (@MascotaGuid, @ClienteGuid, @EmpleadoGuid, @Tipo, @Fecha, @Notas, @NombreMascota);";

            using var cn = _factory.Create();
            var newId = await cn.ExecuteScalarAsync<Guid>(insertSql, new
            {
                MascotaGuid = dto.MascotaId,
                ClienteGuid = dto.ClienteId,
                EmpleadoGuid = (dto.EmpleadoId == null || dto.EmpleadoId == Guid.Empty) ? (Guid?)null : dto.EmpleadoId,
                Tipo = dto.Tipo.ToString(),
                Fecha = dto.Fecha,                 // preserva hora (no .Date)
                Notas = dto.Notas,
                NombreMascota = (string?)null      // si quieres denormalizar, puedes setearlo desde UI/servicio
            });

            // Devolvemos el registro recién creado
            var created = await GetAsync(newId);
            if (created is null)
                throw new InvalidOperationException("No fue posible leer el procedimiento recién insertado.");
            return created;
        }

        public async Task<bool> UpdateAsync(Guid id, ProcedimientoMascotaUpdateDto dto)
        {
            if (dto.MascotaId == Guid.Empty)
                throw new InvalidOperationException("MascotaId es obligatorio.");

            const string sql = @"
UPDATE dbo.ProcedimientoMascotas
   SET MascotaId  = @MascotaGuid,
       ClienteId  = @ClienteGuid,
       EmpleadoId = @EmpleadoGuid,
       Tipo       = @Tipo,
       Fecha      = @Fecha,
       Notas      = @Notas
 WHERE Id = @Id;";

            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                MascotaGuid = dto.MascotaId,
                ClienteGuid = (dto.ClienteId == null || dto.ClienteId == Guid.Empty) ? (Guid?)null : dto.ClienteId,
                EmpleadoGuid = (dto.EmpleadoId == null || dto.EmpleadoId == Guid.Empty) ? (Guid?)null : dto.EmpleadoId,
                Tipo = dto.Tipo.ToString(),
                Fecha = dto.Fecha,     // preserva hora
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

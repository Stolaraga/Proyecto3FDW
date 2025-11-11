using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Veterinaria.Api.Infrastructure;     
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Enums;

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface IMascotasSqlRepository
    {
        Task<List<MascotaReadDto>> GetAllAsync(Guid? clienteId = null);
        Task<MascotaReadDto?> GetAsync(Guid id);
        Task<MascotaReadDto> AddAsync(MascotaCreateDto dto);
        Task<bool> UpdateAsync(Guid id, MascotaUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public sealed class MascotasSqlRepository : IMascotasSqlRepository
    {
        private readonly IConnectionFactory _factory;
        public MascotasSqlRepository(IConnectionFactory factory) => _factory = factory;

        public async Task<List<MascotaReadDto>> GetAllAsync(Guid? clienteId = null)
        {
            const string sql = @"
SELECT 
    m.Id                             AS Id,
    c.Id                             AS ClienteId,
    m.Nombre,
    e.Nombre                         AS Especie,          -- Dapper mapeará string → enum por nombre
    r.Nombre                         AS Raza,
    CASE m.Sexo 
        WHEN N'H' THEN N'Hembra'
        WHEN N'M' THEN N'Macho'
        ELSE N'Indeterminado'
    END                               AS Sexo,            -- Dapper mapeará string → enum por nombre
    CAST(m.FechaNac AS date)          AS FechaNacimiento,
    m.Activo
FROM dbo.Mascotas m
JOIN dbo.Clientes  c ON c.ClienteId = m.ClienteId
LEFT JOIN dbo.Especies e ON e.EspecieId = m.EspecieId
LEFT JOIN dbo.Razas    r ON r.RazaId    = m.RazaId
WHERE (@ClienteGuid IS NULL OR c.Id = @ClienteGuid)
ORDER BY m.MascotaId;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<MascotaReadDto>(sql, new { ClienteGuid = clienteId });
            return rows.ToList();
        }

        public async Task<MascotaReadDto?> GetAsync(Guid id)
        {
            const string sql = @"
SELECT 
    m.Id                             AS Id,
    c.Id                             AS ClienteId,
    m.Nombre,
    e.Nombre                         AS Especie,          
    r.Nombre                         AS Raza,
    CASE m.Sexo 
        WHEN N'H' THEN N'Hembra'
        WHEN N'M' THEN N'Macho'
        ELSE N'Indeterminado'
    END                               AS Sexo,            
    CAST(m.FechaNac AS date)          AS FechaNacimiento,
    m.Activo
FROM dbo.Mascotas m
JOIN dbo.Clientes  c ON c.ClienteId = m.ClienteId
LEFT JOIN dbo.Especies e ON e.EspecieId = m.EspecieId
LEFT JOIN dbo.Razas    r ON r.RazaId    = m.RazaId
WHERE m.Id = @Id;";

            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<MascotaReadDto>(sql, new { Id = id });
        }



        public async Task<MascotaReadDto> AddAsync(MascotaCreateDto dto)
        {

            const string insertSql = @"
DECLARE @ClienteIdInt int = (SELECT ClienteId FROM dbo.Clientes WHERE Id = @ClienteGuid);
IF @ClienteIdInt IS NULL THROW 50001, 'Cliente GUID no existe', 1;

DECLARE @EspecieNombre nvarchar(80) = @EspecieStr;
DECLARE @EspecieId int = (SELECT EspecieId FROM dbo.Especies WHERE Nombre = @EspecieNombre);
IF @EspecieId IS NULL THROW 50002, 'Especie no existe en tabla Especies', 1;

DECLARE @RazaId int = NULL;
IF @RazaNombre IS NOT NULL
BEGIN
    SELECT @RazaId = RazaId FROM dbo.Razas WHERE EspecieId = @EspecieId AND Nombre = @RazaNombre;
    IF @RazaId IS NULL
    BEGIN
        INSERT INTO dbo.Razas(EspecieId, Nombre) VALUES (@EspecieId, @RazaNombre);
        SET @RazaId = SCOPE_IDENTITY();
    END
END

/* No enviamos el Id; se genera con DEFAULT DF_Mascotas_Id */
INSERT INTO dbo.Mascotas (ClienteId, EspecieId, RazaId, Nombre, Sexo, FechaNac, Color, PesoKg, Activo)
OUTPUT inserted.Id
VALUES (@ClienteIdInt, @EspecieId, @RazaId, @Nombre, @SexoChar, @FechaNac, NULL, NULL, 1);";




            const string selectSql = @"
SELECT 
    m.Id                             AS Id,
    c.Id                             AS ClienteId,
    m.Nombre,
    e.Nombre                         AS Especie,
    r.Nombre                         AS Raza,
    CASE m.Sexo WHEN N'H' THEN N'Hembra' WHEN N'M' THEN N'Macho' ELSE N'Indeterminado' END AS Sexo,
    CAST(m.FechaNac AS date)         AS FechaNacimiento,
    m.Activo
FROM dbo.Mascotas m
JOIN dbo.Clientes  c ON c.ClienteId = m.ClienteId
LEFT JOIN dbo.Especies e ON e.EspecieId = m.EspecieId
LEFT JOIN dbo.Razas    r ON r.RazaId    = m.RazaId
WHERE m.Id = @Id;";

            using var cn = _factory.Create();

            var sexoChar = dto.Sexo switch
            {
                SexoMascota.Hembra => "H",
                SexoMascota.Macho => "M",
                _ => "I"
            };

            var newId = await cn.ExecuteScalarAsync<Guid>(insertSql, new
            {
                ClienteGuid = dto.ClienteId,
                EspecieStr = dto.Especie.ToString(), // "Perro", "Gato", …
                RazaNombre = dto.Raza,
                Nombre = dto.Nombre,
                SexoChar = sexoChar,
                FechaNac = dto.FechaNacimiento
            });

            var creado = await cn.QueryFirstAsync<MascotaReadDto>(selectSql, new { Id = newId });
            return creado;
        }




        public async Task<bool> UpdateAsync(Guid id, MascotaUpdateDto dto)
        {
            const string sql = @"
DECLARE @ClienteIdInt int = (SELECT ClienteId FROM dbo.Clientes WHERE Id = @ClienteGuid);
IF @ClienteIdInt IS NULL THROW 50001, 'Cliente GUID no existe', 1;

DECLARE @EspecieNombre nvarchar(80) = @EspecieStr;
DECLARE @EspecieId int = (SELECT EspecieId FROM dbo.Especies WHERE Nombre = @EspecieNombre);
IF @EspecieId IS NULL THROW 50002, 'Especie no existe en tabla Especies', 1;

DECLARE @RazaId int = NULL;
IF @RazaNombre IS NOT NULL
BEGIN
    SELECT @RazaId = RazaId FROM dbo.Razas WHERE EspecieId = @EspecieId AND Nombre = @RazaNombre;
    IF @RazaId IS NULL
    BEGIN
        INSERT INTO dbo.Razas(EspecieId, Nombre) VALUES (@EspecieId, @RazaNombre);
        SET @RazaId = SCOPE_IDENTITY();
    END
END

UPDATE dbo.Mascotas
SET ClienteId = @ClienteIdInt,
    EspecieId = @EspecieId,
    RazaId    = @RazaId,
    Nombre    = @Nombre,
    Sexo      = @SexoChar,
    FechaNac  = @FechaNac,
    Activo    = @Activo
WHERE Id = @Id;";

            using var cn = _factory.Create();

            var sexoChar = dto.Sexo switch
            {
                SexoMascota.Hembra => "H",
                SexoMascota.Macho => "M",
                _ => "I"
            };

            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                ClienteGuid = dto.ClienteId,
                EspecieStr = dto.Especie.ToString(),
                RazaNombre = dto.Raza,
                Nombre = dto.Nombre,
                SexoChar = sexoChar,
                FechaNac = dto.FechaNacimiento,
                Activo = dto.Activo
            });
            return rows > 0;
        }






        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = @"DELETE FROM dbo.Mascotas WHERE Id = @Id;";
            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}

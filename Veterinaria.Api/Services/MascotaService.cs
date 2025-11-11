using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;
using Veterinaria.Api.Infrastructure.RepositoriesSql;

namespace Veterinaria.Domain.Services
{
    public interface IMascotaService
    {
        Task<MascotaReadDto?> ObtenerAsync(MascotaReadDto inputConId);   // solo Id
        Task<IReadOnlyList<MascotaReadDto>> ListarAsync(Guid? clienteId = null);
        Task<MascotaReadDto> CrearAsync(MascotaCreateDto dto);
        Task<bool> ActualizarAsync(MascotaUpdateDto dtoConId);
        Task<bool> EliminarAsync(MascotaReadDto dtoConId);
    }

    public sealed class MascotaService : IMascotaService
    {
        private readonly IMascotasSqlRepository _repoSql;

        public MascotaService(IMascotasSqlRepository repoSql)
        {
            _repoSql = repoSql;
        }

        public async Task<MascotaReadDto?> ObtenerAsync(MascotaReadDto inputConId)
        {
            return await _repoSql.GetAsync(inputConId.Id);
        }

        public async Task<IReadOnlyList<MascotaReadDto>> ListarAsync(Guid? clienteId = null)
        {
            var list = await _repoSql.GetAllAsync(clienteId);
            return list;
        }

        public async Task<MascotaReadDto> CrearAsync(MascotaCreateDto dto)
        {
            var creado = await _repoSql.AddAsync(dto);
            return creado;
        }

        public async Task<bool> ActualizarAsync(MascotaUpdateDto dtoConId)
        {
            return await _repoSql.UpdateAsync(dtoConId.Id, dtoConId);
        }

        public async Task<bool> EliminarAsync(MascotaReadDto dtoConId)
        {
            return await _repoSql.DeleteAsync(dtoConId.Id);
        }
    }
}

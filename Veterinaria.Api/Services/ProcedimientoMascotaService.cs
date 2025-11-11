using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Api.Infrastructure.RepositoriesSql;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Domain.Services
{
    public interface IProcedimientoMascotaService
    {
        Task<ProcedimientoMascotaReadDto?> ObtenerAsync(Guid id);
        Task<IReadOnlyList<ProcedimientoMascotaReadDto>> ListarAsync(Guid? mascotaId = null);
        Task<ProcedimientoMascotaReadDto> CrearAsync(ProcedimientoMascotaCreateDto dto);
        Task<bool> ActualizarAsync(Guid id, ProcedimientoMascotaUpdateDto dto);
        Task<bool> EliminarAsync(Guid id);
    }

    public sealed class ProcedimientoMascotaService : IProcedimientoMascotaService
    {
        private readonly IProcedimientosMascotasSqlRepository _procRepo;
        private readonly IMascotasSqlRepository _mascRepo;

        public ProcedimientoMascotaService(
            IProcedimientosMascotasSqlRepository procRepo,
            IMascotasSqlRepository mascRepo)
        {
            _procRepo = procRepo;
            _mascRepo = mascRepo;
        }

        public Task<ProcedimientoMascotaReadDto?> ObtenerAsync(Guid id)
            => _procRepo.GetAsync(id);

        public Task<IReadOnlyList<ProcedimientoMascotaReadDto>> ListarAsync(Guid? mascotaId = null)
            => _procRepo.GetAllAsync(mascotaId);

        public async Task<ProcedimientoMascotaReadDto> CrearAsync(ProcedimientoMascotaCreateDto dto)
        {
            // Asegurar ClienteId si no viene en el DTO (se toma del dueño real de la mascota).
            if (dto.ClienteId is null || dto.ClienteId == Guid.Empty)
            {
                var mascota = await _mascRepo.GetAsync(dto.MascotaId);
                if (mascota is null)
                    throw new InvalidOperationException($"Mascota inexistente: {dto.MascotaId}");

                dto = dto with { ClienteId = mascota.ClienteId };
            }

            return await _procRepo.AddAsync(dto);
        }

        public async Task<bool> ActualizarAsync(Guid id, ProcedimientoMascotaUpdateDto dto)
        {
            // Igual que en Crear: si no viene ClienteId, lo resolvemos.
            if (dto.ClienteId is null || dto.ClienteId == Guid.Empty)
            {
                var mascota = await _mascRepo.GetAsync(dto.MascotaId);
                if (mascota is null)
                    throw new InvalidOperationException($"Mascota inexistente: {dto.MascotaId}");

                dto = dto with { ClienteId = mascota.ClienteId };
            }

            return await _procRepo.UpdateAsync(id, dto);
        }

        public Task<bool> EliminarAsync(Guid id)
            => _procRepo.DeleteAsync(id);
    }
}

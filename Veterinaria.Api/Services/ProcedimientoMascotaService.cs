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

    /// <summary>
    /// Orquesta la lógica de Procedimientos de Mascotas.
    /// - Completa ClienteId a partir de la Mascota cuando no viene.
    /// - Preserva la hora en 'Fecha' (DateTime completo).
    /// - Usa GUIDs (alineado a la BD nueva).
    /// </summary>
    public sealed class ProcedimientoMascotaService : IProcedimientoMascotaService
    {
        private readonly IProcedimientosMascotasSqlRepository _procRepo;

        // IMPORTANTE: usamos la interfaz real del repositorio de Mascotas que ya tienes en tu capa SQL.
        // No definimos MascotaReadDto aquí para evitar ambigüedad con Veterinaria.Domain.DTOs.MascotaReadDto.
        private readonly IMascotasSqlRepository _mascRepo;

        public ProcedimientoMascotaService(
            IProcedimientosMascotasSqlRepository procRepo,
            IMascotasSqlRepository mascRepo)
        {
            _procRepo = procRepo ?? throw new ArgumentNullException(nameof(procRepo));
            _mascRepo = mascRepo ?? throw new ArgumentNullException(nameof(mascRepo));
        }

        public Task<ProcedimientoMascotaReadDto?> ObtenerAsync(Guid id)
            => _procRepo.GetAsync(id);

        public Task<IReadOnlyList<ProcedimientoMascotaReadDto>> ListarAsync(Guid? mascotaId = null)
            => _procRepo.GetAllAsync(mascotaId);

        public async Task<ProcedimientoMascotaReadDto> CrearAsync(ProcedimientoMascotaCreateDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (dto.MascotaId == Guid.Empty)
                throw new InvalidOperationException("MascotaId es obligatorio.");

            Guid? clienteId = dto.ClienteId;

            if (clienteId is null || clienteId == Guid.Empty)
            {
                var mascota = await _mascRepo.GetAsync(dto.MascotaId);
                if (mascota is null)
                    throw new InvalidOperationException($"Mascota inexistente: {dto.MascotaId}");

                // TOMAMOS ClienteId de la mascota real
                clienteId = mascota.ClienteId;
                if (clienteId == Guid.Empty)
                    throw new InvalidOperationException("La mascota no tiene un Cliente asociado válido.");
            }

            // Construimos un nuevo DTO (evitando 'with', que requiere record)
            var dtoCompleto = new ProcedimientoMascotaCreateDto
            {
                MascotaId = dto.MascotaId,
                ClienteId = clienteId,
                EmpleadoId = dto.EmpleadoId,
                Tipo = dto.Tipo,
                Fecha = dto.Fecha,
                Notas = dto.Notas,
                Precio = dto.Precio           
            };


            return await _procRepo.AddAsync(dtoCompleto);
        }

        public async Task<bool> ActualizarAsync(Guid id, ProcedimientoMascotaUpdateDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (id == Guid.Empty) throw new InvalidOperationException("Id inválido.");
            if (dto.MascotaId == Guid.Empty)
                throw new InvalidOperationException("MascotaId es obligatorio.");

            Guid? clienteId = dto.ClienteId;

            if (clienteId is null || clienteId == Guid.Empty)
            {
                var mascota = await _mascRepo.GetAsync(dto.MascotaId);
                if (mascota is null)
                    throw new InvalidOperationException($"Mascota inexistente: {dto.MascotaId}");

                clienteId = mascota.ClienteId;
                if (clienteId == Guid.Empty)
                    throw new InvalidOperationException("La mascota no tiene un Cliente asociado válido.");
            }

            var dtoCompleto = new ProcedimientoMascotaUpdateDto
            {
                MascotaId = dto.MascotaId,
                ClienteId = clienteId,
                EmpleadoId = dto.EmpleadoId,
                Tipo = dto.Tipo,
                Fecha = dto.Fecha,   // preserva hora
                Notas = dto.Notas,
                Precio = dto.Precio
            };

            return await _procRepo.UpdateAsync(id, dtoCompleto);
        }

        public Task<bool> EliminarAsync(Guid id)
        {
            if (id == Guid.Empty) throw new InvalidOperationException("Id inválido.");
            return _procRepo.DeleteAsync(id);
        }
    }
}

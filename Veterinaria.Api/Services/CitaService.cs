using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Api.Infrastructure.RepositoriesSql;
using Veterinaria.Domain.DTOs;


namespace Veterinaria.Api.Services
{

    public interface ICitaService
    {
        Task<IReadOnlyList<CitaReadDto>> ListarAsync(Guid? mascotaId = null, string? estado = null, DateTime? desde = null, DateTime? hasta = null);
        Task<CitaReadDto?> ObtenerAsync(CitaReadDto inputConId);
        Task<CitaReadDto> CrearAsync(CitaCreateDto dto);
        Task<bool> ActualizarAsync(CitaUpdateDto dtoConId);
        Task<bool> EliminarAsync(CitaReadDto dtoConId);
    }

    public sealed class CitaService : ICitaService
    {
        private readonly ICitasSqlRepository _repo;
        public CitaService(ICitasSqlRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<CitaReadDto>> ListarAsync(Guid? mascotaId = null, string? estado = null, DateTime? desde = null, DateTime? hasta = null)
            => await _repo.GetAllAsync(mascotaId, estado, desde, hasta);

        public async Task<CitaReadDto?> ObtenerAsync(CitaReadDto inputConId)
            => await _repo.GetAsync(inputConId.Id);

        public async Task<CitaReadDto> CrearAsync(CitaCreateDto dto)
            => await _repo.AddAsync(dto);

        public async Task<bool> ActualizarAsync(CitaUpdateDto dtoConId)
            => await _repo.UpdateAsync(dtoConId.Id, dtoConId);

        public async Task<bool> EliminarAsync(CitaReadDto dtoConId)
            => await _repo.DeleteAsync(dtoConId.Id);
    }


}

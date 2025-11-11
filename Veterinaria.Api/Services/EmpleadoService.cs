using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Api.Infrastructure.RepositoriesSql;
using Veterinaria.Domain.DTOs;



namespace Veterinaria.Api.Services
{


    public interface IEmpleadoService
    {
        Task<EmpleadoReadDto?> ObtenerAsync(EmpleadoReadDto dtoConId);
        Task<IReadOnlyList<EmpleadoReadDto>> ListarAsync();
        Task<EmpleadoReadDto> CrearAsync(EmpleadoCreateDto dto);
        Task<bool> ActualizarAsync(EmpleadoUpdateDto dtoConId);
        Task<bool> EliminarAsync(EmpleadoReadDto dtoConId);
    }

    public sealed class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadosSqlRepository _repoSql;

        public EmpleadoService(IEmpleadosSqlRepository repoSql)
        {
            _repoSql = repoSql;
        }

        public async Task<EmpleadoReadDto?> ObtenerAsync(EmpleadoReadDto dtoConId)
            => await _repoSql.GetAsync(dtoConId.Id);

        public async Task<IReadOnlyList<EmpleadoReadDto>> ListarAsync()
            => await _repoSql.GetAllAsync();

        public async Task<EmpleadoReadDto> CrearAsync(EmpleadoCreateDto dto)
            => await _repoSql.AddAsync(dto);

        public async Task<bool> ActualizarAsync(EmpleadoUpdateDto dtoConId)
            => await _repoSql.UpdateAsync(dtoConId.Id, dtoConId);

        public async Task<bool> EliminarAsync(EmpleadoReadDto dtoConId)
            => await _repoSql.DeleteAsync(dtoConId.Id);
    }


}

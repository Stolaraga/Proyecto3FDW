using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;
using Veterinaria.Api.Infrastructure.RepositoriesSql; // IClientesSqlRepository

namespace Veterinaria.Domain.Services
{
    // Las firmas siguen recibiendo/retornando DTOs (el controller no cambia)
    public interface IClienteService
    {
        Task<ClienteReadDto?> ObtenerAsync(ClienteReadDto inputConId); // inputConId.Id poblado
        Task<IReadOnlyList<ClienteReadDto>> ListarAsync();
        Task<ClienteReadDto> CrearAsync(ClienteCreateDto dto);
        Task<bool> ActualizarAsync(ClienteUpdateDto dtoConId);         // dtoConId.Id poblado (viene de BaseUpdateDto)
        Task<bool> EliminarAsync(ClienteReadDto dtoConId);             // solo Id
    }

    public sealed class ClienteService : IClienteService
    {
        private readonly IClientesSqlRepository _repoSql;

        public ClienteService(IClientesSqlRepository repoSql)
        {
            _repoSql = repoSql;
        }

        public async Task<ClienteReadDto?> ObtenerAsync(ClienteReadDto inputConId)
        {
            // Va directo a SQL (Id debe coincidir con el tipo usado en DTOs/repos SQL: int)
            return await _repoSql.GetAsync(inputConId.Id);
        }

        public async Task<IReadOnlyList<ClienteReadDto>> ListarAsync()
        {
            var list = await _repoSql.GetAllAsync();
            return list;
        }

        public async Task<ClienteReadDto> CrearAsync(ClienteCreateDto dto)
        {
            var creado = await _repoSql.AddAsync(dto);
            return creado;
        }

        public async Task<bool> ActualizarAsync(ClienteUpdateDto dtoConId)
        {
            return await _repoSql.UpdateAsync(dtoConId.Id, dtoConId);
        }

        public async Task<bool> EliminarAsync(ClienteReadDto dtoConId)
        {
            return await _repoSql.DeleteAsync(dtoConId.Id);
        }
    }
}

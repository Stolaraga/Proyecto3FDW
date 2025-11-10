using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Entities;
using Veterinaria.Api.Infrastructure; 

namespace Veterinaria.Domain.Services
{
    // ===== Interfaz de servicio (viajan DTOs) =====
    public interface IClienteService
    {
        Task<ClienteReadDto?> ObtenerAsync(ClienteReadDto inputConId);   // inputConId.Id poblado
        Task<IReadOnlyList<ClienteReadDto>> ListarAsync();
        Task<ClienteReadDto> CrearAsync(ClienteCreateDto dto);
        Task<bool> ActualizarAsync(ClienteUpdateDto dtoConId);           // dtoConId.Id poblado (BaseUpdateDto)
        Task<bool> EliminarAsync(ClienteReadDto dtoConId);               // solo Id
    }

    // ===== Implementación =====
    public sealed class ClienteService : IClienteService
    {
        private readonly ICrudRepository<Cliente> _repo;

        public ClienteService(ICrudRepository<Cliente> repo)
        {
            _repo = repo;
        }

        public async Task<ClienteReadDto?> ObtenerAsync(ClienteReadDto inputConId)
        {
            // Si tu repo es síncrono (GetById), lo envolvemos con Task.FromResult.
            // Si fuera asíncrono (GetByIdAsync), cambia esta línea por: var entidad = await _repo.GetByIdAsync(inputConId.Id);
            var entidad = await Task.FromResult(_repo.Get(inputConId.Id));
            return entidad is null ? null : ToReadDto(entidad);
        }

        public async Task<IReadOnlyList<ClienteReadDto>> ListarAsync()
        {
            // Igual estrategia para GetAll / GetAllAsync
            var entidades = await Task.FromResult(_repo.GetAll());
            return entidades.Select(ToReadDto).ToList();
        }

        public async Task<ClienteReadDto> CrearAsync(ClienteCreateDto dto)
        {
            var entidad = FromCreateDto(dto);

            // Si tu PK es GUID generado en app, descomenta:
            // entidad.Id = Guid.NewGuid();

            // Add / AddAsync
            var creada = await Task.FromResult(_repo.Add(entidad));
            return ToReadDto(creada);
        }

        public async Task<bool> ActualizarAsync(ClienteUpdateDto dtoConId)
        {
            
            var existente = await Task.FromResult(_repo.Get(dtoConId.Id));
            if (existente is null) return false;

            MapUpdate(dtoConId, existente);

            // Update / UpdateAsync
            return await Task.FromResult(_repo.Update(existente));
        }

        public async Task<bool> EliminarAsync(ClienteReadDto dtoConId)
        {
            var existente = await Task.FromResult(_repo.Get(dtoConId.Id));
            if (existente is null) return false;

            // Delete por Id (ajusta si tu repo elimina por entidad)
            return await Task.FromResult(_repo.Delete(existente.Id));
        }

        // ====== Mapeos privados (DTO <-> Entity) ======
        private static Cliente FromCreateDto(ClienteCreateDto d) => new Cliente
        {
            Cedula = d.Cedula,
            Nombre = d.Nombre,
            Apellidos = d.Apellidos,
            Email = d.Email,
            Telefono = d.Telefono,
            CanalPreferido = d.CanalPreferido,
            Direccion = d.Direccion,
            Activo = true
            // FechaRegistro: ideal que lo asigne la BD (DEFAULT GETDATE()).
        };

        private static void MapUpdate(ClienteUpdateDto d, Cliente e)
        {
            e.Cedula = d.Cedula;
            e.Nombre = d.Nombre;
            e.Apellidos = d.Apellidos;
            e.Email = d.Email;
            e.Telefono = d.Telefono;
            e.CanalPreferido = d.CanalPreferido;
            e.Direccion = d.Direccion;
            e.Activo = d.Activo;
        }

        private static ClienteReadDto ToReadDto(Cliente e) => new ClienteReadDto
        {
            Id = e.Id,
            Cedula = e.Cedula,
            NombreCompleto = $"{e.Nombre} {e.Apellidos}",
            Email = e.Email,
            Telefono = e.Telefono,
            CanalPreferido = e.CanalPreferido,
            Direccion = e.Direccion,
            Activo = e.Activo,
            // Si la entidad usa DateTime en BD:
            FechaRegistro = e.FechaRegistro
        };
    }
}

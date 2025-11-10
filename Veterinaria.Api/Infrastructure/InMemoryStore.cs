using System.Collections.Concurrent;
using Veterinaria.Domain.Entities;


namespace Veterinaria.Api.Infrastructure
{

    public sealed class InMemoryStore
    {
        public ConcurrentDictionary<Guid, Cliente> Clientes { get; } = new();
        public ConcurrentDictionary<Guid, Mascota> Mascotas { get; } = new();
        public ConcurrentDictionary<Guid, Empleado> Empleados { get; } = new();
        public ConcurrentDictionary<Guid, ProcedimientoMascotas> ProcedimientoMascotas { get; } = new();
    }



}

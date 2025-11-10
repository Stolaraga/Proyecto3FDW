
using System;
using System.Collections.Generic;
using System.Linq;
using Veterinaria.Domain.Entities;

namespace Veterinaria.Api.Infrastructure.Repositories

{

    public sealed class EmpleadosRepository : ICrudRepository<Empleado>
    {
        private readonly InMemoryStore _db;
        public EmpleadosRepository(InMemoryStore db) => _db = db;

        public Empleado Add(Empleado e)
        {
            _db.Empleados[e.Id] = e;
            return e;
        }

        public bool Update(Empleado e)
        {
            if (!_db.Empleados.ContainsKey(e.Id)) return false;
            _db.Empleados[e.Id] = e;
            return true;
        }

        public bool Delete(Guid id) => _db.Empleados.TryRemove(id, out _);

        public Empleado? Get(Guid id)
            => _db.Empleados.TryGetValue(id, out var e) ? e : null;

        public List<Empleado> GetAll()
            => _db.Empleados.Values.ToList();
    }



}





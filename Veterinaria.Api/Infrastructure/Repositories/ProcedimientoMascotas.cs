using System;
using System.Collections.Generic;
using System.Linq;
using Veterinaria.Domain.Entities;

namespace Veterinaria.Api.Infrastructure.Repositories
{

    public sealed class AtencionesRepository : ICrudRepository<ProcedimientoMascotas>
    {
        private readonly InMemoryStore _db;
        public AtencionesRepository(InMemoryStore db) => _db = db;

        public ProcedimientoMascotas Add(ProcedimientoMascotas e)
        {
            _db.ProcedimientoMascotas[e.Id] = e;
            return e;
        }

        public bool Update(ProcedimientoMascotas e)
        {
            if (!_db.ProcedimientoMascotas.ContainsKey(e.Id)) return false;
            _db.ProcedimientoMascotas[e.Id] = e;
            return true;
        }

        public bool Delete(Guid id) => _db.ProcedimientoMascotas.TryRemove(id, out _);

        public ProcedimientoMascotas? Get(Guid id)
            => _db.ProcedimientoMascotas.TryGetValue(id, out var e) ? e : null;

        public List<ProcedimientoMascotas> GetAll()
            => _db.ProcedimientoMascotas.Values.ToList();
    }
}

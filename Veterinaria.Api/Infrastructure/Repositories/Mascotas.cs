using System;
using System.Collections.Generic;
using System.Linq;
using Veterinaria.Domain.Entities;



namespace Veterinaria.Api.Infrastructure.Repositories
{
    public sealed class MascotasRepository : ICrudRepository<Mascota>
    {
        private readonly InMemoryStore _db;
        public MascotasRepository(InMemoryStore db) => _db = db;

        public Mascota Add(Mascota e)
        {
            _db.Mascotas[e.Id] = e;
            return e;
        }

        public bool Update(Mascota e)
        {
            if (!_db.Mascotas.ContainsKey(e.Id)) return false;
            _db.Mascotas[e.Id] = e;
            return true;
        }

        public bool Delete(Guid id) => _db.Mascotas.TryRemove(id, out _);

        public Mascota? Get(Guid id)
            => _db.Mascotas.TryGetValue(id, out var e) ? e : null;

        
        public List<Mascota> GetAll()
            => _db.Mascotas.Values.ToList();
    }
}


using Veterinaria.Domain.Entities;


namespace Veterinaria.Api.Infrastructure.Repositories
{

    public sealed class AtencionesRepository : ICrudRepository<Atencion>
    {
        private readonly InMemoryStore _db;
        public AtencionesRepository(InMemoryStore db) => _db = db;

        public Atencion Add(Atencion e) { _db.Atenciones[e.Id] = e; return e; }
        public bool Delete(Guid id) => _db.Atenciones.TryRemove(id, out _);
        public Atencion? Get(Guid id) => _db.Atenciones.TryGetValue(id, out var e) ? e : null;
        public IEnumerable<Atencion> GetAll() => _db.Atenciones.Values;
        public bool Update(Atencion e) { if (!_db.Atenciones.ContainsKey(e.Id)) return false; _db.Atenciones[e.Id] = e; return true; }

        public IEnumerable<Atencion> GetByMascota(Guid mascotaId) => _db.Atenciones.Values.Where(a => a.MascotaId == mascotaId);
    }



}

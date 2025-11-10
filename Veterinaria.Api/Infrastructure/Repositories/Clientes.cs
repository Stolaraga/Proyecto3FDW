using System;
using System.Collections.Generic;
using System.Linq;
using Veterinaria.Domain.Entities;




namespace Veterinaria.Api.Infrastructure.Repositories
{


    public sealed class ClientesRepository : ICrudRepository<Cliente>
    {
        private readonly InMemoryStore _db;
        public ClientesRepository(InMemoryStore db) => _db = db;

        public Cliente Add(Cliente e) { _db.Clientes[e.Id] = e; return e; }
        public bool Delete(Guid id) => _db.Clientes.TryRemove(id, out _);
        public Cliente? Get(Guid id) => _db.Clientes.TryGetValue(id, out var e) ? e : null;
        public List<Cliente> GetAll() => _db.Clientes.Values.ToList();

        public bool Update(Cliente e) { if (!_db.Clientes.ContainsKey(e.Id)) return false; _db.Clientes[e.Id] = e; return true; }
    }


}

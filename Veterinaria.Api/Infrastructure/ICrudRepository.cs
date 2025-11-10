using System;
using System.Collections.Generic;

namespace Veterinaria.Api.Infrastructure
{
    /// <summary>
    /// Contrato CRUD simple para entidades con PK Guid.
    /// (Firmas alineadas con los repos existentes en /Infrastructure/Repositories)
    /// </summary>
    public interface ICrudRepository<T> where T : class
    {
        // Lecturas
        T? Get(Guid id);              // ← nombre corto "Get" (no GetById)
        List<T> GetAll();             // ← List<T> (no IReadOnlyList<T>)

        // Escrituras
        T Add(T entity);
        bool Update(T entity);
        bool Delete(Guid id);
    }
}

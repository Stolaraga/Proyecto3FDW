

namespace Veterinaria.Api.Infrastructure


{

    public interface ICrudRepository<T>
    {
        IEnumerable<T> GetAll();
        T? Get(Guid id);
        T Add(T entity);
        bool Update(T entity);
        bool Delete(Guid id);
    }



}

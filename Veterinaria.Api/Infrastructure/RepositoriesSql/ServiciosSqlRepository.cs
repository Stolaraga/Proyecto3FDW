using Dapper;

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface IServiciosSqlRepository
    {
        Task<ServicioRow?> GetByIdAsync(int servicioId);
        Task<ServicioRow?> GetByNombreAsync(string nombre);
        Task<ServicioRow> AddAsync(string nombre, decimal precioBase, bool activo = true);
        Task<IReadOnlyList<ServicioRow>> GetAllAsync(bool soloActivos = true);
        Task<bool> UpdateAsync(int id, string nombre, decimal precioBase, bool activo);
        Task<bool> DeleteAsync(int id);




    }

    public sealed class ServiciosSqlRepository : IServiciosSqlRepository
    {
        private readonly IConnectionFactory _factory;
        public ServiciosSqlRepository(IConnectionFactory factory) => _factory = factory;

        public async Task<ServicioRow?> GetByIdAsync(int servicioId)
        {
            const string sql = @"SELECT TOP(1) ServicioId, Nombre, PrecioBase, Activo
                                 FROM dbo.Servicios WHERE ServicioId = @ServicioId;";
            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<ServicioRow>(sql, new { ServicioId = servicioId });
        }

        public async Task<bool> UpdateAsync(int id, string nombre, decimal precioBase, bool activo)
        {
            const string sql = @"
UPDATE dbo.Servicios
SET    Nombre = @Nombre,
       PrecioBase = @PrecioBase,
       Activo = @Activo
WHERE  ServicioId = @Id;";

            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = id,
                Nombre = nombre,
                PrecioBase = precioBase,
                Activo = activo
            });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"DELETE FROM dbo.Servicios WHERE ServicioId = @Id;";
            using var cn = _factory.Create();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }



        public async Task<ServicioRow?> GetByNombreAsync(string nombre)
        {
            const string sql = @"SELECT TOP(1) ServicioId, Nombre, PrecioBase, Activo
                                 FROM dbo.Servicios
                                 WHERE Activo = 1 AND LOWER(Nombre) = LOWER(@Nombre);";
            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<ServicioRow>(sql, new { Nombre = nombre });
        }


        public async Task<ServicioRow> AddAsync(string nombre, decimal precioBase, bool activo = true)
        {
            const string sql = @"
INSERT INTO dbo.Servicios (Nombre, PrecioBase, Activo)
OUTPUT inserted.ServicioId, inserted.Nombre, inserted.PrecioBase, inserted.Activo
VALUES (@Nombre, @PrecioBase, @Activo);";

            using var cn = _factory.Create();
            return await cn.QuerySingleAsync<ServicioRow>(sql, new
            {
                Nombre = nombre,
                PrecioBase = precioBase,
                Activo = activo
            });
        }

            public async Task<IReadOnlyList<ServicioRow>> GetAllAsync(bool soloActivos = true)
            {
                const string sql = @"
SELECT  ServicioId, Nombre, PrecioBase, Activo
FROM    dbo.Servicios
WHERE   (@SoloActivos = 0 OR Activo = 1)
ORDER BY Nombre;";

                using var cn = _factory.Create();
                var rows = await cn.QueryAsync<ServicioRow>(sql, new { SoloActivos = soloActivos ? 1 : 0 });
                return rows.AsList();
            }

    }




    public sealed class ServicioRow
    {
        public int ServicioId { get; init; }
        public string Nombre { get; init; } = "";
        public decimal PrecioBase { get; init; }
        public bool Activo { get; init; }
    }


}

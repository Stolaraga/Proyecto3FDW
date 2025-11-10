using Veterinaria.Domain.Entities;
using Veterinaria.Domain.Enums;
using Veterinaria.Domain.Abstractions;


namespace Veterinaria.Api.Infrastructure.Seed

{

    public static class Seed
    {
        public static void SeedData(IServiceProvider sp)
        {
            var db = sp.GetRequiredService<InMemoryStore>();
            var clock = sp.GetRequiredService<IClock>();

            
                return;


           
        }

    }

}

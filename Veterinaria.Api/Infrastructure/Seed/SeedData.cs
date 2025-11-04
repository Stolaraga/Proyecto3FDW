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

            if (db.Clientes.Any()) return; // idempotente

            var c1 = new Cliente { Cedula = "10101010", Nombre = "Ana", Apellidos = "Solano", Email = "ana@ej.com", Telefono = "7000-0001" };
            var c2 = new Cliente { Cedula = "20202020", Nombre = "Luisito", Apellidos = "Comunicados", Email = "luis@ej.com", Telefono = "7000-0002" };
            db.Clientes[c1.Id] = c1; db.Clientes[c2.Id] = c2;

            var e1 = new Empleado { Nombre = "María", Apellidos = "Veterinaria", Rol = RolEmpleado.Veterinario };
            db.Empleados[e1.Id] = e1;

            var m1 = new Mascota { ClienteId = c1.Id, Nombre = "ElFirulais", Especie = Especie.Perro, Sexo = SexoMascota.Macho };
            var m2 = new Mascota { ClienteId = c2.Id, Nombre = "Mishingans", Especie = Especie.Gato, Sexo = SexoMascota.Hembra };
            db.Mascotas[m1.Id] = m1; db.Mascotas[m2.Id] = m2;

            var a1Id = Guid.NewGuid();
            db.Atenciones[a1Id] = new ProcedimientoMascotas
            {
                Id = a1Id,
                MascotaId = m1.Id,
                ClienteId = c1.Id,
                EmpleadoId = e1.Id,
                Tipo = TipoProcedimientoMascota.VacunacionAnual,
                Fecha = clock.Today.AddMonths(-11),
                Notas = "Vacuna anual múltiple"
            };

            var a2Id = Guid.NewGuid();
            db.Atenciones[a2Id] = new ProcedimientoMascotas
            {
                Id = a2Id,
                MascotaId = m2.Id,
                ClienteId = c2.Id,
                EmpleadoId = e1.Id,
                Tipo = TipoProcedimientoMascota.Consulta,
                Fecha = clock.Today.AddDays(-20),
                Notas = "Control"
            };
        }

    }

}

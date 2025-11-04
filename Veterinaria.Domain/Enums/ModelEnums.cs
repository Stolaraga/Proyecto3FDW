using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinaria.Domain.Enums
{


    public enum Especie { Perro = 1, Gato = 2, Ave = 3, Reptil = 4, Roedor = 5, Otro = 9 }
    public enum SexoMascota { Macho = 1, Hembra = 2, Indeterminado = 9 }
    public enum TipoProcedimientoMascota { VacunacionAnual = 1, Desparasitacion = 2, Consulta = 3, Cirugia = 4, Grooming = 5, Otro = 9 }
    public enum CanalContacto { Telefono = 1, Email = 2, WhatsApp = 3 }
    public enum RolEmpleado { Veterinario = 1, Asistente = 2, Administracion = 3, Groomer = 4, Otro = 9 }



}

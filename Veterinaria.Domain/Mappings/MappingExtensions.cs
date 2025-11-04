using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Entities;



namespace Veterinaria.Domain.Mappings
{

    public static class MappingExtensions
    {
        // ---- Clientes ----
        public static ClienteReadDto ToReadDto(this Cliente c) => new()
        {
            Id = c.Id,
            Cedula = c.Cedula,
            NombreCompleto = $"{c.Nombre} {c.Apellidos}",
            Email = c.Email,
            Telefono = c.Telefono,
            CanalPreferido = c.CanalPreferido,
            Direccion = c.Direccion,
            Activo = c.Activo,
            FechaRegistro = c.FechaRegistro
        };

        public static void Apply(this Cliente c, ClienteCreateDto dto)
        {
            c.Cedula = dto.Cedula;
            c.Nombre = dto.Nombre;
            c.Apellidos = dto.Apellidos;
            c.Email = dto.Email;
            c.Telefono = dto.Telefono;
            c.CanalPreferido = dto.CanalPreferido;
            c.Direccion = dto.Direccion;
        }


        public static void Apply(this Cliente c, ClienteUpdateDto dto)
        {
            c.Cedula = dto.Cedula;
            c.Nombre = dto.Nombre;
            c.Apellidos = dto.Apellidos;
            c.Email = dto.Email;
            c.Telefono = dto.Telefono;
            c.CanalPreferido = dto.CanalPreferido;
            c.Direccion = dto.Direccion;
            c.Activo = dto.Activo;
        }

        // ---- Mascotas ----

        public static MascotaReadDto ToReadDto(this Mascota m) => new()
        {
            Id = m.Id,
            ClienteId = m.ClienteId,
            Nombre = m.Nombre,
            Especie = m.Especie,
            Raza = m.Raza,
            Sexo = m.Sexo,
            FechaNacimiento = m.FechaNacimiento,
            Activo = m.Activo
        };

        public static void Apply(this Mascota m, MascotaCreateDto dto)
        {
            m.ClienteId = dto.ClienteId;
            m.Nombre = dto.Nombre;
            m.Especie = dto.Especie;
            m.Raza = dto.Raza;
            m.Sexo = dto.Sexo;
            m.FechaNacimiento = dto.FechaNacimiento;
        }

        public static void Apply(this Mascota m, MascotaUpdateDto dto)
        {
            m.ClienteId = dto.ClienteId;
            m.Nombre = dto.Nombre;
            m.Especie = dto.Especie;
            m.Raza = dto.Raza;
            m.Sexo = dto.Sexo;
            m.FechaNacimiento = dto.FechaNacimiento;
            m.Activo = dto.Activo;
        }




        // ---- Empleados ----
        public static EmpleadoReadDto ToReadDto(this Empleado e) => new()
        {
            Id = e.Id,
            NombreCompleto = $"{e.Nombre} {e.Apellidos}",
            Email = e.Email,
            Telefono = e.Telefono,
            Rol = e.Rol,
            FechaIngreso = e.FechaIngreso,
            Activo = e.Activo
        };


        public static void Apply(this Empleado e, EmpleadoCreateDto dto)
        {
            e.Nombre = dto.Nombre;
            e.Apellidos = dto.Apellidos;
            e.Email = dto.Email;
            e.Telefono = dto.Telefono;
            e.Rol = dto.Rol;
            e.FechaIngreso = dto.FechaIngreso ?? e.FechaIngreso;
        }


        public static void Apply(this Empleado e, EmpleadoUpdateDto dto)
        {
            e.Nombre = dto.Nombre;
            e.Apellidos = dto.Apellidos;
            e.Email = dto.Email;
            e.Telefono = dto.Telefono;
            e.Rol = dto.Rol;
            e.FechaIngreso = dto.FechaIngreso ?? e.FechaIngreso;
            e.Activo = dto.Activo;
        }


        // ---- Atenciones ----
        public static AtencionReadDto ToReadDto(this Atencion a) => new()
        {
            Id = a.Id,
            MascotaId = a.MascotaId,
            ClienteId = a.ClienteId,
            EmpleadoId = a.EmpleadoId,
            Tipo = a.Tipo,
            Fecha = a.Fecha,
            Notas = a.Notas
        };


        public static void Apply(this Atencion a, AtencionCreateDto dto)
        {
            a.MascotaId = dto.MascotaId;
            a.ClienteId = dto.ClienteId;
            a.EmpleadoId = dto.EmpleadoId;
            a.Tipo = dto.Tipo;
            a.Fecha = dto.Fecha;
            a.Notas = dto.Notas;
        }


        public static void Apply(this Atencion a, AtencionUpdateDto dto)
        {
            a.MascotaId = dto.MascotaId;
            a.ClienteId = dto.ClienteId;
            a.EmpleadoId = dto.EmpleadoId;
            a.Tipo = dto.Tipo;
            a.Fecha = dto.Fecha;
            a.Notas = dto.Notas;
        }





    }









    }

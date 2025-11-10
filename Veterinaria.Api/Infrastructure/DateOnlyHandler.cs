using System;
using System.Data;
using Dapper;

namespace Veterinaria.Api.Infrastructure
{
    /// <summary>
    /// TypeHandler para mapear columnas SQL DATE/DATETIME <-> C# DateOnly.
    /// Regístralo en Program.cs con: SqlMapper.AddTypeHandler(new DateOnlyHandler());
    /// </summary>
    public sealed class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            // Al enviar a SQL, usamos DateTime a medianoche
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }

        public override DateOnly Parse(object value)
        {
            // Al leer de SQL (date/datetime), convertimos a DateOnly
            return value switch
            {
                DateTime dt => DateOnly.FromDateTime(dt),
                DateOnly d => d,
                _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
            };
        }
    }
}

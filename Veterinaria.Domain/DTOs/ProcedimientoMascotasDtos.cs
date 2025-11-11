using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Veterinaria.Domain.Enums;

namespace Veterinaria.Domain.DTOs
{


    
    public sealed class TipoProcedimientoMascotaConverter : JsonConverter<TipoProcedimientoMascota>
    {
        public override TipoProcedimientoMascota Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var n))
                return (TipoProcedimientoMascota)n;

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString() ?? string.Empty;
                if (Enum.TryParse<TipoProcedimientoMascota>(s, ignoreCase: true, out var byName))
                    return byName;

                // intenta con PascalCase si viene camelCase
                var pascal = s.Length > 0 ? char.ToUpperInvariant(s[0]) + s[1..] : s;
                if (Enum.TryParse<TipoProcedimientoMascota>(pascal, ignoreCase: false, out var byPascal))
                    return byPascal;
            }

            throw new JsonException($"Valor de enum inválido para {nameof(TipoProcedimientoMascota)}.");
        }

        public override void Write(Utf8JsonWriter writer, TipoProcedimientoMascota value, JsonSerializerOptions options)
        {
            // emite camelCase para JS
            var name = Enum.GetName(typeof(TipoProcedimientoMascota), value) ?? value.ToString();
            var camel = name.Length > 0 ? char.ToLowerInvariant(name[0]) + name[1..] : name;
            writer.WriteStringValue(camel);
        }
    }

    
    public sealed class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] Formats = new[]
        {
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", // ISO completo
            "yyyy-MM-ddTHH:mm:ssK",
            "yyyy-MM-ddTHH:mmK",
            "yyyy-MM-dd",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Se esperaba una cadena para la fecha.");

            var s = reader.GetString() ?? string.Empty;

            // 1) ISO redondo
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
                return dt;

            // 2) Intentos exactos comunes (local)
            if (DateTime.TryParseExact(s, Formats, CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeLocal, out dt))
                return dt;

            throw new JsonException($"Formato de fecha/hora inválido: \"{s}\".");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // ISO-8601 sin zona (local) => más amigable al front si no usas TZ
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }

    

    
    public sealed record ProcedimientoMascotaCreateDto : BaseCreateDto
    {
        [Required] public Guid MascotaId { get; init; }     // dbo.Mascotas.Id
        public Guid? ClienteId { get; init; }               // opcional (valida pertenencia)
        public Guid? EmpleadoId { get; init; }              // opcional

        [Required, JsonConverter(typeof(TipoProcedimientoMascotaConverter))]
        public TipoProcedimientoMascota Tipo { get; init; } = TipoProcedimientoMascota.Consulta;

        [Required, DataType(DataType.DateTime), JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Fecha { get; init; }                // fecha/hora del procedimiento

        [StringLength(1000)] public string? Notas { get; init; }
    }

    
    public sealed record ProcedimientoMascotaUpdateDto : BaseUpdateDto
    {
        [Required] public Guid MascotaId { get; init; }
        public Guid? ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }

        [Required, JsonConverter(typeof(TipoProcedimientoMascotaConverter))]
        public TipoProcedimientoMascota Tipo { get; init; } = TipoProcedimientoMascota.Consulta;

        [Required, DataType(DataType.DateTime), JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Fecha { get; init; }

        [StringLength(1000)] public string? Notas { get; init; }
    }

    
    public sealed record ProcedimientoMascotaReadDto
    {
        public Guid Id { get; init; }
        public Guid MascotaId { get; init; }
        public Guid? ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }

        [JsonConverter(typeof(TipoProcedimientoMascotaConverter))]
        public TipoProcedimientoMascota Tipo { get; init; }

        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Fecha { get; init; }

        [MaxLength(1000)] public string? Notas { get; init; }
    }
        

    
}

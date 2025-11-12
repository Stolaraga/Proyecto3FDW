using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Veterinaria.Domain.Enums;

namespace Veterinaria.Domain.DTOs
{
    // ----------------------------
    // JsonConverters auxiliares
    // ----------------------------

    /// <summary>
    /// Converter robusto para mapear strings (camelCase/PascalCase) & números al enum TipoProcedimientoMascota.
    /// </summary>
    public sealed class TipoProcedimientoMascotaConverter : JsonConverter<TipoProcedimientoMascota>
    {
        public override TipoProcedimientoMascota Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Si viene numérico, mapea por valor
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var num))
            {
                if (Enum.IsDefined(typeof(TipoProcedimientoMascota), num))
                    return (TipoProcedimientoMascota)num;
                throw new JsonException($"Valor numérico inválido para {nameof(TipoProcedimientoMascota)}: {num}");
            }

            // Si viene como string, aceptamos varias formas (camelCase, PascalCase, mayúsculas)
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString() ?? string.Empty;

                // Intento directo (case-insensitive)
                if (Enum.TryParse<TipoProcedimientoMascota>(s, ignoreCase: true, out var byName))
                    return byName;

                // Si viene en camelCase, probamos PascalCase (primera mayúscula)
                if (s.Length > 0)
                {
                    var pascal = char.ToUpperInvariant(s[0]) + s[1..];
                    if (Enum.TryParse<TipoProcedimientoMascota>(pascal, ignoreCase: false, out var byPascal))
                        return byPascal;
                }

                // Último intento: sin espacios y sin guiones bajos
                var compact = s.Replace("_", "").Replace(" ", "");
                if (Enum.TryParse<TipoProcedimientoMascota>(compact, ignoreCase: true, out var byCompact))
                    return byCompact;
            }

            throw new JsonException($"Valor inválido para {nameof(TipoProcedimientoMascota)}.");
        }

        public override void Write(Utf8JsonWriter writer, TipoProcedimientoMascota value, JsonSerializerOptions options)
        {
            // Emitimos el nombre tal cual (PascalCase). Si prefieres camelCase, conviértelo aquí.
            writer.WriteStringValue(Enum.GetName(typeof(TipoProcedimientoMascota), value) ?? value.ToString());
        }
    }

    /// <summary>
    /// Acepta DateTime, ISO 8601, 'yyyy-MM-dd', 'dd/MM/yyyy', y variantes con hora.
    /// Escribe siempre en formato ISO 8601 "o" (round-trip).
    /// </summary>
    public sealed class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] Formats = new[]
        {
            "o",                       // 2025-11-11T23:59:59.0000000Z
            "yyyy-MM-ddTHH:mm:ss.FFFK",
            "yyyy-MM-ddTHH:mm:ssK",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    // Intento exacto con formatos conocidos (invariant + cultura local por si acaso)
                    if (DateTime.TryParseExact(s, Formats, CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal, out var dt))
                        return dt.ToLocalTime();

                    if (DateTime.TryParse(s, CultureInfo.CurrentCulture,
                            DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal, out var dt2))
                        return dt2.ToLocalTime();
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                // Epoch milliseconds opcional
                if (reader.TryGetInt64(out var epochMs))
                {
                    var epoch = DateTimeOffset.FromUnixTimeMilliseconds(epochMs);
                    return epoch.LocalDateTime;
                }
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Algunos frontends envían { "date": "...", "time": "..." }
                using (var doc = JsonDocument.ParseValue(ref reader))
                {
                    var root = doc.RootElement;
                    string? date = root.TryGetProperty("date", out var d) ? d.GetString() : null;
                    string? time = root.TryGetProperty("time", out var t) ? t.GetString() : null;

                    if (!string.IsNullOrWhiteSpace(date))
                    {
                        var combined = string.IsNullOrWhiteSpace(time) ? date : $"{date} {time}";
                        if (DateTime.TryParse(combined, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                            return dt;
                    }
                }
            }

            throw new JsonException("Formato de fecha/hora no soportado para 'Fecha'.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToLocalTime().ToString("o"));
    }

    // ----------------------------
    // DTOs de ProcedimientoMascota
    // ----------------------------

    public sealed class ProcedimientoMascotaCreateDto
    {
        [Required] public Guid MascotaId { get; init; }
        // ClienteId es obligatorio en BD; el Service lo completa si llega vacío.
        public Guid? ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }

        [Required]
        [JsonConverter(typeof(TipoProcedimientoMascotaConverter))]
        public TipoProcedimientoMascota Tipo { get; init; }

        [Required]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Fecha { get; init; }

        [MaxLength(1000)]
        public string? Notas { get; init; }
    }

    public sealed class ProcedimientoMascotaUpdateDto
    {
        [Required] public Guid MascotaId { get; init; }
        public Guid? ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }

        [Required]
        [JsonConverter(typeof(TipoProcedimientoMascotaConverter))]
        public TipoProcedimientoMascota Tipo { get; init; }

        [Required]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Fecha { get; init; }

        [MaxLength(1000)]
        public string? Notas { get; init; }
    }

    /// <summary>
    /// DTO de lectura alineado a la tabla nueva (incluye columnas denormalizadas y de estado/IVA).
    /// </summary>
    public sealed class ProcedimientoMascotaReadDto
    {
        public Guid Id { get; init; }
        public Guid MascotaId { get; init; }
        public Guid? ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }

        [JsonConverter(typeof(TipoProcedimientoMascotaConverter))]
        public TipoProcedimientoMascota Tipo { get; init; }

        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Fecha { get; init; }

        [MaxLength(1000)]
        public string? Notas { get; init; }

        // Nuevas columnas presentes en la tabla (para listar sin JOIN)
        public string? NombreMascota { get; init; }
        public decimal IvaPorcentaje { get; init; }
        public string Estado { get; init; } = "Agendado";
    }
}

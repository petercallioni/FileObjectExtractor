using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileObjectExtractor.Models.MagicBytes
{
    // Custom JSON converter to parse a sign string in the format "offset,hex"
    public class SignJsonConverter : JsonConverter<Sign>
    {
        public override Sign Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Ensure we are reading a string.
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected string token for a sign value.");
            }

            string? rawValue = reader.GetString();
            string[]? parts = rawValue?.Split(',');

            // There must be exactly two parts: offset and hex.
            if (parts == null || parts.Length == 0 || parts.Length != 2)
            {
                throw new JsonException("Invalid format for sign. Expected format: \"offset,hex\".");
            }

            if (!int.TryParse(parts[0].Trim(), out int offset))
            {
                throw new JsonException("Invalid offset; could not parse to an integer.");
            }

            // Trim the hex value just in case.
            string hexPart = parts[1].Trim();

            return new Sign(offset, hexPart);
        }

        public override void Write(Utf8JsonWriter writer, Sign value, JsonSerializerOptions options)
        {
            // Write out the sign as "offset,hex"
            writer.WriteStringValue($"{value.Offset},{value.Hex}");
        }
    }
}

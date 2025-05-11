using FileObjectExtractor.Converters;
using System.Text.Json.Serialization;

namespace FileObjectExtractor.Models.MagicBytes
{
    [JsonConverter(typeof(SignJsonConverter))]
    public record Sign(int Offset, string Hex)
    {
        public byte[] HexBytes => HexConverter.HexStringToByteArray(Hex, true);
    }
}

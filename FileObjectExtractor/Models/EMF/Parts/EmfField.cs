using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.Converters;
using System;
using System.Text;

namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfField
    {
        public int ByteLength { get; set; } = -1;
        public Type? Type { get; set; }
        public string RawValue { get; set; } = string.Empty;
        public dynamic Value { get; set; } = string.Empty;

        public EmfField(int byteLength = -1, Type? type = null)
        {
            ByteLength = byteLength;
            Type = type;
        }

        // Alternative constructors
        public EmfField(Type? type) : this(-1, type) { }
        public EmfField() : this(-1, null) { }

        public void Initialize(StringBuilder input)
        {
            if (ByteLength == -1)
            {
                throw new InvalidOperationException("Byte length of field not set");
            }

            RawValue = input.Shift(0, ByteLength * 2);

            Value = Type switch
            {
                null => RawValue,
                Type t when t == typeof(string) => HexConverter.HexToString(RawValue),
                Type t when t == typeof(uint) => HexConverter.LittleEndianHexToUInt(RawValue),
                Type t when t == typeof(float) => HexConverter.LittleEndianHexToFloat(RawValue),
                _ => RawValue,
            };
        }
    }
}
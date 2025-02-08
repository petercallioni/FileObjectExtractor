using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.Converters;
using System;
using System.Text;

namespace FileObjectExtractor.Models.EMF
{
    public class EmfField
    {
        public int ByteLength { get; set; }
        public Type? Type { get; set; }
        public string RawValue { get; set; }
        public dynamic Value { get; set; }
        public EmfField(int byteLength = -1, Type? type = null)
        {
            ByteLength = byteLength;
            Type = type;
            RawValue = string.Empty;
            Value = string.Empty;
        }

        // Alternative constructors
        public EmfField(Type? type) : this(-1, type) { }

        public EmfField() : this(-1, null) { }

        public void Intitialize(StringBuilder input)
        {
            if (ByteLength == -1)
            {
                throw new InvalidOperationException($"Byte length of field not set");
            }

            RawValue = input.Shift(0, ByteLength * 2);

            if (Type == null)
            {
                Value = RawValue;
            }
            else if (Type == typeof(string))
            {
                Value = HexConverter.HexToString(RawValue);
            }
            else if (Type == typeof(uint))
            {
                Value = HexConverter.LittleEndianHexToUInt(RawValue);
            }
            else if (Type == typeof(float))
            {
                Value = HexConverter.LittleEndianHexToFloat(RawValue);
            }
            else
            {
                Value = RawValue;
            }
        }
    }
}
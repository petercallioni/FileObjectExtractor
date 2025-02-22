using FileObjectExtractor.Extensions;
using FileObjectExtractor.Interfaces;
using FileObjectExtractor.Models.Converters;
using System;
using System.Text;

namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfField : IEmfField
    {
        public int ByteLength { get; set; } = -1;
        public string RawValue { get; set; } = string.Empty;
        public object Value { get; set; }

        // Constructor with byteLength parameter
        public EmfField(int byteLength = -1)
        {
            ByteLength = byteLength;
            Value = string.Empty;
        }

        // Parameterless constructor
        public EmfField() : this(-1) { }

        public virtual void Initialize(StringBuilder input)
        {
            if (ByteLength == -1)
            {
                throw new InvalidOperationException("Byte length of field not set");
            }

            RawValue = input.Shift(0, ByteLength * 2);

            Value = RawValue;
        }
    }

    public class EmfField<T> : EmfField
    {
        public new T? Value { get; set; }

        // Constructor with byteLength parameter
        public EmfField(int byteLength = -1) : base(byteLength)
        {
            Value = default(T);
        }

        // Parameterless constructor
        public EmfField() : this(-1) { }

        public override void Initialize(StringBuilder input)
        {
            if (ByteLength == -1)
            {
                throw new InvalidOperationException("Byte length of field not set");
            }

            RawValue = input.Shift(0, ByteLength * 2);

            Value = typeof(T) switch
            {
                Type t when t == typeof(string) => (T)(object)HexConverter.HexToString(RawValue),
                Type t when t == typeof(uint) => (T)(object)HexConverter.LittleEndianHexToUInt(RawValue),
                Type t when t == typeof(float) => (T)(object)HexConverter.LittleEndianHexToFloat(RawValue),
                _ => (T)(object)RawValue,
            };
        }
    }
}

using FileObjectExtractor.Converters;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.EMF.Parts;
using System;
using System.Collections.Generic;

namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfField : IEmfField
    {
        public int ByteLength { get; set; } = -1;
        public byte[] RawValue { get; set; } = [];
        public object Value { get; set; }

        // Constructor with byteLength parameter
        public EmfField(int byteLength = -1)
        {
            ByteLength = byteLength;
            Value = string.Empty;
        }

        // Parameterless constructor
        public EmfField() : this(-1) { }

        public virtual void Initialize(Queue<byte> data)
        {
            if (ByteLength == -1)
            {
                throw new InvalidOperationException("Byte length of field not set");
            }

            RawValue = data.DequeueMultiple(ByteLength);

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

        public override void Initialize(Queue<byte> data)
        {
            if (ByteLength == -1)
            {
                throw new InvalidOperationException("Byte length of field not set");
            }

            RawValue = data.DequeueMultiple(ByteLength);

            Value = typeof(T) switch
            {
                Type t when t == typeof(string) => (T)(object)HexConverter.HexToString(RawValue),
                Type t when t == typeof(int) => (T)(object)HexConverter.LittleEndianHexToInt(RawValue),
                Type t when t == typeof(uint) => (T)(object)HexConverter.LittleEndianHexToUInt(RawValue),
                Type t when t == typeof(float) => (T)(object)HexConverter.LittleEndianHexToFloat(RawValue),
                _ => (T)(object)RawValue,
            };
        }
    }
}

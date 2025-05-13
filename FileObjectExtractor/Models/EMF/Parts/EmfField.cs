using FileObjectExtractor.Converters;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.EMF.Parts;
using System;
using System.Collections.Generic;
using System.Text;

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

        // Parameterless constructor that calls the constructor with byteLength parameter
        public EmfField() : this(-1) { }

        /// <summary>
        /// Initializes the field by reading a specified number of bytes from the data queue.
        /// This method is virtual, allowing derived classes to provide their own initialization logic.
        /// </summary>
        /// <param name="data">The queue containing byte data to be read.</param>
        /// <returns>True if initialization was successful, otherwise false.</returns>
        public virtual bool Initialize(Queue<byte> data)
        {
            try
            {
                if (ByteLength == -1)
                {
                    throw new InvalidOperationException("Byte length of field not set");
                }
                RawValue = data.DequeueMultiple(ByteLength);
                Value = RawValue;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class EmfField<T> : EmfField
    {
        // New property that shadows the base class's Value property with a specific type T
        public new T? Value { get; set; }

        // Constructor with byteLength parameter that calls the base constructor
        public EmfField(int byteLength = -1) : base(byteLength)
        {
            Value = default(T);
        }

        // Parameterless constructor that calls the constructor with byteLength parameter
        public EmfField() : this(-1) { }

        /// <summary>
        /// Overrides the Initialize method to provide type-specific initialization logic.
        /// This method reads a specified number of bytes from the data queue and converts them to the appropriate type T.
        /// </summary>
        /// <param name="data">The queue containing byte data to be read.</param>
        /// <returns>True if initialization was successful, otherwise false.</returns>
        public override bool Initialize(Queue<byte> data)
        {
            try
            {
                if (ByteLength == -1)
                {
                    throw new InvalidOperationException("Byte length of field not set");
                }
                RawValue = data.DequeueMultiple(ByteLength);
                Value = typeof(T) switch
                {
                    Type t when t == typeof(string) => (T)(object)HexConverter.HexToString(RawValue, Encoding.Unicode),
                    Type t when t == typeof(int) => (T)(object)HexConverter.LittleEndianHexToInt(RawValue),
                    Type t when t == typeof(uint) => (T)(object)HexConverter.LittleEndianHexToUInt(RawValue),
                    Type t when t == typeof(float) => (T)(object)HexConverter.LittleEndianHexToFloat(RawValue),
                    _ => throw new NotSupportedException($"Type {typeof(T)} is not supported for conversion.")
                };
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
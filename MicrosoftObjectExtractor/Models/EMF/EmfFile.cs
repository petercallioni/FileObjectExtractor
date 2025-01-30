using MicrosoftObjectExtractor.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicrosoftObjectExtractor.Models.EMF
{
    public class EmfFile
    {
        public EmfFileHeader EmfFileHeader { get; set; }
        public EmfHeader EmfHeader { get; set; }
        public EmfHeaderExtension1 EmfHeaderExtension1 { get; set; }
        public EmfHeaderExtension2 EmfHeaderExtension2 { get; set; }
        public EmfDescriptionBuffer EmfDescriptionBuffer { get; set; }
        public EmfPixelFormatBuffer EmfPixelFormatBuffer { get; set; }
        public List<EmfTextRecord> EmfTextRecords { get; set; }

        public EmfFile()
        {
            EmfFileHeader = new();
            EmfHeader = new();
            EmfDescriptionBuffer = new();
            EmfPixelFormatBuffer = new();
            EmfHeaderExtension1 = new();
            EmfHeaderExtension2 = new();
            EmfDescriptionBuffer = new();
            EmfPixelFormatBuffer = new();
            EmfTextRecords = new();
        }
    }

    public abstract class EmfFilePart
    {
        protected List<EmfField> Fields = new List<EmfField>();

        public void Initialize(StringBuilder input)
        {
            foreach (EmfField field in Fields)
            {
                field.Intitialize(input);
            }
        }
    }
    public class EmfField
    {
        public int ByteLength { get; set; }
        public Type Type { get; set; }
        public string RawValue { get; set; }
        public dynamic Value { get; set; }
        public EmfField(int byteLength, Type type = null)
        {
            ByteLength = byteLength;
            Type = type;
        }

        public EmfField(Type type = null)
        {
            ByteLength = -1; // Variable, set later
            Type = type;
        }

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
                Value = HexToString(RawValue);
            }
            else if (Type == typeof(uint))
            {
                Value = LittleEndianHexToUInt(RawValue);
            }
            else if (Type == typeof(float))
            {
                Value = LittleEndianHexToFloat(RawValue);
            }
            else
            {
                Value = RawValue;
            }
        }

        private uint LittleEndianHexToUInt(string hexString)
        {
            // Convert hex string to byte array (little-endian)
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            // Convert little-endian byte array to unsigned integer
            uint uintValue = BitConverter.ToUInt32(bytes, 0);
            return uintValue;
        }

        private float LittleEndianHexToFloat(string hexString)
        {
            // Convert hex string to byte array (little-endian)
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            // Convert little-endian byte array to unsigned integer
            float uintValue = BitConverter.ToSingle(bytes, 0);
            return uintValue;
        }

        private string HexToString(string hexString)
        {
            // Ensure the hex string length is even
            if (hexString.Length % 4 != 0)
            {
                throw new ArgumentException("Invalid hex string length.");
            }

            // Convert hex string to byte array
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string byteValue = hexString.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteValue, 16);
            }

            // Decode bytes to string using UTF-16 encoding
            string result = Encoding.Unicode.GetString(bytes);
            return result;
        }
    }

    public class EmfFileHeader : EmfFilePart
    {
        public EmfField Type = new(4, typeof(uint));
        public EmfField Size = new(4, typeof(uint));

        public EmfFileHeader()
        {
            Fields.AddRange(new[]
              {
                Type, Size
            });
        }
    }
    public class EmfHeader : EmfFilePart
    {
        public EmfField Bounds = new(16);
        public EmfField Frame = new(16);
        public EmfField RecordSignature = new(4);
        public EmfField Version = new(4);
        public EmfField Bytes = new(4);
        public EmfField Records = new(4);
        public EmfField Handles = new(2);
        public EmfField Reserved = new(2);
        public EmfField nDescription = new(4, typeof(uint));
        public EmfField offDescription = new(4, typeof(uint));
        public EmfField nPalEntries = new(4);
        public EmfField Device = new(8);
        public EmfField Millimeters = new(8);

        public EmfHeader()
        {
            Fields.AddRange(new[]
              {
                Bounds, Frame, RecordSignature, Version, Bytes, Records, Handles, Reserved, nDescription, offDescription, nPalEntries, Device, Millimeters
            });
        }
    }

    public class EmfHeaderExtension1 : EmfFilePart
    {
        public EmfField cbPixelFormat = new(4, typeof(uint));
        public EmfField offPixelFormat = new(4, typeof(uint));
        public EmfField bOpenGl = new(4);

        public EmfHeaderExtension1()
        {
            Fields.AddRange(new[]
            {
               cbPixelFormat, offPixelFormat, bOpenGl
            });
        }
    }
    public class EmfHeaderExtension2 : EmfFilePart
    {
        public EmfField MicrometersX = new(4);
        public EmfField MicrometersY = new(4);

        public EmfHeaderExtension2()
        {
            Fields.AddRange(new[]
            {
               MicrometersX, MicrometersY
            });
        }
    }

    public class EmfDescriptionBuffer : EmfFilePart
    {
        public EmfField Contents = new(typeof(string)); // Variable Length

        public EmfDescriptionBuffer()
        {
            Fields.AddRange(new[]
            {
               Contents
            });
        }
    }

    public class EmfPixelFormatBuffer : EmfFilePart
    {
        public EmfField Contents = new(typeof(string)); // Variable Length

        public EmfPixelFormatBuffer()
        {
            Fields.AddRange(new[]
            {
               Contents
            });
        }
    }

    public class EmfTextRecord : EmfFilePart
    {
        public EmfField Bounds = new(16);
        public EmfField iGraphicsMode = new(4);
        public EmfField exScale = new(4);
        public EmfField eyScale = new(4);

        public EmfField Reference = new(8);
        public EmfField Unknown = new(4);
        public EmfField Chars = new(4, typeof(uint));
        public EmfField offString = new(4, typeof(uint));
        public EmfField Options = new(4, typeof(uint));
        public EmfField Rectangle = new(16);
        public EmfField offDx = new(4, typeof(uint));
        public EmfField OutputString = new(typeof(string));
        public EmfField OutputDx = new();

        public EmfTextRecord()
        {
            Fields.AddRange(new[]
            {
               Bounds, iGraphicsMode, exScale, eyScale, Reference, Unknown, Chars, offString, Options, Rectangle, offDx
            });
        }
    }
}
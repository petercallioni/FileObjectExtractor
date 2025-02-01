using MicrosoftObjectExtractor.Extensions;
using MicrosoftObjectExtractor.Models.Converters;
using MicrosoftObjectExtractor.Models.EMF.Enums;
using System;
using System.Text;

namespace MicrosoftObjectExtractor.Models.EMF
{
    public class EmfParser
    {
        public EmfParser() { }

        public EmfFile Parse(byte[] data)
        {
            EmfFile file = new EmfFile();
            StringBuilder baseString = new StringBuilder(Convert.ToHexString(data));

            file.EmfFileHeader.Initialize(baseString);
            file.EmfHeader.Initialize(baseString);
            int headerSize = 80;

            if (file.EmfFileHeader.Size.Value >= 88)
            {
                file.EmfHeaderExtension1.Initialize(baseString);
                headerSize += 12;
            }

            if (file.EmfFileHeader.Size.Value >= 100)
            {
                file.EmfHeaderExtension2.Initialize(baseString);
                headerSize += 8;
            }

            if (file.EmfHeader.nDescription.Value != 0 || file.EmfHeader.offDescription.Value != 0)
            {
                baseString.Remove(0, file.EmfHeader.offDescription.Value - headerSize); // Skip to description
                file.EmfDescriptionBuffer.Contents.ByteLength = file.EmfHeader.nDescription.Value;
                file.EmfDescriptionBuffer.Initialize(baseString);
            }

            if (file.EmfHeaderExtension1.cbPixelFormat.Value != 0 || file.EmfHeaderExtension1.offPixelFormat.Value != 0)
            {
                baseString.Remove(0, file.EmfHeaderExtension1.offPixelFormat.Value); // Skip to Value
                file.EmfPixelFormatBuffer.Contents.ByteLength = file.EmfHeaderExtension1.cbPixelFormat.Value;
                file.EmfPixelFormatBuffer.Initialize(baseString);
            }

            while (SkipToTextRecord(baseString))
            {
                EmfTextRecord emfTextRecord = new EmfTextRecord();
                emfTextRecord.Initialize(baseString); // Only partial initialization
                emfTextRecord.OutputString.ByteLength = (int)emfTextRecord.Chars.Value * 2; // Two characters to a byte
                emfTextRecord.OutputString.Intitialize(baseString);

                file.EmfTextRecords.Add(emfTextRecord);
            }

            return file;
        }

        public bool SkipToTextRecord(StringBuilder input)
        {
            while (input.Length > 0)
            {
                int shiftAmount = 4 * 2;

                if (input.Length - shiftAmount < 0)
                {
                    input.Clear();
                    return false;
                }

                int value = HexConverter.LittleEndianHexToInt(input.Shift(0, shiftAmount));
                RecordType recordType = (RecordType)value;
                if (recordType == RecordType.EMR_EXTTEXTOUTW || recordType == RecordType.EMR_EXTTEXTOUTA)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

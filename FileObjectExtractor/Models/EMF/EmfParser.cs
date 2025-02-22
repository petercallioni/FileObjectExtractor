using FileObjectExtractor.Models.Converters;
using FileObjectExtractor.Models.EMF.EmfPart;
using FileObjectExtractor.Models.EMF.Enums;
using System;
using System.Text;

namespace FileObjectExtractor.Models.EMF
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
                int charsToSkip = (int)emfTextRecord.Chars.Value;

                emfTextRecord.OutputString.ByteLength = (int)emfTextRecord.Chars.Value * 2; // Two characters to a byte
                emfTextRecord.OutputString.Initialize(baseString);

                file.EmfTextRecords.Add(emfTextRecord);
            }

            return file;
        }

        public bool SkipToTextRecord(StringBuilder input)
        {
            const int ShiftAmount = 8; // 4 bytes represented by 8 hex characters
            const int Step = 2 * 2; // Advance two characters (one byte) at a time

            while (input.Length >= ShiftAmount)
            {
                int value = HexConverter.LittleEndianHexToInt(input.ToString().Substring(0, ShiftAmount));
                RecordType recordType = (RecordType)value;
                if (recordType == RecordType.EMR_EXTTEXTOUTW || recordType == RecordType.EMR_EXTTEXTOUTA)
                {
                    input.Remove(0, ShiftAmount);
                    return true;
                }

                // Remove the first two characters (one byte) and continue
                input.Remove(0, Step);
            }

            input.Clear();
            return false;
        }
    }
}

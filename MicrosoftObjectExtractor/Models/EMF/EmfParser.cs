using MicrosoftObjectExtractor.Extensions;
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

            while (SkipToRecord(baseString))
            {
                EmfTextRecord emfTextRecord = new EmfTextRecord();
                emfTextRecord.Initialize(baseString); // Only partial initialization
                emfTextRecord.OutputString.ByteLength = (int)emfTextRecord.Chars.Value * 2; // Two characters to a byte
                emfTextRecord.OutputString.Intitialize(baseString);

                file.EmfTextRecords.Add(emfTextRecord);
            }

            return file;
        }

        public bool SkipToRecord(StringBuilder input)
        {
            while (input.Length > 0)
            {
                int shiftAmount = 4 * 2;

                if (input.Length - shiftAmount < 0)
                {
                    input.Clear();
                    return false;
                }

                int value = LittleEndianHexToInt(input.Shift(0, shiftAmount));
                RecordType recordType = (RecordType)value;
                if (recordType == RecordType.EMR_EXTTEXTOUTW)
                {
                    return true;
                }
            }
            return false;
        }

        private int LittleEndianHexToInt(string hexString)
        {
            // Convert hex string to byte array (little-endian)
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            // Convert little-endian byte array to integer
            int intValue = BitConverter.ToInt32(bytes, 0);
            return intValue;
        }

        //private string ExtractTextFromEmf(byte[] emfBytes)
        //{
        //    StringBuilder returnString = new StringBuilder();
        //    string returnText = string.Empty;

        //    string baseString = Convert.ToHexString(emfBytes);

        //    // Need to find the records section first and change how it is split
        //    List<StringBuilder> parts = baseString.Split("54000000")
        //        .Select(x => new StringBuilder(x))
        //        .ToList();

        //    if (parts.Count > 0)
        //    {
        //        parts.RemoveAt(0); // Discard the first part which is useless

        //        foreach (StringBuilder part in parts)
        //        {
        //            part.Remove(0, 16 * 2); // Remove Bounds
        //            part.Remove(0, 4 * 2); // Remove iGraphicsMode 
        //            part.Remove(0, 4 * 2); // Remove exScale 
        //            part.Remove(0, 4 * 2); // Remove eyScale 
        //            part.Remove(0, 8 * 2); // Remove Reference
        //            part.Remove(0, 4 * 2); // ????
        //            int charCount = LittleEndianHexToInt(part.Shift(0, 8)); //Amount of characters
        //            int offset = LittleEndianHexToInt(part.Shift(0, 8)); //Bytes offset from start

        //            returnText = part.Shift(offset - 28, charCount * 4); // Number of bytes from Bounds to eyScale, then characters are four bytes per character
        //            returnString.Append(HexToString(returnText));
        //        }
        //    }

        //    return returnString.ToString();
        //}
    }
}

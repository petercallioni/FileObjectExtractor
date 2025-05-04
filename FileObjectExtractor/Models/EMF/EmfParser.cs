using FileObjectExtractor.Converters;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.EMF.EmfPart;
using FileObjectExtractor.Models.EMF.Enums;
using System.Collections.Generic;

namespace FileObjectExtractor.Models.EMF
{
    public class EmfParser
    {
        public EmfParser() { }

        public EmfFile Parse(byte[] data)
        {
            EmfFile file = new EmfFile();
            Queue<byte> dataQueue = new Queue<byte>(data);

            file.EmfFileHeader.Initialize(dataQueue);
            file.EmfHeader.Initialize(dataQueue);
            int headerSize = 80;

            if (file.EmfFileHeader.Size.Value >= 88)
            {
                file.EmfHeaderExtension1.Initialize(dataQueue);
                headerSize += 12;
            }

            if (file.EmfFileHeader.Size.Value >= 100)
            {
                file.EmfHeaderExtension2.Initialize(dataQueue);
                headerSize += 8;
            }

            if (file.EmfHeader.nDescription.Value != 0 || file.EmfHeader.offDescription.Value != 0)
            {
                dataQueue.DequeueMultiple((int)file.EmfHeader.offDescription.Value - headerSize); // Skip to description
                file.EmfDescriptionBuffer.Contents.ByteLength = (int)file.EmfHeader.nDescription.Value;
                file.EmfDescriptionBuffer.Initialize(dataQueue);
            }

            if (file.EmfHeaderExtension1.cbPixelFormat.Value != 0 || file.EmfHeaderExtension1.offPixelFormat.Value != 0)
            {
                dataQueue.DequeueMultiple((int)file.EmfHeaderExtension1.offPixelFormat.Value); // Skip to Value;
                file.EmfPixelFormatBuffer.Contents.ByteLength = (int)file.EmfHeaderExtension1.cbPixelFormat.Value;
                file.EmfPixelFormatBuffer.Initialize(dataQueue);
            }

            while (SkipToTextRecord(dataQueue))
            {
                EmfTextRecord emfTextRecord = new EmfTextRecord();
                emfTextRecord.Initialize(dataQueue); // Only partial initialization
                int charsToSkip = (int)emfTextRecord.Chars.Value;

                emfTextRecord.OutputString.ByteLength = (int)emfTextRecord.Chars.Value * 2; // Two characters to a byte

                // It was a valid text record, so add it to the list
                if (emfTextRecord.OutputString.Initialize(dataQueue))
                {
                    file.EmfTextRecords.Add(emfTextRecord);
                }
            }

            return file;
        }

        public bool SkipToTextRecord(Queue<byte> input)
        {
            const int ShiftAmount = 4; // 4 bytes

            Queue<byte> buffer = new Queue<byte>(ShiftAmount);

            while (input.Count >= ShiftAmount)
            {
                while (buffer.Count < ShiftAmount)
                {
                    buffer.Enqueue(input.Dequeue());
                }

                int value = HexConverter.LittleEndianHexToInt(buffer.ToArray());
                RecordType recordType = (RecordType)value;

                if (recordType == RecordType.EMR_EXTTEXTOUTW || recordType == RecordType.EMR_EXTTEXTOUTA)
                {
                    return true;
                }

                // Remove one byte and continue
                buffer.Dequeue();
            }

            input.Clear();
            return false;
        }
    }
}

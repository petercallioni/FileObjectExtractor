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

            // Parse the main headers and update headerSize appropriately.
            int headerSize = ParseHeaders(file, dataQueue);

            // If a description is present, jump to it and initialize the description buffer.
            ParseDescription(file, dataQueue, headerSize);

            // Likewise for the pixel format section.
            ParsePixelFormat(file, dataQueue);

            // Finally, process all text records.
            ParseTextRecords(file, dataQueue);

            return file;
        }

        private int ParseHeaders(EmfFile file, Queue<byte> dataQueue)
        {
            file.EmfFileHeader.Initialize(dataQueue);
            file.EmfHeader.Initialize(dataQueue);

            // Base header size.
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

            return headerSize;
        }

        private void ParseDescription(EmfFile file, Queue<byte> dataQueue, int headerSize)
        {
            if (file.EmfHeader.nDescription.Value != 0 ||
                file.EmfHeader.offDescription.Value != 0)
            {
                // Skip bytes until reaching the description based on the header offset.
                int skipBytes = (int)file.EmfHeader.offDescription.Value - headerSize;
                dataQueue.DequeueMultiple(skipBytes);

                // Set the length and initialize the description contents.
                file.EmfDescriptionBuffer.Contents.ByteLength = (int)file.EmfHeader.nDescription.Value;
                file.EmfDescriptionBuffer.Initialize(dataQueue);
            }
        }

        private void ParsePixelFormat(EmfFile file, Queue<byte> dataQueue)
        {
            if (file.EmfHeaderExtension1.cbPixelFormat.Value != 0 ||
                file.EmfHeaderExtension1.offPixelFormat.Value != 0)
            {
                // Jump to the pixel format data.
                dataQueue.DequeueMultiple((int)file.EmfHeaderExtension1.offPixelFormat.Value);
                file.EmfPixelFormatBuffer.Contents.ByteLength = (int)file.EmfHeaderExtension1.cbPixelFormat.Value;
                file.EmfPixelFormatBuffer.Initialize(dataQueue);
            }
        }

        private void ParseTextRecords(EmfFile file, Queue<byte> dataQueue)
        {
            while (SkipToTextRecord(dataQueue, out RecordType? recordType))
            {
                // Extract the record size (after reading the record type which is already determined).
                uint recordSize = HexConverter.LittleEndianHexToUInt(dataQueue.DequeueMultiple(4));
                int recordBufferSize = (int)recordSize - 8; // Account for RecordType and Size fields.

                // The math is easier if shift the whole record to a new buffer.
                Queue<byte> recordBuffer = new Queue<byte>(dataQueue.DequeueMultiple(recordBufferSize));

                // Create and partially initialize a new text record.
                EmfTextRecord textRecord = new EmfTextRecord(recordType!.Value, recordSize);
                textRecord.Initialize(recordBuffer);

                // Determine if there is a Rectangle field based on Options.
                if (textRecord.Options.Value != 0x00000100)
                {
                    textRecord.HasRectangle = true;
                }

                if (textRecord.HasRectangle)
                {
                    textRecord.Rectangle.Initialize(recordBuffer);
                }

                // Process the offDx field.
                textRecord.offDx.Initialize(recordBuffer);

                // Process text string if present.
                if ((int)textRecord.offString.Value > 0)
                {
                    int emptyBytes = (int)recordSize - recordBuffer.Count - (int)textRecord.offString.Value;
                    recordBuffer.DequeueMultiple(emptyBytes); // Skip to the start of the string.

                    textRecord.OutputString.ByteLength = (int)textRecord.Chars.Value * textRecord.CharSizeModifer;

                    // If the string is properly initialized, add the text record.
                    if (textRecord.OutputString.Initialize(recordBuffer))
                    {
                        file.EmfTextRecords.Add(textRecord);
                    }
                }

                // The rest of the record buffer is for the OutputDx field which is not needed.
            }
        }

        public bool SkipToTextRecord(Queue<byte> input, out RecordType? recordType)
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
                recordType = (RecordType)value;

                if (recordType == RecordType.EMR_EXTTEXTOUTW || recordType == RecordType.EMR_EXTTEXTOUTA)
                {
                    return true;
                }

                // Remove one byte and continue
                buffer.Dequeue();
            }

            recordType = null;
            input.Clear();
            return false;
        }
    }
}

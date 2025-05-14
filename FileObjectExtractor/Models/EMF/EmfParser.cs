using FileObjectExtractor.Converters;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.EMF.EmfPart;
using FileObjectExtractor.Models.EMF.Enums;
using System;
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
            while (SkipToTextRecord(dataQueue, out RecordType recordType, out uint recordSize))
            {
                uint recordBufferSize = recordSize - 8u; // Account for RecordType and Size fields.

                // The math is easier if shift the whole record to a new buffer.
                Queue<byte> recordBuffer = new Queue<byte>(dataQueue.DequeueMultiple((int)recordBufferSize));

                // Create and partially initialize a new text record.
                EmfTextRecord textRecord = new EmfTextRecord(recordType, (uint)recordBufferSize);
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

        /// <summary>
        /// Skip to the next text record in the input queue.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="recordType"></param>
        /// <param name="recordSize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool SkipToTextRecord(Queue<byte> input, out RecordType recordType, out uint recordSize)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Constants for the header bytes
            const int recordTypeByteCount = 4;   // Bytes for RecordType field
            const int recordSizeByteCount = 4;   // Bytes for RecordSize field
            const int recordHeaderSize = recordTypeByteCount + recordSizeByteCount;

            // Initialize output parameters
            recordType = RecordType.EMR_HEADER;
            recordSize = 0;

            // We must have at least header bytes to safely read a record header
            while (input.Count >= recordHeaderSize)
            {
                // Dequeue and convert record type
                byte[] typeBytes = input.DequeueMultiple(recordTypeByteCount);
                recordType = (RecordType)HexConverter.LittleEndianHexToInt(typeBytes);

                // Dequeue and convert record size
                byte[] sizeBytes = input.DequeueMultiple(recordSizeByteCount);
                recordSize = HexConverter.LittleEndianHexToUInt(sizeBytes);

                // Check for file termination marker
                if (recordType == RecordType.EMR_EOF)
                {
                    input.Clear(); // clear any remaining bytes. We do not need the optional palettes.
                    break;
                }

                // If the record is one of the text record types, return immediately
                if (recordType == RecordType.EMR_EXTTEXTOUTW || recordType == RecordType.EMR_EXTTEXTOUTA)
                {
                    return true;
                }

                // For other record types, skip the body of the record.
                // Calculate remaining number of bytes to skip.
                int remainingRecordBytes = (int)recordSize - recordHeaderSize;
                if (remainingRecordBytes > 0)
                {
                    // Ensure that we don't attempt to skip more bytes than available.
                    // This might indicate a malformed record.
                    int bytesToSkip = Math.Min(remainingRecordBytes, input.Count);
                    input.DequeueMultiple(bytesToSkip);
                }
            }

            return false;
        }
    }
}

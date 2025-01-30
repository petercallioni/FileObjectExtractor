using MicrosoftObjectExtractor.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Tesseract;

namespace MicrosoftObjectExtractor.Models
{
    public class ExtractedFile
    {
        public readonly byte[] IconFile;
        public readonly byte[] ArchivedFile;
        public string FileName;

        public ExtractedFile(ZipArchiveEntry iconFileEntry, ZipArchiveEntry archivedFileEntry)
        {
            IconFile = ExtractToMemory(iconFileEntry);
            ArchivedFile = ExtractToMemory(archivedFileEntry);
            FileName = "TBA";
        }

        private byte[] ExtractToMemory(ZipArchiveEntry entry)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Stream entryStream = entry.Open())
                {
                    entryStream.CopyTo(memoryStream);
                }
                return memoryStream.ToArray();
            }
        }

        private string ExtractNameFromFile(byte[] file)
        {
            byte[] pngBytes = ConvertEmfToPngByteArray(file);

            using (TesseractEngine engine = new TesseractEngine("tessdata", "eng", EngineMode.Default))
            using (Pix img = Pix.LoadFromMemory(pngBytes))
            {
                using (Page page = engine.Process(img))
                {
                    string text = page.GetText();
                    return text;
                }
            }
        }

        private string ExtractTextFromEmf(byte[] emfBytes)
        {
            StringBuilder returnString = new StringBuilder();
            string returnText = string.Empty;

            string baseString = Convert.ToHexString(emfBytes);

            // Need to find the records section first and change how it is split
            List<StringBuilder> parts = baseString.Split("54000000")
                .Select(x => new StringBuilder(x))
                .ToList();

            if (parts.Count > 0)
            {
                parts.RemoveAt(0); // Discard the first part which is useless

                foreach (StringBuilder part in parts)
                {
                    part.Remove(0, 16 * 2); // Remove Bounds
                    part.Remove(0, 4 * 2); // Remove iGraphicsMode 
                    part.Remove(0, 4 * 2); // Remove exScale 
                    part.Remove(0, 4 * 2); // Remove eyScale 
                    part.Remove(0, 8 * 2); // Remove Reference
                    part.Remove(0, 4 * 2); // ????
                    int charCount = LittleEndianHexToInt(part.Shift(0, 8)); //Amount of characters
                    int offset = LittleEndianHexToInt(part.Shift(0, 8)); //Bytes offset from start

                    returnText = part.Shift(offset - 28, charCount * 4); // Number of bytes from Bounds to eyScale, then characters are four bytes per character
                    returnString.Append(HexToString(returnText));
                }
            }

            return returnString.ToString();
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

        private byte[] ConvertEmfToPngByteArray(byte[] emfBytes)
        {
            using (MemoryStream inputStream = new MemoryStream(emfBytes))
            using (Metafile image = new Metafile(inputStream))
            using (MemoryStream outputStream = new MemoryStream())
            {
                // Upscale the image to improve OCR accuracy
                using (Bitmap upscaledImage = new Bitmap(image.Width * 4, image.Height * 4))
                {
                    using (Graphics graphics = Graphics.FromImage(upscaledImage))
                    {
                        graphics.DrawImage(image, 0, 0, upscaledImage.Width, upscaledImage.Height);
                    }

                    // Crop the top half of the upscaled image
                    int croppedHeight = upscaledImage.Height / 2;
                    using (Bitmap croppedImage = new Bitmap(upscaledImage.Width, croppedHeight))
                    {
                        using (Graphics graphics = Graphics.FromImage(croppedImage))
                        {
                            graphics.DrawImage(upscaledImage, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), new Rectangle(0, croppedHeight, upscaledImage.Width, croppedHeight), GraphicsUnit.Pixel);
                        }

                        croppedImage.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                return outputStream.ToArray();
            }
        }

        private string FixFileName(string fileName)
        {
            StringBuilder newFileName = new StringBuilder();

            return fileName.Replace("\n", "");
        }
    }
}
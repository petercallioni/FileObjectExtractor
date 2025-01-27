using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text;
using Tesseract;

namespace MicrosoftObjectExtractor.Models
{
    public class ExtractedFiles
    {
        public readonly byte[] IconFile;
        public readonly byte[] ArchivedFile;
        public string FileName;

        public ExtractedFiles(ZipArchiveEntry iconFileEntry, ZipArchiveEntry archivedFileEntry)
        {
            IconFile = ExtractToMemory(iconFileEntry);
            ArchivedFile = ExtractToMemory(archivedFileEntry);
            FileName = FixFileName(ExtractNameFromFile(IconFile));
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
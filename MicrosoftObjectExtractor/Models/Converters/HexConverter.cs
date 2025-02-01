using System;
using System.Text;

namespace MicrosoftObjectExtractor.Models.Converters
{
    public static class HexConverter
    {
        private static byte[] HexStringToByteArray(string hexString, bool littleEndian = true)
        {
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        public static int LittleEndianHexToInt(string hexString)
        {
            byte[] bytes = HexStringToByteArray(hexString);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint LittleEndianHexToUInt(string hexString)
        {
            byte[] bytes = HexStringToByteArray(hexString);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static float LittleEndianHexToFloat(string hexString)
        {
            byte[] bytes = HexStringToByteArray(hexString);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static string HexToString(string hexString)
        {
            if (hexString.Length % 4 != 0)
            {
                throw new ArgumentException("Invalid hex string length.");
            }

            byte[] bytes = HexStringToByteArray(hexString, false);
            return Encoding.Unicode.GetString(bytes);
        }
    }
}

using System;
using System.Text;

namespace FileObjectExtractor.Converters
{
    public static class HexConverter
    {
        public static byte[] HexStringToByteArray(string hexString, bool littleEndian = true)
        {
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        public static int LittleEndianHexToInt(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }

        public static uint LittleEndianHexToUInt(byte[] data)
        {
            return BitConverter.ToUInt32(data, 0);
        }

        public static float LittleEndianHexToFloat(byte[] data)
        {
            return BitConverter.ToSingle(data, 0);
        }

        public static string HexToString(byte[] data, Encoding encoding)
        {
            return encoding.GetString(data);
        }
    }
}

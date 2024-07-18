using System;

namespace ServiceViewer.Models;

public static class BigEndianBitConverter
{
    public static int ToInt32(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(int)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(int));
        return BitConverter.ToInt32(EnsureBigEndian(bytes), 0);
    }

    public static uint ToUInt32(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(int)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(int));
        return BitConverter.ToUInt32(EnsureBigEndian(bytes), 0);
    }

    public static short ToInt16(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(short)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(short));
        return BitConverter.ToInt16(EnsureBigEndian(bytes), 0);
    }

    public static ushort ToUInt16(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(ushort)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(ushort));
        return BitConverter.ToUInt16(EnsureBigEndian(bytes), 0);
    }

    public static long ToInt64(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(long)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(long));
        return BitConverter.ToInt64(EnsureBigEndian(bytes), 0);
    }

    public static float ToSingle(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(float)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(float));
        return BitConverter.ToSingle(EnsureBigEndian(bytes), 0);
    }

    public static double ToDouble(byte[] data, int startIndex = 0)
    {
        byte[] bytes = new byte[sizeof(double)];
        Array.Copy(data, startIndex, bytes, 0, sizeof(double));
        return BitConverter.ToDouble(EnsureBigEndian(bytes), 0);
    }

    private static byte[] EnsureBigEndian(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }
}
namespace JustCause.FileFormats.Utilities;

using System;

public static class ByteArrayExtensions
{
	public static void Set(this byte[] array, long index, byte[] value, bool reverse = false)
	{
		if (reverse)
		{
			Array.Reverse(value);
		}

		value.CopyTo(array, index);
	}

	public static void Set(this byte[] array, long index, bool value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, char value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, double value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, short value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, int value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, long value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, float value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, ushort value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, uint value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());

	public static void Set(this byte[] array, long index, ulong value, Endian endian = default)
		=> Set(array, index, BitConverter.GetBytes(value), !endian.IsPlatformEndian());
}

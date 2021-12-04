namespace JustCause.FileFormats.Utilities;

using System;

public static class NumericExtensions
{
	private static byte[] Reverse(this byte[] array)
	{
		Array.Reverse(array);
		return array;
	}

	public static char Swap(this char value)
		=> BitConverter.ToChar(BitConverter.GetBytes(value).Reverse());

	public static double Swap(this double value)
		=> BitConverter.ToDouble(BitConverter.GetBytes(value).Reverse());

	public static short Swap(this short value)
		=> BitConverter.ToInt16(BitConverter.GetBytes(value).Reverse());

	public static int Swap(this int value)
		=> BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse());

	public static long Swap(this long value)
		=> BitConverter.ToInt64(BitConverter.GetBytes(value).Reverse());

	public static float Swap(this float value)
		=> BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse());

	public static ushort Swap(this ushort value)
		=> BitConverter.ToUInt16(BitConverter.GetBytes(value).Reverse());

	public static uint Swap(this uint value)
		=> BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse());

	public static ulong Swap(this ulong value)
		=> BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse());
}

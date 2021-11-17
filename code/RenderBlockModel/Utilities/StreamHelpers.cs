namespace JustCause.RenderBlockModel.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum Endian
{
	Little,
	Big
}

public static class StreamHelpers
{
	private static T ChangeType<T, V>(V value)
	{
		return (T)Convert.ChangeType(value, typeof(T));
	}

	public static void ReadArray<T>(this BinaryReader reader, T[] array, Endian endian) where T : struct
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		for (int i = 0; i < array.Length; ++i)
		{
			array[i] = reader.Read<T>(endian);
		}
	}

	public static void ReadBinaryFormatArray<T>(this BinaryReader reader, T[] array, Endian endian) where T : IBinaryFormat
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		for (int i = 0; i < array.Length; ++i)
		{
			array[i].Deserialize(reader, endian);
		}
	}

	public static void ReadList<T>(this BinaryReader reader, List<T> array, Endian endian) where T : struct
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		array.Clear();
		array.Capacity = reader.ReadInt32(endian);

		for (int i = 0; i < array.Capacity; ++i)
		{
			array.Add(reader.Read<T>(endian));
		}
	}

	public static void ReadBinaryFormatList<T>(this BinaryReader reader, List<T> list, Endian endian) where T : IBinaryFormat, new()
	{
		if (list == null)
		{
			throw new ArgumentNullException(nameof(list));
		}

		list.Clear();
		list.Capacity = reader.ReadInt32(endian);

		for (int i = 0; i < list.Capacity; ++i)
		{
			T item = new();
			item.Deserialize(reader, endian);
			list.Add(item);
		}
	}

	public static T Read<T>(this BinaryReader reader, Endian endian) where T : struct
	{
		byte[] bytes = null;
		Type returnType = typeof(T);

		if (returnType == typeof(byte) || returnType == typeof(sbyte) || returnType == typeof(bool))
		{
			return ChangeType<T, byte>(reader.ReadByte());
		}
		else if (returnType == typeof(short) || returnType == typeof(ushort) || returnType == typeof(char))
		{
			bytes = reader.ReadBytes(sizeof(short));
		}
		else if (returnType == typeof(int) || returnType == typeof(uint) || returnType == typeof(float))
		{
			bytes = reader.ReadBytes(sizeof(int));
		}
		else if (returnType == typeof(long) || returnType == typeof(ulong) || returnType == typeof(double))
		{
			bytes = reader.ReadBytes(sizeof(long));
		}

		if (bytes != null)
		{
			if (BitConverter.IsLittleEndian != (endian == Endian.Little))
			{
				Array.Reverse(bytes);
			}

			if (returnType == typeof(short))
			{
				return ChangeType<T, short>(BitConverter.ToInt16(bytes));
			}
			else if (returnType == typeof(ushort))
			{
				return ChangeType<T, ushort>(BitConverter.ToUInt16(bytes));
			}
			else if (returnType == typeof(int))
			{
				return ChangeType<T, int>(BitConverter.ToInt32(bytes));
			}
			else if (returnType == typeof(uint))
			{
				return ChangeType<T, uint>(BitConverter.ToUInt32(bytes));
			}
			else if (returnType == typeof(long))
			{
				return ChangeType<T, long>(BitConverter.ToInt64(bytes));
			}
			else if (returnType == typeof(ulong))
			{
				return ChangeType<T, ulong>(BitConverter.ToUInt64(bytes));
			}
			else if (returnType == typeof(char))
			{
				return ChangeType<T, char>(BitConverter.ToChar(bytes));
			}
			else if (returnType == typeof(float))
			{
				return ChangeType<T, float>(BitConverter.ToSingle(bytes));
			}
			else if (returnType == typeof(double))
			{
				return ChangeType<T, double>(BitConverter.ToDouble(bytes));
			}
		}

		return default;
	}

	public static short ReadInt16(this BinaryReader reader, Endian endian)
	{
		return reader.Read<short>(endian);
	}

	public static ushort ReadUInt16(this BinaryReader reader, Endian endian)
	{
		return reader.Read<ushort>(endian);
	}

	public static int ReadInt32(this BinaryReader reader, Endian endian)
	{
		return reader.Read<int>(endian);
	}

	public static uint ReadUInt32(this BinaryReader reader, Endian endian)
	{
		return reader.Read<uint>(endian);
	}

	public static long ReadInt64(this BinaryReader reader, Endian endian)
	{
		return reader.Read<long>(endian);
	}

	public static ulong ReadUInt64(this BinaryReader reader, Endian endian)
	{
		return reader.Read<ulong>(endian);
	}

	public static char ReadChar(this BinaryReader reader, Endian endian)
	{
		return reader.Read<char>(endian);
	}

	public static float ReadSingle(this BinaryReader reader, Endian endian)
	{
		return reader.Read<float>(endian);
	}

	public static double ReadDouble(this BinaryReader reader, Endian endian)
	{
		return reader.Read<double>(endian);
	}

	public static string ReadString(this BinaryReader reader, Encoding encoding, Endian endian)
	{
		byte[] bytes = reader.ReadBytes(reader.ReadInt32(endian));
		return encoding.GetString(bytes);
	}

	public static string ReadString(this BinaryReader reader, int length, Encoding encoding)
	{
		byte[] bytes = reader.ReadBytes(length);
		return encoding.GetString(bytes);
	}
}

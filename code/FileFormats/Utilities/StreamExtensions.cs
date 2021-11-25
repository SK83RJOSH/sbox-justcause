namespace JustCause.FileFormats.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum Endian
{
	Little,
	Big
}

public static class StreamExtensions
{
	private static T ChangeType<T, V>(V value)
	{
		Type type = typeof(T);

		if (type.IsEnum)
		{
			type = Enum.GetUnderlyingType(type);
		}

		return (T)Convert.ChangeType(value, type);
	}

	public static bool IsEOS(this BinaryReader reader)
	{
		if (reader == null)
		{
			return true;
		}

		if (reader.BaseStream == null)
		{
			return true;
		}

		return reader.BaseStream.Position >= reader.BaseStream.Length;
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

	public static int Read<T>(this BinaryReader reader, out T[] value, uint count, Endian endian) where T : struct
	{
		int bytes_read = 0;
		value = new T[count];

		for (uint i = 0; i < count; ++i)
		{
			bytes_read += reader.Read(out value[i], endian);
		}

		return bytes_read;
	}

	public static int Read(this BinaryReader reader, out string value, int length, Encoding encoding)
	{
		byte[] bytes = reader.ReadBytes(length);
		value = encoding.GetString(bytes);
		return bytes.Length;
	}

	public static int Read(this BinaryReader reader, out string value, int length)
	{
		return reader.Read(out value, length, Encoding.ASCII);
	}

	public static int Read(this BinaryReader reader, out string value, Encoding encoding, Endian endian)
	{
		int read = reader.Read(out int length, endian);
		return read + reader.Read(out value, length, encoding);
	}

	public static int Read(this BinaryReader reader, out string value, Endian endian)
	{
		return reader.Read(out value, Encoding.ASCII, endian);
	}

	public static T[] Read<T>(this BinaryReader reader, uint count, Endian endian) where T : struct
	{
		reader.Read(out T[] value, count, endian);
		return value;
	}

	public static int Read<T>(this BinaryReader reader, out T value, Endian endian) where T : struct
	{
		byte[] bytes = null;
		Type returnType = typeof(T);

		if (returnType.IsEnum)
		{
			returnType = returnType.GetEnumUnderlyingType();
		}

		if (returnType == typeof(byte) || returnType == typeof(sbyte) || returnType == typeof(bool))
		{
			value = ChangeType<T, byte>(reader.ReadByte());
			return 1;
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

		if (bytes == null)
		{
			value = default;
			return 0;
		}

		if (BitConverter.IsLittleEndian != (endian == Endian.Little))
		{
			Array.Reverse(bytes);
		}

		if (returnType == typeof(short))
		{
			value = ChangeType<T, short>(BitConverter.ToInt16(bytes));
		}
		else if (returnType == typeof(ushort))
		{
			value = ChangeType<T, ushort>(BitConverter.ToUInt16(bytes));
		}
		else if (returnType == typeof(int))
		{
			value = ChangeType<T, int>(BitConverter.ToInt32(bytes));
		}
		else if (returnType == typeof(uint))
		{
			value = ChangeType<T, uint>(BitConverter.ToUInt32(bytes));
		}
		else if (returnType == typeof(long))
		{
			value = ChangeType<T, long>(BitConverter.ToInt64(bytes));
		}
		else if (returnType == typeof(ulong))
		{
			value = ChangeType<T, ulong>(BitConverter.ToUInt64(bytes));
		}
		else if (returnType == typeof(char))
		{
			value = ChangeType<T, char>(BitConverter.ToChar(bytes));
		}
		else if (returnType == typeof(float))
		{
			value = ChangeType<T, float>(BitConverter.ToSingle(bytes));
		}
		else if (returnType == typeof(double))
		{
			value = ChangeType<T, double>(BitConverter.ToDouble(bytes));
		}
		else
		{
			value = default;
		}

		return bytes.Length;
	}

	public static T Read<T>(this BinaryReader reader, Endian endian) where T : struct
	{
		Read(reader, out T value, endian);
		return value;
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
		reader.Read(out string value, encoding, endian);
		return value;
	}

	public static string ReadString(this BinaryReader reader, int length, Encoding encoding)
	{
		reader.Read(out string value, length, encoding);
		return value;
	}

	public static string ReadString(this BinaryReader reader, Endian endian)
	{
		reader.Read(out string value, endian);
		return value;
	}

	public static string ReadString(this BinaryReader reader, int length)
	{
		reader.Read(out string value, length);
		return value;
	}
}

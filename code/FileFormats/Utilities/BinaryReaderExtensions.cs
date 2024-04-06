namespace JustCause.FileFormats.Utilities;

using System;
using System.IO;
using System.Text;

public enum Endian
{
	Default,
	Little,
	Big
}

public static class EndianExtensions
{
	public static bool IsPlatformEndian(this Endian endian)
	{
		return endian == Endian.Default || endian == (BitConverter.IsLittleEndian ? Endian.Little : Endian.Big);
	}
}

public static class BoolExtensions
{
	public static Endian AsEndian(this bool boolean)
	{
		if (boolean)
		{
			return (BitConverter.IsLittleEndian ? Endian.Big : Endian.Little);
		}

		return Endian.Default;
	}
}

public static class BinaryReaderExtensions
{
	public static long Tell(this BinaryReader reader)
		=> reader.BaseStream.Position;

	public static void Seek(this BinaryReader reader, long offset, SeekOrigin origin = SeekOrigin.Begin)
		=> reader.BaseStream.Seek(offset, origin);

	public static void Move(this BinaryReader reader, long offset)
		=> reader.Seek(offset, SeekOrigin.Current);

	public static bool ReachedPosition(this BinaryReader reader, long position)
		=> reader.Tell() >= position;

	public static bool AtEnd(this BinaryReader reader)
		=> reader.ReachedPosition(reader.BaseStream.Length);

	public static bool Read(this BinaryReader reader, out string value, Endian endian = default)
		=> reader.Read(out value, Encoding.ASCII, endian);

	public static bool Read<T>(this BinaryReader reader, out T value, Endian endian = default)
		where T : new()
		=> Read(reader, out value, !endian.IsPlatformEndian());

	public static bool Read<T>(this BinaryReader reader, out T[] value, int count, Endian endian = default)
		where T : new()
		=> Read(reader, out value, count, !endian.IsPlatformEndian());

	public static bool Read<T>(this BinaryReader reader, out T[] value, Endian endian = default)
		where T : new()
		=> Read(reader, out value, !endian.IsPlatformEndian());

	public static bool Read(this BinaryReader reader, out string value, int length)
		=> reader.Read(out value, length, Encoding.ASCII);

	public static bool Read(this BinaryReader reader, out string value, int length, Encoding encoding)
	{
		bool null_terminated = length < 0;

		if (null_terminated)
		{
			length = 0;

			while (reader.Read(out byte character))
			{
				++length;

				if (character == 0)
				{
					break;
				}
			}

			reader.Seek(-length, SeekOrigin.Current);
		}

		byte[] bytes = reader.ReadBytes(length);

		if (bytes == null || bytes.Length != length)
		{
			value = default;
			return false;
		}

		value = encoding.GetString(bytes);

		if (null_terminated)
		{
			value = value.TrimEnd('\0');
		}

		return true;
	}

	public static bool Read(this BinaryReader reader, out string value, Encoding encoding, Endian endian = default)
	{
		if (!reader.Read(out int length, endian) || length < 0)
		{
			value = default;
			return false;
		}

		return reader.Read(out value, length, encoding);
	}

	private static T ChangeType<T, V>(V value, Type type)
	{
		return (T)Convert.ChangeType(value, type);
	}

	private static int GetEnumSize(Type type)
	{
		return true switch
		{
			true when type == typeof(byte) || type == typeof(sbyte) => sizeof(byte),
			true when type == typeof(short) || type == typeof(ushort) => sizeof(short),
			true when type == typeof(int) || type == typeof(uint) => sizeof(int),
			true when type == typeof(long) || type == typeof(ulong) => sizeof(long),
			_ => default
		};
	}

	private static bool ReadPrimitive<T>(this BinaryReader reader, out T value, bool reverse)
	{
		value = default;

		int type_size = value switch
		{
			byte or sbyte or bool => sizeof(byte),
			short or ushort or char => sizeof(short),
			int or uint or float => sizeof(int),
			long or ulong or double => sizeof(long),
			Enum => GetEnumSize(typeof(T).GetEnumUnderlyingType()),
			_ => default
		};

		if (type_size == 0)
		{
			return false;
		}

		byte[] bytes = reader.ReadBytes(type_size);

		if (bytes == null || bytes.Length != type_size)
		{
			return false;
		}

		if (reverse)
		{
			Array.Reverse(bytes);
		}

		Type type = typeof(T);

		if (type.IsEnum)
		{
			type = type.GetEnumUnderlyingType();
		}

		value = true switch
		{
			true when type == typeof(byte) => ChangeType<T, byte>(bytes[0], type),
			true when type == typeof(sbyte) => ChangeType<T, sbyte>(unchecked((sbyte)bytes[0]), type),
			true when type == typeof(bool) => ChangeType<T, bool>(BitConverter.ToBoolean(bytes, 0), type),
			true when type == typeof(short) => ChangeType<T, short>(BitConverter.ToInt16(bytes), type),
			true when type == typeof(ushort) => ChangeType<T, ushort>(BitConverter.ToUInt16(bytes), type),
			true when type == typeof(char) => ChangeType<T, char>((char)BitConverter.ToUInt16(bytes, 0), type),
			true when type == typeof(int) => ChangeType<T, int>(BitConverter.ToInt32(bytes), type),
			true when type == typeof(uint) => ChangeType<T, uint>(BitConverter.ToUInt32(bytes), type),
			true when type == typeof(float) => ChangeType<T, float>(BitConverter.ToSingle(bytes), type),
			true when type == typeof(long) => ChangeType<T, long>(BitConverter.ToInt64(bytes), type),
			true when type == typeof(ulong) => ChangeType<T, ulong>(BitConverter.ToUInt64(bytes), type),
			true when type == typeof(double) => ChangeType<T, double>(BitConverter.ToDouble(bytes), type),
			_ => default
		};

		return true;
	}

	private static bool ReadBinary<T>(this BinaryReader reader, out T value, bool reverse)
		where T : new()
	{
		value = new();
		return value is IBinaryReader binary_reader && binary_reader.Read(reader, reverse.AsEndian());
	}

	private static bool Read<T>(this BinaryReader reader, out T value, bool reverse)
		where T : new()
	{
		Type type = typeof(T);

		if (type.IsPrimitive || type.IsEnum)
		{
			return reader.ReadPrimitive(out value, reverse);
		}
		else if (typeof(IBinaryReader).IsAssignableFrom(type))
		{
			return reader.ReadBinary(out value, reverse);
		}

		value = default;
		return false;
	}

	private static bool Read<T>(this BinaryReader reader, out T[] value, int count, bool reverse)
		where T : new()
	{
		value = new T[count];

		for (uint i = 0; i < count; ++i)
		{
			if (!reader.Read(out value[i], reverse))
			{
				return false;
			}
		}

		return true;
	}

	private static bool Read<T>(this BinaryReader reader, out T[] value, bool reverse)
		where T : new()
	{
		if (!reader.Read(out int count, reverse))
		{
			value = default;
			return false;
		}

		return reader.Read(out value, count, reverse);
	}
}

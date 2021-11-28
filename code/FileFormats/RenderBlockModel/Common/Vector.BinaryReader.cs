namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public partial struct Vector2<T>
{
	static public bool Read(BinaryReader reader, out Vector2<T> vector, Endian endian = default)
	{
		if (!reader.Read(out T[] values, 2, endian) || values == null || values.Length != 2)
		{
			vector = default;
			return false;
		}

		vector = new(values[0], values[1]);
		return true;
	}
}

public partial struct Vector3<T>
{
	public static bool Read(BinaryReader reader, out Vector3<T> vector, Endian endian = default)
	{
		if (!reader.Read(out T[] values, 3, endian) || values == null || values.Length != 3)
		{
			vector = default;
			return false;
		}

		vector = new(values[0], values[1], values[2]);
		return true;
	}
}

public partial struct Vector4<T>
{
	public static bool Read(BinaryReader reader, out Vector4<T> vector, Endian endian = default)
	{
		if (!reader.Read(out T[] values, 4, endian) || values == null || values.Length != 4)
		{
			vector = default;
			return false;
		}

		vector = new(values[0], values[1], values[2], values[3]);
		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read<T>(this BinaryReader reader, out Vector2<T> vector, Endian endian = default)
		where T : unmanaged, IComparable
	{
		return Vector2<T>.Read(reader, out vector, endian);
	}

	public static bool Read<T>(this BinaryReader reader, out Vector3<T> vector, Endian endian = default)
		where T : unmanaged, IComparable
	{
		return Vector3<T>.Read(reader, out vector, endian);
	}

	public static bool Read<T>(this BinaryReader reader, out Vector4<T> vector, Endian endian = default)
		where T : unmanaged, IComparable
	{
		return Vector4<T>.Read(reader, out vector, endian);
	}
	public static bool Read<T>(this BinaryReader reader, out Vector2<T>[] vectors, int count, Endian endian = default)
		where T : unmanaged, IComparable
	{
		vectors = new Vector2<T>[count];

		for (int i = 0; i < count; ++i)
		{
			if (!Vector2<T>.Read(reader, out vectors[i], endian))
			{
				return false;
			}
		}

		return true;
	}

	public static bool Read<T>(this BinaryReader reader, out Vector3<T>[] vectors, int count, Endian endian = default)
		where T : unmanaged, IComparable
	{
		vectors = new Vector3<T>[count];

		for (int i = 0; i < count; ++i)
		{
			if (!Vector3<T>.Read(reader, out vectors[i], endian))
			{
				return false;
			}
		}

		return true;
	}

	public static bool Read<T>(this BinaryReader reader, out Vector4<T>[] vectors, int count, Endian endian = default)
		where T : unmanaged, IComparable
	{
		vectors = new Vector4<T>[count];

		for (int i = 0; i < count; ++i)
		{
			if (!Vector4<T>.Read(reader, out vectors[i], endian))
			{
				return false;
			}
		}

		return true;
	}
}

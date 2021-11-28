namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public partial struct PackedVector3<T>
{
	public static bool Read(BinaryReader reader, out PackedVector3<T> vector, Endian endian)
	{
		if (!reader.Read(out float packed, endian))
		{
			vector = default;
			return false;
		}

		vector = new(new T().Unpack(packed), MathF.Sign(packed));
		return true;
	}
}

public partial struct PackedVector4<T>
{
	public static bool Read(BinaryReader reader, out PackedVector4<T> vector, Endian endian)
	{
		if (!reader.Read(out float packed, endian))
		{
			vector = default;
			return false;
		}

		vector = new(new T().Unpack(packed), MathF.Sign(packed));
		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read<T>(this BinaryReader reader, out PackedVector3<T> vector, Endian endian = default) where T : PackingModel<Vector3<float>>, new()
	{
		return PackedVector3<T>.Read(reader, out vector, endian);
	}

	public static bool Read<T>(this BinaryReader reader, out PackedVector4<T> vector, Endian endian = default) where T : PackingModel<Vector4<float>>, new()
	{
		return PackedVector4<T>.Read(reader, out vector, endian);
	}
}

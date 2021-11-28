namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using Vector2f = Vector2<float>;
using Vector4b = Vector4<byte>;

public enum VertexFormat : uint
{
	Float32,
	Int16
}

public struct VertexInfo
{
	public VertexFormat Format;
	public float Scale;
	public Vector2f[] UVExtents;
	public float ColorExtent;
	public Vector4b Color;

	public static bool Read(BinaryReader reader, out VertexInfo info, bool packed_extents, Endian endian = default)
	{
		info = default;

		if (!reader.Read(out info.Format, endian))
		{
			return false;
		}

		if (!reader.Read(out info.Scale, endian))
		{
			return false;
		}

		if (packed_extents)
		{
			if (!reader.Read(out float[] extents, 2, endian))
			{
				return false;
			}

			info.UVExtents = new Vector2f[] { new(extents[0]), new(extents[1]) };
		}
		else if (!reader.Read(out info.UVExtents, 2, endian))
		{
			return false;
		}

		if (!reader.Read(out info.ColorExtent, endian))
		{
			return false;
		}

		if (!reader.Read(out info.Color, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out VertexInfo vertex, bool packed_extents, Endian endian = default)
	{
		return VertexInfo.Read(reader, out vertex, packed_extents, endian);
	}
}


namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using PackedNormal = DataTypes.PackedVector3<DataTypes.NormalPackingModel>;
using Vector2f = DataTypes.Vector2<float>;
using Vector3f = DataTypes.Vector3<float>;

public struct CarPaintSimpleVertex
{
	public Vector3f Position;
	public PackedNormal Normal;
	public Vector2f UV;
	public PackedNormal Tangent;
	public PackedNormal Binormal;

	public static bool Read(BinaryReader reader, out CarPaintSimpleVertex vertex, Endian endian = default)
	{
		vertex = default;

		if (!reader.Read(out vertex.Position, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.Normal, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.UV, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.Tangent, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.Binormal, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out CarPaintSimpleVertex vertex, Endian endian = default)
	{
		return CarPaintSimpleVertex.Read(reader, out vertex, endian);
	}

	public static bool Read(this BinaryReader reader, out CarPaintSimpleVertex[] vertices, Endian endian = default)
	{
		if (!reader.Read(out int count, endian))
		{
			vertices = default;
			return false;
		}

		vertices = new CarPaintSimpleVertex[count];

		for (int i = 0; i < count; ++i)
		{
			if (!reader.Read(out vertices[i], endian))
			{
				return false;
			}
		}

		return true;
	}
}

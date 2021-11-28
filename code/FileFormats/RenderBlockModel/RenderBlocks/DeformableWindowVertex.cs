namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using PackedNormal = DataTypes.PackedVector3<DataTypes.NormalPackingModel>;
using Vector2f = DataTypes.Vector2<float>;
using Vector4f = DataTypes.Vector4<float>;
using Vector4s = DataTypes.Vector4<short>;

public struct DeformableWindowVertex
{
	public Vector4f Position;
	public Vector4s DeformWeights;
	public PackedNormal Normal;
	public PackedNormal DeformedNormal;
	public PackedNormal Tangent;
	public PackedNormal DeformedTangent;
	public Vector2f UV;

	public static bool Read(BinaryReader reader, out DeformableWindowVertex vertex, Endian endian = default)
	{
		vertex = new();

		if (!reader.Read(out vertex.Position, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.DeformWeights, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.Normal, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.DeformedNormal, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.Tangent, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.DeformedTangent, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.UV, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out DeformableWindowVertex vertex, Endian endian = default)
	{
		return DeformableWindowVertex.Read(reader, out vertex, endian);
	}

	public static bool Read(this BinaryReader reader, out DeformableWindowVertex[] vertices, Endian endian = default)
	{
		if (!reader.Read(out int count, endian))
		{
			vertices = default;
			return false;
		}

		vertices = new DeformableWindowVertex[count];

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

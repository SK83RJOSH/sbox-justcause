namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using Vector2f = DataTypes.Vector2<float>;
using Vector3f = DataTypes.Vector3<float>;
using PackedNormal = DataTypes.PackedVector3<DataTypes.NormalPackingModel>;
using PackedColor = DataTypes.PackedVector3<DataTypes.ColorPackingModelRGB>;

public struct GeneralVertex
{
	public Vector3f Position;
	public Vector2f[] UVs;
	public PackedNormal Normal;
	public PackedNormal Tangent;
	public PackedColor Color;

	public static bool Read(BinaryReader reader, out GeneralVertex vertex, VertexFormat format, Endian endian = default)
	{
		vertex = default;

		if (format == VertexFormat.Float32)
		{
			if (!reader.Read(out vertex.Position, endian))
			{
				return false;
			}

			if (!reader.Read(out vertex.UVs, 2, endian))
			{
				return false;
			}

			if (!reader.Read(out vertex.Normal, endian))
			{
				return false;
			}

			if (!reader.Read(out vertex.Tangent, endian))
			{
				return false;
			}

			if (!reader.Read(out vertex.Color, endian))
			{
				return false;
			}
		}
		else
		{
			if (!reader.Read(out Vector2<short>[] uvs, 2, endian))
			{
				return false;
			}

			vertex.UVs = new Vector2f[] {
				new(uvs[0].X / 32767f, uvs[0].Y / 32767f),
				new(uvs[1].X / 32767f, uvs[1].Y / 32767f),
			};

			if (!reader.Read(out vertex.Normal, endian))
			{
				return false;
			}

			if (!reader.Read(out vertex.Tangent, endian))
			{
				return false;
			}

			if (!reader.Read(out vertex.Color, endian))
			{
				return false;
			}

			if (!reader.Read(out Vector3<short> position, endian))
			{
				return false;
			}

			vertex.Position = new Vector3f(position.X / 32767f, position.Y / 32767f, position.Z / 32767f);
			reader.Skip(2);
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out GeneralVertex vertex, VertexFormat format, Endian endian = default)
	{
		return GeneralVertex.Read(reader, out vertex, format, endian);
	}

	public static bool Read(this BinaryReader reader, out GeneralVertex[] vertices, VertexFormat format, Endian endian = default)
	{
		if (!reader.Read(out int count, endian))
		{
			vertices = default;
			return false;
		}

		vertices = new GeneralVertex[count];

		for (int i = 0; i < count; ++i)
		{
			if (!reader.Read(out vertices[i], format, endian))
			{
				return false;
			}
		}

		return true;
	}
}

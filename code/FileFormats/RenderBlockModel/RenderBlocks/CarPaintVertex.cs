namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public struct CarPaintVertex
{
	public Vector4<float> Position;
	public Vector4<short> DeformWeights;

	public static bool Read(BinaryReader reader, out CarPaintVertex vertex, Endian endian = default)
	{
		vertex = default;

		if (!reader.Read(out vertex.Position, endian))
		{
			return false;
		}

		if (!reader.Read(out vertex.DeformWeights, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out CarPaintVertex vertex, Endian endian = default)
	{
		return CarPaintVertex.Read(reader, out vertex, endian);
	}

	public static bool Read(this BinaryReader reader, out CarPaintVertex[] vertices, Endian endian = default)
	{
		if (!reader.Read(out int count, endian))
		{
			vertices = default;
			return false;
		}

		vertices = new CarPaintVertex[count];

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

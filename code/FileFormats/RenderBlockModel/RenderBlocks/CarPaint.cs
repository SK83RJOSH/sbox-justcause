namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public class CarPaint : IRenderBlock
{
	public CarPaintAttributes Attributes;
	public Material Material;
	public CarPaintCombinedVertex[] Vertices;
	public short[] Indices;
	public DeformTable DeformTable;

	public static bool Read(BinaryReader reader, out CarPaint block, Endian endian = default)
	{
		block = new();

		if (!reader.Read(out byte version, endian))
		{
			return false;
		}

		if (version < 3 || version > 4) // TODO: V3 used combined normal/vertice data
		{
			// unhandled CarPaint version
			return false;
		}

		if (!reader.Read(out block.Attributes, endian))
		{
			return false;
		}

		if (version >= 4)
		{
			if (!reader.Read(out block.DeformTable, endian))
			{
				return false;
			}
		}

		if (!reader.Read(out block.Material, endian))
		{
			return false;
		}

		if (version >= 3)
		{
			if (!reader.Read(out CarPaintVertex[] vertices, endian))
			{
				return false;
			}

			if (!reader.Read(out CarPaintNormal[] normals, endian))
			{
				return false;
			}

			if (vertices.Length != normals.Length)
			{
				// CarPaint vertex count did not equal normal count!
				return false;
			}

			block.Vertices = new CarPaintCombinedVertex[vertices.Length];

			for (int i = 0; i < vertices.Length; ++i)
			{
				block.Vertices[i].Position = vertices[i].Position;
				block.Vertices[i].DeformWeights = vertices[i].DeformWeights;
				block.Vertices[i].UV = normals[i].UV;
				block.Vertices[i].Light = normals[i].Light;
				block.Vertices[i].Normal = normals[i].Normal;
				block.Vertices[i].DeformedNormal = normals[i].DeformedNormal;
				block.Vertices[i].Tangent = normals[i].Tangent;
				block.Vertices[i].DeformedTangent = normals[i].DeformedTangent;
			}
		}

		if (!reader.Read(out block.Indices, endian))
		{
			return false;
		}

		if (version <= 3)
		{
			if (!reader.Read(out block.DeformTable, endian))
			{
				return false;
			}
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out CarPaint block, Endian endian = default)
	{
		return CarPaint.Read(reader, out block, endian);
	}
}

namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

[Sandbox.Library]
public struct CarPaint : IRenderBlock
{
	public byte Version;
	public CarPaintAttributes Attributes;
	public Material Material;
	public List<CarPaintCombinedVertex> Vertices = new();
	public List<short> Indices = new();
	public DeformTable DeformTable;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Version = reader.ReadByte();

		if (Version < 3 || Version > 4) // Version < 3 used combined normal/vertice data
		{
			throw new FormatException("unhandled CarPaint version");
		}

		Attributes.Deserialize(reader, endian);

		if (Version >= 4)
		{
			DeformTable.Deserialize(reader, endian);
		}

		Material.Deserialize(reader, endian);

		if (Version >= 3)
		{
			List<CarPaintVertex> vertices = new();
			reader.ReadBinaryFormatList(vertices, endian);
			List<CarPaintNormal> normals = new();
			reader.ReadBinaryFormatList(normals, endian);

			if (vertices.Count != normals.Count)
			{
				throw new FormatException("CarPaint vertex count did not equal normal count!");
			}

			Vertices.Capacity = vertices.Count;

			for (int i = 0; i < vertices.Count; ++i)
			{
				CarPaintCombinedVertex vertex = default;
				vertex.Position = vertices[i].Position;
				vertex.DeformWeights = vertices[i].DeformWeights;
				vertex.UVL = normals[i].UVL;
				vertex.Normal = normals[i].Normal;
				vertex.DeformedNormal = normals[i].DeformedNormal;
				vertex.Tangent = normals[i].Tangent;
				vertex.DeformedTangent = normals[i].DeformedTangent;
				Vertices.Add(vertex);
			}
		}

		reader.ReadList(Indices, endian);

		if (Version <= 3)
		{
			DeformTable.Deserialize(reader, endian);
		}
	}
}

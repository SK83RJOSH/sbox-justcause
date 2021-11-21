namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

[Sandbox.Library]
public struct General : IRenderBlock
{
	public byte Version;
	public GeneralAttributes Attributes;
	public Material Material;
	public List<GeneralVertex> Vertices = new();
	public List<short> Indices = new();

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Version = reader.ReadByte();

		if (Version < 2 || Version > 3)
		{
			throw new FormatException("unhandled General version");
		}

		Attributes.Deserialize(reader, endian, Version);
		Material.Deserialize(reader, endian);

		Vertices.Clear();
		Vertices.Capacity = reader.ReadInt32();

		for (int i = 0; i < Vertices.Capacity; ++i)
		{
			GeneralVertex vertex = new();
			vertex.Deserialize(reader, endian, Attributes.VertexInfo.Format);
			Vertices.Add(vertex);
		}

		reader.ReadList(Indices, endian);
	}
}

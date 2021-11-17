namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

[Sandbox.Library]
public struct DeformableWindow : IRenderBlock
{
	public byte Version;
	public Material Material;
	public DeformableWindowAttributes Attributes;
	public DeformTable DeformTable;
	public List<DeformableWindowVertex> Vertices = new();
	public List<short> Indices = new();

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Version = reader.ReadByte();

		if (Version > 2)
		{
			throw new FormatException("unhandled DeformableWindow version");
		}

		Material.Deserialize(reader, endian);

		if (Version > 1)
		{
			Attributes.Deserialize(reader, endian);
			DeformTable.Deserialize(reader, endian);
		}

		reader.ReadBinaryFormatList(Vertices, endian);
		reader.ReadList(Indices, endian);

		if (Version < 2)
		{
			DeformTable.Deserialize(reader, endian);
		}

		if (Version == 1)
		{
			Attributes.Deserialize(reader, endian);
		}
	}
}

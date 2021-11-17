namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

[Sandbox.Library]
public struct CarPaintSimple : IRenderBlock
{
	public byte Version;
	public CarPaintAttributes Attributes;
	public Material Material;
	public List<CarPaintSimpleVertex> Vertices = new();
	public List<short> Indices = new();
	public DeformTable DeformTable;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Version = reader.ReadByte();

		if (Version != 1)
		{
			throw new FormatException("unhandled CarPaintSimple version");
		}

		Attributes.Deserialize(reader, endian);
		Material.Deserialize(reader, endian);
		reader.ReadBinaryFormatList(Vertices, endian);
		reader.ReadList(Indices, endian);
	}
}

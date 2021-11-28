namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public class General : IRenderBlock
{
	public GeneralAttributes Attributes;
	public Material Material;
	public GeneralVertex[] Vertices;
	public short[] Indices;

	public static bool Read(BinaryReader reader, out General block, Endian endian)
	{
		block = new();

		if (!reader.Read(out byte version, endian) || version < 2 || version > 3)
		{
			throw new FormatException("unhandled General version");
		}

		if (!reader.Read(out block.Attributes, version, endian) || !reader.Read(out block.Material, endian))
		{
			return false;
		}

		VertexFormat format = block.Attributes.VertexInfo.Format;

		if (!reader.Read(out block.Vertices, format, endian) || !reader.Read(out block.Indices, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out General block, Endian endian = default)
	{
		return General.Read(reader, out block, endian);
	}
}

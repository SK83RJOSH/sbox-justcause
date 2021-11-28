namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public class Lambert : IRenderBlock
{
	public LambertAttributes Attributes;
	public Material Material;
	public GeneralVertex[] Vertices;
	public short[] Indices;

	public static bool Read(BinaryReader reader, out Lambert block, Endian endian)
	{
		block = new();

		if (!reader.Read(out byte version) || version == 1 || version > 4)
		{
			// unhandled version
			return false;
		}

		if (!reader.Read(out block.Attributes, version, endian))
		{
			return false;
		}

		if (!reader.Read(out block.Material, endian))
		{
			return false;
		}

		VertexFormat format = block.Attributes.VertexInfo.Format;

		if (!reader.Read(out block.Vertices, format, endian))
		{
			return false;
		}

		if (!reader.Read(out block.Indices, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out Lambert block, Endian endian = default)
	{
		return Lambert.Read(reader, out block, endian);
	}
}

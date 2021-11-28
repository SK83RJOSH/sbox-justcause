namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public class DeformableWindow : IRenderBlock
{
	public Material Material;
	public DeformableWindowAttributes Attributes;
	public DeformTable DeformTable;
	public DeformableWindowVertex[] Vertices;
	public short[] Indices;

	public static bool Read(BinaryReader reader, out DeformableWindow block, Endian endian = default)
	{
		block = new();

		if (!reader.Read(out byte version, endian) || version > 2)
		{
			// unhandled DeformableWindow version
			return false;
		}

		if (!reader.Read(out block.Material))
		{
			return false;
		}

		if (version > 1 && (!reader.Read(out block.Attributes, endian) || reader.Read(out block.DeformTable, endian)))
		{
			return false;
		}

		if (!reader.Read(out block.Vertices, endian) || !reader.Read(out block.Indices, endian))
		{
			return false;
		}

		if (version < 2 && !reader.Read(out block.DeformTable, endian))
		{
			return false;
		}

		if (version == 1 && !reader.Read(out block.Attributes, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out DeformableWindow block, Endian endian = default)
	{
		return DeformableWindow.Read(reader, out block, endian);
	}
}

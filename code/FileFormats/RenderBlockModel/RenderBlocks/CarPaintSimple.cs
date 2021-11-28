namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public class CarPaintSimple : IRenderBlock
{
	public CarPaintAttributes Attributes;
	public Material Material;
	public CarPaintSimpleVertex[] Vertices;
	public short[] Indices;
	public DeformTable DeformTable;

	public static bool Read(BinaryReader reader, out CarPaintSimple block, Endian endian = default)
	{
		block = new();

		if (!reader.Read(out byte version, endian))
		{
			return false;
		}

		if (version != 1)
		{
			// unhandled CarPaintSimple version
			return false;
		}

		if (!reader.Read(out block.Attributes, endian))
		{
			return false;
		}

		if (!reader.Read(out block.Material, endian))
		{
			return false;
		}

		if (!reader.Read(out block.Vertices, endian))
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
	public static bool Read(this BinaryReader reader, out CarPaintSimple block, Endian endian = default)
	{
		return CarPaintSimple.Read(reader, out block, endian);
	}
}

namespace JustCause.FileFormats.RenderBlockModel;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System;
using System.IO;
using Vector3i = DataTypes.Vector3<int>;
using Vector3f = DataTypes.Vector3<float>;

public class RenderBlockModel
{
	public Vector3i Version;
	public Vector3f Min;
	public Vector3f Max;
	public IRenderBlock[] Blocks;

	public static bool Read(BinaryReader reader, out RenderBlockModel model)
	{
		model = new();

		if (!reader.Read(out uint magic_length) && magic_length != 5 && magic_length != 83886080)
		{
			// invalid magic length
			return false;
		}

		Endian endian = magic_length == 5 ? Endian.Little : Endian.Big;

		if (!reader.Read(out string magic, 5) && magic != "RBMDL")
		{
			// invalid magic
			return false;
		}

		if (!reader.Read(out model.Version, endian) || model.Version.X != 1 || model.Version.Y != 13)
		{
			// unsupported RBMDL version
			return false;
		}

		if (!reader.Read(out model.Min, endian))
		{
			return false;
		}

		if (!reader.Read(out model.Max, endian))
		{
			return false;
		}

		if (!reader.Read(out int block_count, endian))
		{
			throw new FormatException("unable to read block count!");
		}

		model.Blocks = new IRenderBlock[block_count];

		for (int i = 0; i < block_count; i++)
		{
			BlockTypeFactory.Read(reader, out model.Blocks[i], endian);
		}

		return true;
	}
}

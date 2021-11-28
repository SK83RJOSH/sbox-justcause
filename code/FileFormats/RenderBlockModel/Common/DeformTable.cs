namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System.IO;

public struct DeformTable
{
	public uint[] Data;

	public static bool Read(BinaryReader reader, out DeformTable table, Endian endian = default)
	{
		table = default;

		if (!reader.Read(out table.Data, 256, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out DeformTable table, Endian endian = default)
	{
		return DeformTable.Read(reader, out table, endian);
	}
}

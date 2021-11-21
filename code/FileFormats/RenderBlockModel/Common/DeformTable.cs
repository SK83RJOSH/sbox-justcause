namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System.IO;

public struct DeformTable : IBinaryFormat
{
	public uint[] Data;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Data = new uint[256];
		reader.ReadArray(Data, endian);
	}
}

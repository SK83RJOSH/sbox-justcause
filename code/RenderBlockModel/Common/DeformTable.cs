namespace JustCause.RenderBlockModel.Common;

using JustCause.RenderBlockModel.Utilities;
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

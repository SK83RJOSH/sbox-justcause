namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Utilities;
using System.IO;

[System.Flags]
public enum DeformableWindowFlags : uint
{

	DarkWindow = 1 << 0,
}

public struct DeformableWindowAttributes
{
	public DeformableWindowFlags Flags;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Flags = (DeformableWindowFlags)reader.ReadUInt32(endian);
	}
}

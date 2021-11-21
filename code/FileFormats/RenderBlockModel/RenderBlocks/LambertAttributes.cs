namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

[System.Flags]
public enum LambertFlags : uint
{
	AlphaBlending = 1 << 0,
	AlphaTest = 1 << 1,
	TwoSided = 1 << 2,
	FlatNormal = 1 << 3,
	NoDirt = 1 << 4,
	UseSnow = 1 << 5,
	UseDynamicLights = 1 << 6,
	UseChannelTextures = 1 << 7,
	UseChannelAmbientOcclusion = 1 << 8,
}

public struct LambertAttributes
{
	public VertexInfo VertexInfo;
	public LambertFlags Flags;
	public float DepthBias;
	public byte TextureChannel;
	public byte AmbientOcclusionChannel;

	public void Deserialize(BinaryReader reader, Endian endian, byte Version)
	{
		if (Version == 4)
		{
			VertexInfo.Deserialize(reader, endian);
		}

		Flags = (LambertFlags)reader.ReadUInt32(endian);
		DepthBias = reader.ReadSingle(endian);

		if (Version == 3)
		{
			VertexInfo.Deserialize(reader, endian);
		}

		if (Version == 0)
		{
			Flags &= LambertFlags.UseDynamicLights;
		}

		if (Version == 4)
		{
			TextureChannel = reader.ReadByte();
			AmbientOcclusionChannel = reader.ReadByte();
			reader.ReadBytes(2); // skip two bytes because this definitely disaligns us
		}
	}
}

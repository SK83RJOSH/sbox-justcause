namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

[System.Flags]
public enum GeneralFlags : uint
{

	NoCulling = 1 << 0,
	AlphaBlending = 1 << 1,
	AdditiveAlpha = 1 << 2,
	UsePalette = 1 << 3,
	UseSubSurface = 1 << 4,
	UseChannelTextures = 1 << 5,
	UseSnow = 1 << 6,
	AnimateTexture = 1 << 7,
	AlphaTest = 1 << 8,
	UseAmbientOcclusion = 1 << 9,
	NoDepthTest = 1 << 10
}

public struct GeneralAttributes
{
	public Vector4<float> ChannelMask;
	public Vector4<float> ChannelAmbientOcclusionMask;
	public float DepthBias;
	public float Specularity;
	public VertexInfo VertexInfo;
	public GeneralFlags Flags;

	public void Deserialize(BinaryReader reader, Endian endian, byte Version)
	{
		ChannelMask.Deserialize(reader, endian);
		ChannelAmbientOcclusionMask.Deserialize(reader, endian);
		DepthBias = reader.ReadSingle(endian);
		Specularity = reader.ReadSingle(endian);
		VertexInfo.Deserialize(reader, endian, Version < 3);
		Flags = (GeneralFlags)reader.ReadUInt32(endian);
	}
}

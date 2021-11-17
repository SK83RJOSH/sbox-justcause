namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System.IO;

public enum VertexFormat : uint
{
	Float32,
	Int16
}

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
	public VertexFormat VertexFormat;
	public float Scale;
	public Vector2<float>[] UVExtents;
	public float ColorExtent;
	public Vector4<byte> Color;
	public GeneralFlags Flags;

	public void Deserialize(BinaryReader reader, Endian endian, byte Version)
	{
		ChannelMask.Deserialize(reader, endian);
		ChannelAmbientOcclusionMask.Deserialize(reader, endian);
		DepthBias = reader.ReadSingle(endian);
		Specularity = reader.ReadSingle(endian);
		VertexFormat = (VertexFormat)reader.ReadUInt32(endian);
		Scale = reader.ReadSingle(endian);
		UVExtents = new Vector2<float>[2];

		if (Version != 3)
		{
			UVExtents[0] = new Vector2<float>(reader.ReadSingle(endian));
			UVExtents[1] = new Vector2<float>(reader.ReadSingle(endian));
		}
		else
		{
			reader.ReadBinaryFormatArray(UVExtents, endian);
		}

		ColorExtent = reader.ReadSingle(endian);
		Color.Deserialize(reader, endian);
		Flags = (GeneralFlags)reader.ReadUInt32(endian);
	}
}

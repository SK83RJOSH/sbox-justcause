namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using Vector4f = DataTypes.Vector4<float>;

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
	public Vector4f ChannelMask;
	public Vector4f ChannelAmbientOcclusionMask;
	public float DepthBias;
	public float Specularity;
	public VertexInfo VertexInfo;
	public GeneralFlags Flags;

	public static bool Read(BinaryReader reader, out GeneralAttributes attributes, byte version, Endian endian = default)
	{
		attributes = default;

		if (!reader.Read(out attributes.ChannelMask, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.ChannelAmbientOcclusionMask, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.DepthBias, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.Specularity, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.VertexInfo, version < 3, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.Flags, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out GeneralAttributes attributes, byte version, Endian endian = default)
	{
		return GeneralAttributes.Read(reader, out attributes, version, endian);
	}
}


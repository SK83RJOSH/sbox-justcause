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

	public static bool Read(BinaryReader reader, out LambertAttributes attributes, byte version, Endian endian)
	{
		attributes = default;

		if (version == 4 && !reader.Read(out attributes.VertexInfo, false, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.Flags, endian))
		{
			return false;
		}

		if (version == 0)
		{
			attributes.Flags &= LambertFlags.UseDynamicLights;
		}

		if (!reader.Read(out attributes.DepthBias, endian))
		{
			return false;
		}

		if (version == 3 && !reader.Read(out attributes.VertexInfo, false, endian))
		{
			return false;
		}

		if (version == 4)
		{
			if (!reader.Read(out attributes.TextureChannel, endian))
			{
				return false;
			}

			if (!reader.Read(out attributes.AmbientOcclusionChannel, endian))
			{
				return false;
			}

			reader.Move(2);
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out LambertAttributes attributes, byte version, Endian endian = default)
	{
		return LambertAttributes.Read(reader, out attributes, version, endian);
	}
}

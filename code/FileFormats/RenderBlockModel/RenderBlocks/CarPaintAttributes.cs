namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

[System.Flags]
public enum CarPaintFlags : uint
{
	NoCulling = 1 << 0,
	AlphaBlending = 1 << 1,
	IgnorePalette = 1 << 3,
	NoDirt = 1 << 4,
	Decal = 1 << 5,
	MaskWater = 1 << 6,
	AlphaTest = 1 << 7,
	Dull = 1 << 8,
	IsLight = 1 << 9,
	FlatNormal = 1 << 10
}

public struct CarPaintAttributes
{
	public Vector3<float>[] TwoToneColors;
	public float Specularity;
	public float DepthBias;
	public float Reflectivity;
	public Vector4<float> Noise;
	public CarPaintFlags Flags;

	public static bool Read(BinaryReader reader, out CarPaintAttributes attributes, Endian endian = default)
	{
		attributes = default;

		if (!reader.Read(out attributes.TwoToneColors, 2, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.Specularity, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.DepthBias, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.Reflectivity, endian))
		{
			return false;
		}

		if (!reader.Read(out attributes.Noise, endian))
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
	public static bool Read(this BinaryReader reader, out CarPaintAttributes attributes, Endian endian = default)
	{
		return CarPaintAttributes.Read(reader, out attributes, endian);
	}
}

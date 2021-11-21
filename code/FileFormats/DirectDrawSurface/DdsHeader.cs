namespace JustCause.FileFormats.DirectDrawSurface;

using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public enum DdsCompressionAlgorithm : uint
{
	None = 0,
	D3DFMT_DXT1 = 827611204,
	D3DFMT_DXT2 = 844388420,
	D3DFMT_DXT3 = 861165636,
	D3DFMT_DXT4 = 877942852,
	D3DFMT_DXT5 = 894720068,
	DX10 = 808540228,
	ATI1 = 826889281,
	BC4U = 1429488450,
	BC4S = 1395934018,
	ATI2 = 843666497,
	BC5U = 1429553986,
	BC5S = 1395999554
}

[Flags]
public enum DdsFlags : uint
{
	Caps = 0x1,
	Height = 0x2,
	Width = 0x4,
	Pitch = 0x8,
	PixelFormat = 0x1000,
	MipMapCount = 0x20000,
	LinearSize = 0x80000,
	Depth = 0x800000
}

[Flags]
public enum DdsPixelFormatFlags : uint
{
	AlphaPixels = 0x1,
	Alpha = 0x2,
	Fourcc = 0x4,
	Rgb = 0x40,
	Yuv = 0x200,
	Luminance = 0x20000
}

public struct DdsPixelFormat : IBinaryFormat
{
	public uint Size;
	public DdsPixelFormatFlags PixelFormatFlags;
	public DdsCompressionAlgorithm FourCC;
	public uint RGBBitCount;
	public uint RBitMask;
	public uint GBitMask;
	public uint BBitMask;
	public uint ABitMask;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Size = reader.ReadUInt32(endian);

		if (Size != 32)
		{
			throw new Exception($"unexpected DDS pixel size, should be 32 not: ${Size}");
		}

		PixelFormatFlags = (DdsPixelFormatFlags)reader.ReadUInt32(endian);
		FourCC = (DdsCompressionAlgorithm)reader.ReadUInt32(endian);
		RGBBitCount = reader.ReadUInt32(endian);
		RBitMask = reader.ReadUInt32(endian);
		GBitMask = reader.ReadUInt32(endian);
		BBitMask = reader.ReadUInt32(endian);
		ABitMask = reader.ReadUInt32(endian);
	}
}

public class DdsHeader : IBinaryFormat
{
	public const int HEADER_SIZE = 124;
	public const uint DDS_MAGIC = 542327876;

	public uint Size;
	public DdsFlags Flags;
	public uint Height;
	public uint Width;
	public uint PitchOrLinearSize;
	public uint Depth;
	public uint MipMapCount;
	public uint[] Reserved1;
	public DdsPixelFormat PixelFormat;
	public uint Caps;
	public uint Caps2;
	public uint Caps3;
	public uint Caps4;
	public uint Reserved2;

	public DdsHeader(BinaryReader reader)
	{
		Deserialize(reader, Endian.Little);
	}

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		uint magic = reader.ReadUInt32(endian);

		if (magic != DDS_MAGIC)
		{
			throw new Exception("invalid DDS magic");
		}

		Size = reader.ReadUInt32(endian);

		if (Size != HEADER_SIZE)
		{
			throw new Exception("invalid DDS header");
		}

		Flags = (DdsFlags)reader.ReadUInt32(endian);
		Height = reader.ReadUInt32(endian);
		Width = reader.ReadUInt32(endian);
		PitchOrLinearSize = reader.ReadUInt32(endian);
		Depth = reader.ReadUInt32(endian);
		MipMapCount = reader.ReadUInt32(endian);
		Reserved1 = new uint[11];
		reader.ReadArray(Reserved1, endian);
		PixelFormat.Deserialize(reader, endian);
		Caps = reader.ReadUInt32(endian);
		Caps2 = reader.ReadUInt32(endian);
		Caps3 = reader.ReadUInt32(endian);
		Caps4 = reader.ReadUInt32(endian);
		Reserved2 = reader.ReadUInt32(endian);
	}
}

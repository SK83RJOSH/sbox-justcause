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

public struct DdsPixelFormat
{
	public uint Size;
	public DdsPixelFormatFlags PixelFormatFlags;
	public DdsCompressionAlgorithm FourCC;
	public uint RGBBitCount;
	public uint RBitMask;
	public uint GBitMask;
	public uint BBitMask;
	public uint ABitMask;

	public static bool Read(BinaryReader reader, out DdsPixelFormat format, Endian endian = default)
	{
		format = default;

		if (!reader.Read(out format.Size, endian) || format.Size != 32)
		{
			// unexpected DDS pixel size, should be 32
			return false;
		}

		if (!reader.Read(out format.PixelFormatFlags, endian))
		{
			return false;
		}

		if (!reader.Read(out format.FourCC, endian))
		{
			return false;
		}

		if (!reader.Read(out format.RGBBitCount, endian))
		{
			return false;
		}

		if (!reader.Read(out format.RBitMask, endian))
		{
			return false;
		}

		if (!reader.Read(out format.GBitMask, endian))
		{
			return false;
		}

		if (!reader.Read(out format.BBitMask, endian))
		{
			return false;

		}

		if (!reader.Read(out format.ABitMask, endian))
		{
			return false;
		}

		return true;
	}
}

public class DdsHeader
{
	public const int HEADER_SIZE = 124;
	public const string DDS_MAGIC = "DDS ";

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

	public static bool Read(BinaryReader reader, out DdsHeader header, Endian endian = default)
	{
		header = new();

		if (!reader.Read(out string magic, 4) || magic != DDS_MAGIC)
		{
			// invalid DDS magic
			return false;
		}

		if (!reader.Read(out header.Size) || header.Size != HEADER_SIZE)
		{
			// invalid DDS header
			return false;
		}

		if (!reader.Read(out header.Flags, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Height, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Width, endian))
		{
			return false;
		}

		if (!reader.Read(out header.PitchOrLinearSize, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Depth, endian))
		{
			return false;
		}

		if (!reader.Read(out header.MipMapCount, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Reserved1, 11, endian))
		{
			return false;
		}

		if (!DdsPixelFormat.Read(reader, out header.PixelFormat, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Caps, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Caps2, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Caps3, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Caps4, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Reserved2, endian))
		{
			return false;
		}

		return true;
	}
}

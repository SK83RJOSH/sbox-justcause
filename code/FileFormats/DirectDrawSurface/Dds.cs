namespace JustCause.FileFormats.DirectDrawSurface;

using JustCause.FileFormats.DirectDrawSurface.Formats;
using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public struct Color888
{
	public byte r;
	public byte g;
	public byte b;
}

public struct Color8888
{
	public byte r;
	public byte g;
	public byte b;
	public byte a;
}

public class MipMapOffset
{
	public MipMapOffset(int width, int height, int stride, int dataOffset, int dataLen)
	{
		Stride = stride;
		Width = width;
		Height = height;
		DataOffset = dataOffset;
		DataLen = dataLen;
	}

	public int Stride { get; }

	public int Width { get; }

	public int Height { get; }

	public int DataOffset { get; }

	public int DataLen { get; }

	protected bool Equals(MipMapOffset other)
	{
		return Stride == other.Stride && Width == other.Width && Height == other.Height && DataOffset == other.DataOffset && DataLen == other.DataLen;
	}

	public override string ToString()
	{
		return $"{nameof(Stride)}: {Stride}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(DataOffset)}: {DataOffset}, {nameof(DataLen)}: {DataLen}";
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((MipMapOffset)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = Stride;
			hashCode = (hashCode * 397) ^ Width;
			hashCode = (hashCode * 397) ^ Height;
			hashCode = (hashCode * 397) ^ DataOffset;
			hashCode = (hashCode * 397) ^ DataLen;
			return hashCode;
		}
	}
}

public abstract class Dds
{
	public DdsHeader Header { get; }
	public abstract int BitsPerPixel { get; }
	public int BytesPerPixel => BitsPerPixel / 8;
	public virtual int Stride => CalculateStride((int)Header.Width, BitsPerPixel);
	public virtual byte[] Data { get; protected set; }
	public int Length { get; protected set; }
	public int Width => (int)Header.Width;
	public int Height => (int)Header.Height;
	public abstract bool Compressed { get; }
	public MipMapOffset[] MipMaps { get; protected set; }
	protected abstract void Decode(BinaryReader reader);
	protected int CalculateStride(int width, int pixel_depth) => 4 * ((width * pixel_depth + 3) / 4);

	protected Dds(DdsHeader header)
	{
		Header = header;
	}

	public static bool Read(MemoryStream stream, out Dds dds, Endian endian = default)
	{
		BinaryReader reader = new BinaryReader(stream);

		if (!DdsHeader.Read(reader, out DdsHeader header, endian))
		{
			dds = null;
			return false;
		}
		
		return Decode(reader, out dds, header);
	}

	private static bool Decode(BinaryReader reader, out Dds dds, DdsHeader header)
	{
		switch (header.PixelFormat.FourCC)
		{
			case DdsCompressionAlgorithm.D3DFMT_DXT1:
				dds = new Dxt1Dds(header);
				break;
			case DdsCompressionAlgorithm.D3DFMT_DXT3:
				dds = new Dxt3Dds(header);
				break;
			case DdsCompressionAlgorithm.D3DFMT_DXT5:
				dds = new Dxt5Dds(header);
				break;
			default:
				throw new ArgumentException($"DDS format {header.PixelFormat.FourCC} not supported.");
		}

		dds.Decode(reader);
		return true;
	}
}

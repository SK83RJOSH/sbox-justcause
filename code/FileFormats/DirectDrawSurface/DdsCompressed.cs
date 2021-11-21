namespace JustCause.FileFormats.DirectDrawSurface;

using System;
using System.IO;

public abstract class DdsCompressed : Dds
{
	protected abstract void Decode(BinaryReader reader, byte[] data, uint dataIndex, uint stride);
	protected abstract byte PixelDepthBytes { get; }
	protected abstract byte DivSize { get; }
	protected abstract byte CompressedBytesPerBlock { get; }
	public override bool Compressed => true;
	public override int Stride => DeflatedStrideBytes;
	private int BytesPerStride => WidthBlocks * CompressedBytesPerBlock;
	private int WidthBlocks => CalcBlocks((int)Header.Width);
	private int HeightBlocks => CalcBlocks((int)Header.Height);
	private int StridePixels => WidthBlocks * DivSize;
	private int DeflatedStrideBytes => StridePixels * PixelDepthBytes;
	private int CalcBlocks(int pixels) => Math.Max(1, (pixels + 3) / 4);

	protected DdsCompressed(DdsHeader header) : base(header)
	{
	}

	public byte[] DataDecode(BinaryReader reader)
	{
		var totalLen = AllocateMipMaps();
		byte[] data = new byte[totalLen];
		var pixelsLeft = totalLen;
		int dataIndex = 0;

		for (int imageIndex = 0; imageIndex < Header.MipMapCount && pixelsLeft > 0; imageIndex++)
		{
			int divSize = DivSize;
			int stride = DeflatedStrideBytes;
			int blocksPerStride = WidthBlocks;
			int indexPixelsLeft = HeightBlocks * DivSize * stride;
			var stridePixels = StridePixels;

			if (imageIndex != 0)
			{
				var width = Math.Max((int)(Header.Width / Math.Pow(2, imageIndex)), 1);
				var height = Math.Max((int)(Header.Height / Math.Pow(2, imageIndex)), 1);
				var widthBlocks = CalcBlocks(width);
				var heightBlocks = CalcBlocks(height);

				stridePixels = widthBlocks * DivSize;
				stride = stridePixels * PixelDepthBytes;
				blocksPerStride = widthBlocks;
				indexPixelsLeft = heightBlocks * DivSize * stride;
			}

			while (indexPixelsLeft > 0)
			{
				var origDataIndex = dataIndex;

				for (uint i = 0; i < blocksPerStride; i++)
				{
					Decode(reader, data, (uint)dataIndex, (uint)stridePixels);
					dataIndex += divSize * PixelDepthBytes;
				}

				var filled = stride * divSize;
				pixelsLeft -= filled;
				indexPixelsLeft -= filled;
				dataIndex = origDataIndex + filled;
			}
		}

		return data;
	}

	private int AllocateMipMaps()
	{
		var len = HeightBlocks * DivSize * DeflatedStrideBytes;

		if (Header.MipMapCount <= 1)
		{
			return len;
		}

		MipMaps = new MipMapOffset[Header.MipMapCount - 1];
		var totalLen = len;

		for (int i = 1; i < Header.MipMapCount; i++)
		{
			var width = Math.Max((int)(Header.Width / Math.Pow(2, i)), 1);
			var height = Math.Max((int)(Header.Height / Math.Pow(2, i)), 1);
			var widthBlocks = CalcBlocks(width);
			var heightBlocks = CalcBlocks(height);

			var stridePixels = widthBlocks * DivSize;
			var stride = stridePixels * PixelDepthBytes;

			len = heightBlocks * DivSize * stride;
			MipMaps[i - 1] = new MipMapOffset(width, height, stride, totalLen, len);
			totalLen += len;
		}

		return totalLen;
	}

	protected override void Decode(BinaryReader reader)
	{
		Data = DataDecode(reader);
	}
}

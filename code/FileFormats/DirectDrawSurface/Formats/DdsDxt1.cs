﻿namespace JustCause.FileFormats.DirectDrawSurface.Formats;

using System.IO;

public class Dxt1Dds : DdsCompressed
{
	private const int PIXEL_DEPTH = 4;
	private const int DIV_SIZE = 4;

	public Dxt1Dds(DdsHeader header) : base(header)
	{
	}

	protected override byte PixelDepthBytes => PIXEL_DEPTH;
	protected override byte DivSize => DIV_SIZE;
	protected override byte CompressedBytesPerBlock => 8;
	public override int BitsPerPixel => 8 * PIXEL_DEPTH;

	private readonly Color8888[] colors = new Color8888[4];

	protected override void Decode(BinaryReader reader, byte[] data, uint dataIndex, uint stride)
	{
		// Colors are stored in a pair of 16 bits
		ushort color0 = reader.ReadUInt16();
		ushort color1 = reader.ReadUInt16();

		// Extract R5G6B5 (in that order)
		colors[0].r = (byte)((color0 & 0x1f));
		colors[0].g = (byte)((color0 & 0x7E0) >> 5);
		colors[0].b = (byte)((color0 & 0xF800) >> 11);
		colors[0].r = (byte)(colors[0].r << 3 | colors[0].r >> 2);
		colors[0].g = (byte)(colors[0].g << 2 | colors[0].g >> 3);
		colors[0].b = (byte)(colors[0].b << 3 | colors[0].b >> 2);
		colors[0].a = 255;

		colors[1].r = (byte)((color1 & 0x1f));
		colors[1].g = (byte)((color1 & 0x7E0) >> 5);
		colors[1].b = (byte)((color1 & 0xF800) >> 11);
		colors[1].r = (byte)(colors[1].r << 3 | colors[1].r >> 2);
		colors[1].g = (byte)(colors[1].g << 2 | colors[1].g >> 3);
		colors[1].b = (byte)(colors[1].b << 3 | colors[1].b >> 2);
		colors[1].a = 255;

		// Used the two extracted colors to create two new colors that are
		// slightly different.
		if (color0 > color1)
		{
			colors[2].r = (byte)((2 * colors[0].r + colors[1].r) / 3);
			colors[2].g = (byte)((2 * colors[0].g + colors[1].g) / 3);
			colors[2].b = (byte)((2 * colors[0].b + colors[1].b) / 3);
			colors[2].a = 255;

			colors[3].r = (byte)((colors[0].r + 2 * colors[1].r) / 3);
			colors[3].g = (byte)((colors[0].g + 2 * colors[1].g) / 3);
			colors[3].b = (byte)((colors[0].b + 2 * colors[1].b) / 3);
			colors[3].a = 255;
		}
		else
		{
			colors[2].r = (byte)((colors[0].r + colors[1].r) / 2);
			colors[2].g = (byte)((colors[0].g + colors[1].g) / 2);
			colors[2].b = (byte)((colors[0].b + colors[1].b) / 2);
			colors[2].a = 255;

			colors[3].r = 0;
			colors[3].g = 0;
			colors[3].b = 0;
			colors[3].a = 0;
		}


		for (int i = 0; i < 4; i++)
		{
			// Every 2 bit is a code [0-3] and represent what color the
			// current pixel is

			// Read in a byte and thus 4 colors
			byte rowVal = reader.ReadByte();
			for (int j = 0; j < 8; j += 2)
			{
				// Extract code by shifting the row byte so that we can
				// AND it with 3 and get a value [0-3]
				var col = colors[(rowVal >> j) & 0x03];
				data[dataIndex++] = col.r;
				data[dataIndex++] = col.g;
				data[dataIndex++] = col.b;
				data[dataIndex++] = col.a;
			}

			// Jump down a row and start at the beginning of the row
			dataIndex += PIXEL_DEPTH * (stride - DIV_SIZE);
		}
	}
}

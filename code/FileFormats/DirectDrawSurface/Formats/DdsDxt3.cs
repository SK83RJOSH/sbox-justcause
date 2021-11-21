namespace JustCause.FileFormats.DirectDrawSurface.Formats;

using System.IO;

public class Dxt3Dds : DdsCompressed
{
	private const byte PIXEL_DEPTH = 4;
	private const byte DIV_SIZE = 4;

	protected override byte DivSize => DIV_SIZE;
	protected override byte CompressedBytesPerBlock => 16;
	protected override byte PixelDepthBytes => PIXEL_DEPTH;
	public override int BitsPerPixel => PIXEL_DEPTH * 8;

	public Dxt3Dds(DdsHeader header) : base(header)
	{
	}

	private readonly Color888[] colors = new Color888[4];

	protected override void Decode(BinaryReader reader, byte[] data, uint dataIndex, uint stride)
	{
		// Fetch row alphas first
		ushort[] rowAlphas = new ushort[4];
		
		for (int i = 0; i < rowAlphas.Length; i++)
		{
			rowAlphas[i] = reader.ReadUInt16();
		}

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

		colors[1].r = (byte)((color1 & 0x1f));
		colors[1].g = (byte)((color1 & 0x7E0) >> 5);
		colors[1].b = (byte)((color1 & 0xF800) >> 11);
		colors[1].r = (byte)(colors[1].r << 3 | colors[1].r >> 2);
		colors[1].g = (byte)(colors[1].g << 2 | colors[1].g >> 3);
		colors[1].b = (byte)(colors[1].b << 3 | colors[1].b >> 2);

		// Used the two extracted colors to create two new colors
		// that are slightly different.
		colors[2].r = (byte)((2 * colors[0].r + colors[1].r) / 3);
		colors[2].g = (byte)((2 * colors[0].g + colors[1].g) / 3);
		colors[2].b = (byte)((2 * colors[0].b + colors[1].b) / 3);

		colors[3].r = (byte)((colors[0].r + 2 * colors[1].r) / 3);
		colors[3].g = (byte)((colors[0].g + 2 * colors[1].g) / 3);
		colors[3].b = (byte)((colors[0].b + 2 * colors[1].b) / 3);

		for (int i = 0; i < 4; i++)
		{
			byte rowVal = reader.ReadByte();

			// Each row of rgb values have 4 alpha values that  are
			// encoded in 4 bits
			ushort rowAlpha = rowAlphas[i];

			for (int j = 0; j < 8; j += 2)
			{
				byte currentAlpha = (byte)((rowAlpha >> (j * 2)) & 0x0f);
				currentAlpha |= (byte)(currentAlpha << 4);
				var col = colors[((rowVal >> j) & 0x03)];
				data[dataIndex++] = col.r;
				data[dataIndex++] = col.g;
				data[dataIndex++] = col.b;
				data[dataIndex++] = currentAlpha;
			}
			dataIndex += PIXEL_DEPTH * (stride - DIV_SIZE);
		}
	}
}

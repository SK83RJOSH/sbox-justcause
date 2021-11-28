namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum PrimitiveType : int
{
	TriangleList,
	TriangleStrip,
	TriangleFan,
	IndexedTriangleList,
	IndexedTriangleStrip,
	IndexedTriangleFan,
	LineList,
	PointSprite,
	IndexedPointSprite
};

public struct Material
{
	public string Texture0;
	public string Texture1;
	public string Texture2;
	public string Texture3;
	public string Texture4;
	public string Texture5;
	public string Texture6;
	public string Texture7;
	public PrimitiveType PrimitiveType;

	public static bool Read(BinaryReader reader, out Material material, Endian endian = default)
	{
		material = default;

		if (!reader.Read(out material.Texture0, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture1, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture2, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture3, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture4, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture5, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture6, endian))
		{
			return false;
		}

		if (!reader.Read(out material.Texture7, endian))
		{
			return false;
		}

		if (!reader.Read(out material.PrimitiveType, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out Material material, Endian endian = default)
	{
		return Material.Read(reader, out material, endian);
	}
}

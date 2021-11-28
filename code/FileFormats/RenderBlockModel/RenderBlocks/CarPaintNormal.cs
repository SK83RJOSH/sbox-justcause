namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using PackedNormal = DataTypes.PackedVector3<DataTypes.NormalPackingModel>;
using Vector2f = DataTypes.Vector2<float>;

public struct CarPaintNormal
{
	public Vector2f UV;
	public float Light;
	public PackedNormal Normal;
	public PackedNormal DeformedNormal;
	public PackedNormal Tangent;
	public PackedNormal DeformedTangent;

	public static bool Read(BinaryReader reader, out CarPaintNormal normal, Endian endian = default)
	{
		normal = new();

		if (!reader.Read(out normal.UV, endian))
		{
			return false;
		}

		if (!reader.Read(out normal.Light, endian))
		{
			return false;
		}

		if (!reader.Read(out normal.Normal, endian))
		{
			return false;
		}

		if (!reader.Read(out normal.DeformedNormal, endian))
		{
			return false;
		}

		if (!reader.Read(out normal.Tangent, endian))
		{
			return false;
		}

		if (!reader.Read(out normal.DeformedTangent, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out CarPaintNormal normal, Endian endian = default)
	{
		return CarPaintNormal.Read(reader, out normal, endian);
	}

	public static bool Read(this BinaryReader reader, out CarPaintNormal[] normals, Endian endian = default)
	{
		if (!reader.Read(out int count, endian))
		{
			normals = default;
			return false;
		}

		normals = new CarPaintNormal[count];

		for (int i = 0; i < count; ++i)
		{
			if (!reader.Read(out normals[i], endian))
			{
				return false;
			}
		}

		return true;
	}
}

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

public struct CarPaintAttributes : IBinaryFormat
{
	public Vector3<float>[] TwoToneColors;
	public float Specularity;
	public float DepthBias;
	public float Reflectivity;
	public Vector4<float> Noise;
	public CarPaintFlags Flags;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		TwoToneColors = new Vector3<float>[2];
		reader.ReadBinaryFormatArray(TwoToneColors, endian);
		Specularity = reader.ReadSingle(endian);
		DepthBias = reader.ReadSingle(endian);
		Reflectivity = reader.ReadSingle(endian);
		Noise.Deserialize(reader, endian);
		Flags = (CarPaintFlags)reader.ReadUInt32(endian);
	}
}

namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public struct CarPaintNormal : IBinaryFormat
{
	public Vector3<float> UVL;
	public PackedVector3 Normal = new(Packing.ZXY);
	public PackedVector3 DeformedNormal = new(Packing.XZY);
	public PackedVector3 Tangent = new(Packing.ZXY);
	public PackedVector3 DeformedTangent = new(Packing.XZY);

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		UVL.Deserialize(reader, endian);
		Normal.Deserialize(reader, endian);
		DeformedNormal.Deserialize(reader, endian);
		Tangent.Deserialize(reader, endian, true);
		DeformedTangent.Deserialize(reader, endian, true);
	}
}

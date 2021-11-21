namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public struct DeformableWindowVertex : IBinaryFormat
{
	public Vector4<float> Position;
	public Vector4<short> DeformWeights;
	public PackedVector3 Normal = new(Packing.Normal);
	public PackedVector3 DeformedNormal = new(Packing.Normal);
	public PackedVector3 Tangent = new(Packing.Normal);
	public PackedVector3 DeformedTangent = new(Packing.Normal);
	public Vector2<float> UV;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Position.Deserialize(reader, endian);
		DeformWeights.Deserialize(reader, endian);
		Normal.Deserialize(reader, endian);
		DeformedNormal.Deserialize(reader, endian);
		Tangent.Deserialize(reader, endian);
		DeformedTangent.Deserialize(reader, endian);
		UV.Deserialize(reader, endian);
	}
}

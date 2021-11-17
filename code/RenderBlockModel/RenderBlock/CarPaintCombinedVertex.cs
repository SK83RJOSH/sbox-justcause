namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System.IO;

public struct CarPaintCombinedVertex : IBinaryFormat
{
	public Vector4<float> Position;
	public Vector4<short> DeformWeights;
	public Vector3<float> UVL;
	public PackedVector3 Normal = new(Packing.ZXY);
	public PackedVector3 DeformedNormal = new(Packing.XZY);
	public PackedVector3 Tangent = new(Packing.ZXY);
	public PackedVector3 DeformedTangent = new(Packing.XZY);

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Position.Deserialize(reader, endian);
		DeformWeights.Deserialize(reader, endian);
		UVL.Deserialize(reader, endian);
		Normal.Deserialize(reader, endian);
		DeformedNormal.Deserialize(reader, endian);
		Tangent.Deserialize(reader, endian);
		DeformedTangent.Deserialize(reader, endian);
	}
}

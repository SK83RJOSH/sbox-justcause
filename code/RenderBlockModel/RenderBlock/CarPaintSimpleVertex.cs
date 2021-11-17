namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System.IO;

public struct CarPaintSimpleVertex : IBinaryFormat
{
	public Vector3<float> Position;
	public PackedVector3 Normal = new(Packing.Normal);
	public Vector2<float> UV;
	public PackedVector3 Tangent = new(Packing.Normal);
	public PackedVector3 Binormal = new(Packing.Normal);

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Position.Deserialize(reader, endian);
		Normal.Deserialize(reader, endian);
		UV.Deserialize(reader, endian);
		Tangent.Deserialize(reader, endian);
		Binormal.Deserialize(reader, endian);
	}
}

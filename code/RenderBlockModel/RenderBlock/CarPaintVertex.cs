namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System.IO;

public struct CarPaintVertex : IBinaryFormat
{
	public Vector4<float> Position;
	public Vector4<short> DeformWeights;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Position.Deserialize(reader, endian);
		DeformWeights.Deserialize(reader, endian);
	}
}

namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
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

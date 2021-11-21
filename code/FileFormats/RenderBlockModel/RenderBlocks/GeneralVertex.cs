namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;

public struct GeneralVertex
{
	public Vector3<float> Position;
	public Vector2<float>[] UVs = new Vector2<float>[2];
	public PackedVector3 Normal = new(Packing.Normal);
	public PackedVector3 Tangent = new(Packing.Normal);
	public PackedColor Color;

	public void Deserialize(BinaryReader reader, Endian endian, VertexFormat Format)
	{
		if (Format == VertexFormat.Float32)
		{
			Position.Deserialize(reader, endian);
			reader.ReadBinaryFormatArray(UVs, endian);
			Normal.Deserialize(reader, endian);
			Tangent.Deserialize(reader, endian);
			Color.Deserialize(reader, endian);
		}
		else
		{
			Vector2<short>[] PackedUVs = new Vector2<short>[2];
			reader.ReadBinaryFormatArray(PackedUVs, endian);
			UVs[0] = new Vector2<float>(PackedUVs[0].X / 32767f, PackedUVs[0].Y / 32767f);
			UVs[1] = new Vector2<float>(PackedUVs[1].X / 32767f, PackedUVs[1].Y / 32767f);

			Normal.Deserialize(reader, endian);
			Tangent.Deserialize(reader, endian);
			Color.Deserialize(reader, endian);

			Vector3<short> PackedPosition = default;
			PackedPosition.Deserialize(reader, endian);
			Position = new Vector3<float>(PackedPosition.X / 32767f, PackedPosition.Y / 32767f, PackedPosition.Z / 32767f);
			reader.ReadBytes(2);
		}
	}
}

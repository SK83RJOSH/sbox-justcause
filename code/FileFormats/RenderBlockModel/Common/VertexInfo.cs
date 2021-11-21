namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum VertexFormat : uint
{
	Float32,
	Int16
}

public struct VertexInfo
{
	public VertexFormat Format;
	public float Scale;
	public Vector2<float>[] UVExtents;
	public float ColorExtent;
	public Vector4<byte> Color;

	public void Deserialize(BinaryReader reader, Endian endian, bool packed_extents = false)
	{
		Format = (VertexFormat)reader.ReadUInt32(endian);
		Scale = reader.ReadSingle(endian);
		UVExtents = new Vector2<float>[2];

		if (!packed_extents)
		{
			reader.ReadBinaryFormatArray(UVExtents, endian);
		}
		else
		{
			UVExtents[0] = new Vector2<float>(reader.ReadSingle(endian));
			UVExtents[1] = new Vector2<float>(reader.ReadSingle(endian));
		}

		ColorExtent = reader.ReadSingle(endian);
		Color.Deserialize(reader, endian);
	}
}

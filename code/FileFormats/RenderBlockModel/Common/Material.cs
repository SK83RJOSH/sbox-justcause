namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System.IO;
using System.Text;

public enum PrimitiveType : int
{
	TriangleList,
	TriangleStrip,
	TriangleFan,
	IndexedTriangleList,
	IndexedTriangleStrip,
	IndexedTriangleFan,
	LineList,
	PointSprite,
	IndexedPointSprite
};

public struct Material : IBinaryFormat
{
	public string Texture0;
	public string Texture1;
	public string Texture2;
	public string Texture3;
	public string Texture4;
	public string Texture5;
	public string Texture6;
	public string Texture7;
	public PrimitiveType PrimitiveType;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Texture0 = reader.ReadString(Encoding.ASCII, endian);
		Texture1 = reader.ReadString(Encoding.ASCII, endian);
		Texture2 = reader.ReadString(Encoding.ASCII, endian);
		Texture3 = reader.ReadString(Encoding.ASCII, endian);
		Texture4 = reader.ReadString(Encoding.ASCII, endian);
		Texture5 = reader.ReadString(Encoding.ASCII, endian);
		Texture6 = reader.ReadString(Encoding.ASCII, endian);
		Texture7 = reader.ReadString(Encoding.ASCII, endian);
		PrimitiveType = (PrimitiveType)reader.ReadInt32(endian);
	}
}

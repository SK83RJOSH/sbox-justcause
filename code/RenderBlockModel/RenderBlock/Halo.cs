namespace JustCause.RenderBlockModel.RenderBlock;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System;
using System.IO;

[Sandbox.Library]
public struct Halo : IRenderBlock
{
	public byte Version;
	public Vector3<float> Position;
	public Vector2<float> UV;
	public Vector2<float> Size;
	public Vector4<byte> Color;

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		Version = reader.ReadByte();

		if (Version != 0)
		{
			throw new FormatException("unhandled Halo version");
		}

		Position.Deserialize(reader, endian);

		Vector4<byte> PackedData = default;
		PackedData.Deserialize(reader, endian);
		UV = new Vector2<float>(PackedData.X / 255f, PackedData.Y / 255f);
		Size = new Vector2<float>(PackedData.Z / 255f, PackedData.W / 255f);

		Color.Deserialize(reader, endian);
	}
}

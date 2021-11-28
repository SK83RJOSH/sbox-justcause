namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.RenderBlockModel.DataTypes;
using JustCause.FileFormats.Utilities;
using System.IO;
using Vector2f = DataTypes.Vector2<float>;
using Vector3f = DataTypes.Vector3<float>;
using Vector4b = DataTypes.Vector4<byte>;

public class Halo : IRenderBlock
{
	public Vector3f Position;
	public Vector2f UV;
	public Vector2f Size;
	public Vector4b Color;

	public static bool Read(BinaryReader reader, out Halo block, Endian endian = default)
	{
		block = new();

		if (!reader.Read(out byte version, endian) || version != 0)
		{
			// unhandled Halo version
			return false;
		}		

		if (!reader.Read(out block.Position, endian))
		{
			return false;
		}

		if (!reader.Read(out Vector4b data, endian))
		{
			return false;
		}

		block.UV = new Vector2f(data.X / 255f, data.Y / 255f);
		block.Size = new Vector2f(data.Z / 255f, data.W / 255f);

		if (!reader.Read(out block.Color, endian))
		{
			return false;
		}

		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out Halo block, Endian endian = default)
	{
		return Halo.Read(reader, out block, endian);
	}
}

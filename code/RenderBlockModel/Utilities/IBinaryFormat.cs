namespace JustCause.RenderBlockModel.Utilities;

using System.IO;

public interface IBinaryFormat
{
	void Deserialize(BinaryReader reader, Endian endian);
}

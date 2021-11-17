namespace JustCause.RenderBlockModel.Utilities;

using System.IO;

public interface IRenderBlock
{
	void Deserialize(BinaryReader reader, Endian endian);
}

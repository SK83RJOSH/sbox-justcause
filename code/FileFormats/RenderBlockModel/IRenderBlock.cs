namespace JustCause.FileFormats.RenderBlockModel;

using JustCause.FileFormats.Utilities;
using System.IO;

public interface IRenderBlock
{
	void Deserialize(BinaryReader reader, Endian endian);
}

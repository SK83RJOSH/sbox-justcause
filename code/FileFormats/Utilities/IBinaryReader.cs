namespace JustCause.FileFormats.Utilities;

using System.IO;

public interface IBinaryReader
{
	public abstract bool Read(BinaryReader reader, Endian endian);
}

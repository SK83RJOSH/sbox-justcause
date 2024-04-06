namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkBaseObject : IBinaryReader
{
	public bool Read(BinaryReader reader, Endian endian)
	{
		reader.Move(4);
		return true;
	}

	public static bool Read(BinaryReader reader, out hkBaseObject value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

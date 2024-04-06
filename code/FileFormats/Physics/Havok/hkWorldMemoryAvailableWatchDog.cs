namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkWorldMemoryAvailableWatchDog : IBinaryReader
{
	public bool Read(BinaryReader reader, Endian endian)
	{
		return true;
	}

	public static bool Read(BinaryReader reader, out hkWorldMemoryAvailableWatchDog value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

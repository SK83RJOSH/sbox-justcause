namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class DataPointer<T> : IBinaryReader
	where T : new()
{
	public T Value;

	public bool Read(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out int pointer, endian))
		{
			return false;
		}

		if (pointer != 0)
		{
			long position = reader.Tell();
			reader.Seek(pointer);
			if (!reader.Read(out Value, endian))
			{
				return false;
			}
			reader.Seek(position);
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkBaseObject value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

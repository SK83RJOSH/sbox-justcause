namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkSmallArray<T> : IBinaryReader
	where T : new()
{
	public T[] Data;
	public ushort Size;
	public ushort CapacityAndFlags;

	public bool Read(BinaryReader reader, Endian endian)
	{
		int pointer;
		long position;

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		if (!reader.Read(out Size, endian))
		{
			return false;
		}

		if (pointer != 0)
		{
			position = reader.Tell();
			reader.Seek(pointer);

			if (!reader.Read(out Data, Size, endian))
			{
				return false;
			}

			reader.Seek(position);
		}

		if (!reader.Read(out CapacityAndFlags, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkArray<T> value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

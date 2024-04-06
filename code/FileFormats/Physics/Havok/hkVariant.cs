namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkVariant : IBinaryReader
{
	public object Value;
	public hkClass Class;

	public bool Read(BinaryReader reader, Endian endian)
	{
		int pointer;
		long position;

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		reader.Move(4); // ignored Object

		reader.Seek(position);

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out Class, endian))
		{
			return false;
		}

		reader.Seek(position);

		return true;
	}

	public static bool Read(BinaryReader reader, out hkVariant value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkReferencedObject : hkBaseObject
{
	public ushort EmSizeAndFlags;
	public short ReferenceCount;

	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
		{
			return false;
		}

		if (!reader.Read(out EmSizeAndFlags, endian))
		{
			return false;
		}

		if (!reader.Read(out ReferenceCount, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkReferencedObject value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

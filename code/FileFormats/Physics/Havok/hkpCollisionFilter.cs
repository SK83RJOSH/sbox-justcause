namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum hkpFilterType : uint
{
	Unknown = 0,
	Null = 1,
	Group = 2,
	List = 3,
	Custom = 4,
	Pair = 5,
	Constraint = 6,
}

public class hkpCollisionFilter : hkReferencedObject
{
	public uint[] Prepad;
	public hkpFilterType Type;
	public uint[] Postpad;

	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
		{
			return false;
		}

		reader.Move(16);

		if (!reader.Read(out Prepad, 2, endian))
		{
			return false;
		}

		if (!reader.Read(out Type, endian))
		{
			return false;
		}

		if (!reader.Read(out Postpad, 3, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkpCollisionFilter value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}


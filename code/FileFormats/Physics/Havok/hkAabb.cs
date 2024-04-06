namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkAabb : IBinaryReader
{
	public hkVector4 In;
	public hkVector4 Ax;

	public bool Read(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out In, endian))
		{
			return false;
		}

		if (!reader.Read(out Ax, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkAabb value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

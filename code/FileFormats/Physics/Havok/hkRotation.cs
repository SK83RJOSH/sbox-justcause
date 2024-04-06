namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkRotation : hkMatrix3
{
	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkRotation value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

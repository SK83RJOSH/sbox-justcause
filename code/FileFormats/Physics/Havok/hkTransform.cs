namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkTransform : IBinaryReader
{
	public hkRotation Rotation;
	public hkVector4 Translation;

	public bool Read(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out Rotation, endian))
		{
			return false;
		}

		if (!reader.Read(out Translation, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkTransform value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

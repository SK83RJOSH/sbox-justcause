namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkVector4 : IBinaryReader
{
	public float X;
	public float Y;
	public float Z;
	public float W;

	public bool Read(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out X, endian))
		{
			return false;
		}

		if (!reader.Read(out Y, endian))
		{
			return false;
		}

		if (!reader.Read(out Z, endian))
		{
			return false;
		}

		if (!reader.Read(out W, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkVector4 value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

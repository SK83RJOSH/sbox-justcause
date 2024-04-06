namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkMatrix3 : IBinaryReader
{
	public hkVector4 Col0;
	public hkVector4 Col1;
	public hkVector4 Col2;

	public bool Read(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out Col0, endian))
		{
			return false;
		}

		if (!reader.Read(out Col1, endian))
		{
			return false;
		}

		if (!reader.Read(out Col2, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkMatrix3 value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

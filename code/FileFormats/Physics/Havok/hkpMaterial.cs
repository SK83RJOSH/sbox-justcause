namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum hkpResponseType : sbyte
{
	Invalid,
	SimpleContact,
	Reporting,
	None,
}

public class hkpMaterial : IBinaryReader
{
	public hkpResponseType ResponseType;
	public float Friction;
	public float Restitution;

	public bool Read(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out ResponseType, endian))
		{
			return false;
		}

		if (!reader.Read(out Friction, endian))
		{
			return false;
		}

		if (!reader.Read(out Restitution, endian))
		{
			return false;
		}

		reader.Move(3);
		return true;
	}

	public static bool Read(BinaryReader reader, out hkpMaterial value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}


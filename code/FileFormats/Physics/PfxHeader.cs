namespace JustCause.FileFormats.Physics;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum PfxFormat : uint
{
	Binary,
	Xml,
}

public enum PfxType : uint
{
	Generic = 0,
	RigidBody,
	Character,
	HeightField,
	Car,
	Breakable,
	MotorBike,
	Helicopter = 8,
	Boat,
	AirPlane = 16,
	Mopp = 18,
	Vehicle = 20,
	Camera,
	Ragdoll,
}

public struct PfxHeader
{
	string Magic;
	public PfxFormat Format;
	public PfxType Type;
	public uint Version;

	public static bool Read(BinaryReader reader, out PfxHeader header, Endian endian)
	{
		header = default;

		if (!reader.Read(out header.Magic, 4) || header.Magic != "PFX\0")
		{
			return false;
		}

		if (!reader.Read(out header.Format, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Type, endian))
		{
			return false;
		}

		if (!reader.Read(out header.Version, endian) || header.Version == 0)
		{
			return false;
		}

		reader.Move(52);
		return true;
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out PfxHeader header, Endian endian = default)
	{
		return PfxHeader.Read(reader, out header, endian);
	}
}

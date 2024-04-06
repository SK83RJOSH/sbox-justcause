namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkpPhysicsSystem : hkReferencedObject
{
	public hkArray<DataPointer<hkpRigidBody>> RigidBodies;
	public hkArray<DataPointer<hkpConstraintInstance>> Constraints;
	public hkArray<DataPointer<hkpAction>> Actions;
	public hkArray<DataPointer<hkpPhantom>> Phantoms;
	public string Name;
	public uint UserData;
	public bool Active;

	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
		{
			return false;
		}

		if (reader.Read(out RigidBodies, endian))
		{
			return false;
		}

		if (reader.Read(out Constraints, endian))
		{
			return false;
		}

		if (reader.Read(out Actions, endian))
		{
			return false;
		}

		if (reader.Read(out Phantoms, endian))
		{
			return false;
		}

		int pointer;
		long position;

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out Name, -1))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out UserData, endian))
		{
			return false;
		}

		if (!reader.Read(out Active, endian))
		{
			return false;
		}

		reader.Move(3);
		return true;
	}

	public static bool Read(BinaryReader reader, out hkpPhysicsSystem value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

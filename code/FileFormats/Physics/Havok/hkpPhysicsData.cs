namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkpPhysicsData : hkReferencedObject
{
	public class SplitPhysicsSystemsOutput : IBinaryReader
	{
		public hkpPhysicsSystem UnconstrainedFixedBodies;
		public hkpPhysicsSystem UnconstrainedKeyframedBodies;
		public hkpPhysicsSystem UnconstrainedMovingBodies;
		public hkpPhysicsSystem Phantoms;
		public hkArray<DataPointer<hkpPhysicsSystem>> ConstrainedSystems;

		public bool Read(BinaryReader reader, Endian endian)
		{
			int pointer;
			long position;

			if (!reader.Read(out pointer, endian))
			{
				return false;
			}

			position = reader.Tell();
			reader.Seek(pointer);

			UnconstrainedFixedBodies = new();
			if (pointer != 0 && !UnconstrainedFixedBodies.Read(reader, endian))
			{
				return false;
			}

			reader.Seek(position);

			if (!reader.Read(out pointer, endian))
			{
				return false;
			}

			position = reader.Tell();
			reader.Seek(pointer);

			UnconstrainedKeyframedBodies = new();
			if (pointer != 0 && !UnconstrainedKeyframedBodies.Read(reader, endian))
			{
				return false;
			}

			reader.Seek(position);

			if (!reader.Read(out pointer, endian))
			{
				return false;
			}

			position = reader.Tell();
			reader.Seek(pointer);

			UnconstrainedMovingBodies = new();
			if (pointer != 0 && !UnconstrainedMovingBodies.Read(reader, endian))
			{
				return false;
			}

			reader.Seek(position);

			if (!reader.Read(out pointer, endian))
			{
				return false;
			}

			position = reader.Tell();
			reader.Seek(pointer);

			Phantoms = new();
			if (pointer != 0 && !Phantoms.Read(reader, endian))
			{
				return false;
			}

			reader.Seek(position);

			ConstrainedSystems = new();
			if (!ConstrainedSystems.Read(reader, endian))
			{
				return false;
			}

			return true;
		}

		public static bool Read(BinaryReader reader, out SplitPhysicsSystemsOutput value, Endian endian)
		{
			value = new();
			return value.Read(reader, endian);
		}
	}

	public hkpWorldCinfo WorldCinfo;
	public hkArray<hkpPhysicsSystem> Systems;

	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
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

		WorldCinfo = new();
		if (pointer != 0 && !WorldCinfo.Read(reader, endian))
		{
			return false;
		}

		reader.Seek(position);

		Systems = new();
		if (!Systems.Read(reader, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkpPhysicsData value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

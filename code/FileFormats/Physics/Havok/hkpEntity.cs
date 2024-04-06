namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkpEntity : hkpWorldObject
{
	public class SpuCollisionCallback : IBinaryReader
	{
		public hkSpuCollisionCallbackUtil Util;
		public ushort Capacity;
		public byte EventFilter;
		public byte UserFilter;

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

			if (pointer != 0 && !reader.Read(out Util, endian))
			{
				return false;
			}

			reader.Seek(position);

			if (!reader.Read(out Capacity, endian))
			{
				return false;
			}

			if (!reader.Read(out EventFilter, endian))
			{
				return false;
			}

			if (!reader.Read(out UserFilter, endian))
			{
				return false;
			}

			return true;
		}

		public static bool Read(BinaryReader reader, out SpuCollisionCallback value, Endian endian)
		{
			value = new();
			return value.Read(reader, endian);
		}
	}

	public class ExtendedListeners : IBinaryReader
	{
		public hkSmallArray<hkpEntityActivationListener> ActivationListeners;
		public hkSmallArray<hkpEntityListener> EntityListeners;

		public bool Read(BinaryReader reader, Endian endian)
		{
			if (!reader.Read(out ActivationListeners, endian))
			{
				return false;
			}

			if (!reader.Read(out EntityListeners, endian))
			{
				return false;
			}

			return true;
		}

		public static bool Read(BinaryReader reader, out ExtendedListeners value, Endian endian)
		{
			value = new();
			return value.Read(reader, endian);
		}
	}

	public hkpMaterial Material;
	public uint SolverData;
	public ushort StorageIndex;
	public ushort ProcessContactCallbackDelay;
	public hkSmallArray<hkConstraintInternal> ConstraintsMaster;
	public hkArray<hkpConstraintInstance> ConstraintsSlave;
	public hkArray<byte> ConstraintRuntime;
	public hkpSimulationIsland SimulationIsland;
	public sbyte AutoRemoveLevel;
	public byte NumUserDatasInContactPointProperties;
	public uint Uid;
	public SpuCollisionCallback SpuCollisionCallback;
	public ExtendedListeners ExtendedListeners;
	public hkpMaxSizeMotion Motion;
	public hkSmallArray<hkpCollisionListener> CollisionListeners;
	public hkSmallArray<hkpAction> Actions;

	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
		{
			return false;
		}

		if (!reader.Read(out Material, endian))
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

		reader.Move(4); // ignored BreakOffPartsUtil

		reader.Seek(position);

		if (!reader.Read(out SolverData, endian))
		{
			return false;
		}

		if (!reader.Read(out StorageIndex, endian))
		{
			return false;
		}

		if (!reader.Read(out ProcessContactCallbackDelay, endian))
		{
			return false;
		}

		if (!reader.Read(out ConstraintsMaster, endian))
		{
			return false;
		}

		if (!reader.Read(out ConstraintsSlave, endian))
		{
			return false;
		}

		if (!reader.Read(out ConstraintRuntime, endian))
		{
			return false;
		}

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out SimulationIsland, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out AutoRemoveLevel, endian))
		{
			return false;
		}

		if (!reader.Read(out NumUserDatasInContactPointProperties, endian))
		{
			return false;
		}

		if (!reader.Read(out Uid, endian))
		{
			return false;
		}

		if (!reader.Read(out SpuCollisionCallback, endian))
		{
			return false;
		}

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out ExtendedListeners, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out Motion, endian))
		{
			return false;
		}

		if (!reader.Read(out CollisionListeners, endian))
		{
			return false;
		}

		if (!reader.Read(out Actions, endian))
		{
			return false;
		}

		reader.Move(2);
		return true;
	}

	public static bool Read(BinaryReader reader, out hkpEntity value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

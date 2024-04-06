namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkRootLevelContainer : IBinaryReader
{
	public class NamedVariant : IBinaryReader
	{
		public string Name;
		public string ClassName;
		public hkVariant Variant;

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

			if (pointer != 0 && !reader.Read(out Name, -1))
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

			if (pointer != 0 && !reader.Read(out ClassName, -1))
			{
				return false;
			}

			reader.Seek(position);

			if (!reader.Read(out Variant, endian))
			{
				return false;
			}

			return true;
		}

		public static bool Read(BinaryReader reader, out NamedVariant value, Endian endian)
		{
			value = new();
			return value.Read(reader, endian);
		}
	}

	public NamedVariant[] NamedVariants;
	public int NumNamedVariants;

	public bool Read(BinaryReader reader, Endian endian)
	{
		int pointer;
		long position;

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		if (!reader.Read(out NumNamedVariants, endian))
		{
			return false;
		}

		if (pointer != 0)
		{
			position = reader.Tell();
			reader.Seek(pointer);

			if (!reader.Read(out NamedVariants, NumNamedVariants, endian))
			{
				return false;
			}

			reader.Seek(position);
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkRootLevelContainer value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

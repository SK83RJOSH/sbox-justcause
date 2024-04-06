namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public class hkCustomAttributes : IBinaryReader
{
	public class Attribute : IBinaryReader
	{
		public string Name;
		public hkVariant Value;

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

			if (!reader.Read(out Value, endian))
			{
				return false;
			}

			return true;
		}

		public static bool Read(BinaryReader reader, out Attribute value, Endian endian)
		{
			value = new();
			return value.Read(reader, endian);
		}
	}

	public Attribute[] Attributes;
	public int NumAttributes;

	public bool Read(BinaryReader reader, Endian endian)
	{
		int pointer;
		long position;

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		if (!reader.Read(out NumAttributes, endian))
		{
			return false;
		}

		if (pointer != 0)
		{
			position = reader.Tell();
			reader.Seek(pointer);

			if (!reader.Read(out Attributes, NumAttributes, endian))
			{
				return false;
			}

			reader.Seek(position);
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkCustomAttributes value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

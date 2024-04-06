namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum hkType : byte
{
	Void,
	Bool,
	Char,
	Int8,
	UInt8,
	Int16,
	UInt16,
	Int32,
	UInt32,
	Int64,
	UInt64,
	Real,
	Vector4,
	Quaternion,
	Matrix3,
	Rotation,
	QsTransform,
	Matrix4,
	Transform,
	Zero,
	Pointer,
	FunctionPointer,
	Array,
	InplaceArray,
	Enum,
	Struct,
	SimpleArray,
	HomogeneousArray,
	Variant,
	CString,
	ULong,
	Flags,
	Max,
}

[System.Flags]
public enum hkTypeFlags : uint
{
	None = 0,
	NotSerializable = 1,
	Size8 = 8,
	Enum8 = 8,
	Size16 = 16,
	Enum16 = 16,
	Size32 = 32,
	Enum32 = 32,
	Align8 = 128,
	Align16 = 256,
	NotOwned = 512,
	SerializeIgnored = 1024,
}

public class hkClassMember : IBinaryReader
{
    public class TypeProperties : IBinaryReader
    {
		public hkType Type;
		public sbyte Name;
		public short Size;
		public short Align;

        public bool Read(BinaryReader reader, Endian endian)
        {
            if (!reader.Read(out Type, endian))
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

            if (pointer != 0 && !reader.Read(out Name, endian))
            {
                return false;
            }

            reader.Seek(position);

            if (!reader.Read(out Size, endian))
            {
                return false;
            }

            if (!reader.Read(out Align, endian))
            {
                return false;
            }

            reader.Move(3);
			return true;
        }

        public static bool Read(BinaryReader reader, out TypeProperties value, Endian endian)
        {
            value = new();
            return value.Read(reader, endian);
        }
    }

    public string Name;
    public hkClass Class;
    public hkClassEnum Enum;
    public hkType Type;
    public hkType Subtype;
    public short CArraySize;
    public hkTypeFlags Flags;
    public ushort Offset;
    public hkCustomAttributes Attributes;

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

        if (pointer != 0 && !reader.Read(out Class, endian))
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

        if (pointer != 0 && !reader.Read(out Enum, endian))
        {
            return false;
        }

        reader.Seek(position);

        if (!reader.Read(out Type, endian))
        {
            return false;
        }

        if (!reader.Read(out Subtype, endian))
        {
            return false;
        }

        if (!reader.Read(out CArraySize, endian))
        {
            return false;
        }

        if (!reader.Read(out Flags, endian))
        {
            return false;
        }

        if (!reader.Read(out Offset, endian))
        {
            return false;
        }

        if (!reader.Read(out pointer, endian))
        {
            return false;
        }

        position = reader.Tell();
        reader.Seek(pointer);

        if (pointer != 0 && !reader.Read(out Attributes, endian))
        {
            return false;
        }

        reader.Seek(position);
		return true;
    }

    public static bool Read(BinaryReader reader, out hkClassMember value, Endian endian)
    {
        value = new();
        return value.Read(reader, endian);
    }
}

public class hkClassEnum : IBinaryReader
{
    public class Item : IBinaryReader
    {
		public int Value;
		public string Name;

        public bool Read(BinaryReader reader, Endian endian)
        {
            if (!reader.Read(out Value, endian))
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
			return true;
        }

        public static bool Read(BinaryReader reader, out Item value, Endian endian)
        {
            value = new();
            return value.Read(reader, endian);
        }
    }

	public string Name;
    public Item[] Items;
    public int NumItems;
    public hkCustomAttributes Attributes;
    public hkTypeFlags Flags;

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

        if (!reader.Read(out NumItems, endian))
        {
            return false;
		}

		if (pointer != 0)
		{
			position = reader.Tell();
			reader.Seek(pointer);

			if (!reader.Read(out Items, NumItems, endian))
			{
				return false;
			}

			reader.Seek(position);
		}

		if (!reader.Read(out pointer, endian))
        {
            return false;
        }

        position = reader.Tell();
        reader.Seek(pointer);

        if (pointer != 0 && !reader.Read(out Attributes, endian))
        {
            return false;
        }

        reader.Seek(position);

        if (!reader.Read(out Flags, endian))
        {
            return false;
		}

		return true;
	}

    public static bool Read(BinaryReader reader, out hkClassEnum value, Endian endian)
    {
        value = new();
        return value.Read(reader, endian);
    }
}

public class hkClass : IBinaryReader
{
	public string Name;
	public hkClass Parent;
	public int ObjectSize;
	public int NumImplementedInterfaces;
	public hkClassEnum[] DeclaredEnums;
	public int NumDeclaredEnums;
	public hkClassMember[] DeclaredMembers;
	public int NumDeclaredMembers;
	public hkCustomAttributes Attributes;
	public hkTypeFlags Flags;
	public int DescribedVersion;

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

		if (pointer != 0 && !reader.Read(out Parent, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out ObjectSize, endian))
		{
			return false;
		}

		if (!reader.Read(out NumImplementedInterfaces, endian))
		{
			return false;
		}

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		if (!reader.Read(out NumDeclaredEnums, endian))
		{
			return false;
		}

		if (pointer != 0)
		{
			position = reader.Tell();
			reader.Seek(pointer);

			if (!reader.Read(out DeclaredEnums, NumDeclaredEnums, endian))
			{
				return false;
			}

			reader.Seek(position);
		}

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		if (!reader.Read(out NumDeclaredMembers, endian))
		{
			return false;
		}

		if (pointer != 0)
		{
			position = reader.Tell();
			reader.Seek(pointer);

			if (!reader.Read(out DeclaredMembers, NumDeclaredMembers, endian))
			{
				return false;
			}

			reader.Seek(position);
		}

		reader.Move(sizeof(int)); // ignored Defaults

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out Attributes, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out Flags, endian))
		{
			return false;
		}

		if (!reader.Read(out DescribedVersion, endian))
		{
			return false;
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out hkClass value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}

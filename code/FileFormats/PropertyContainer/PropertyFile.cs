namespace JustCause.FileFormats.PropertyContainer;

using JustCause.FileFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

enum SectionType : ushort
{
	Unknown,
	Container,
	Variant,
	Raw,
	HashedContainer,
	HashedVariant,
}

enum TagType : ushort
{
	Unknown,
	Object,
	Value,
	SubObject,
}

public class PropertyFile<KeyType> where KeyType : struct, IConvertible
{
	public Endian Endian { get; set; }
	public Stack<PropertyContainer<KeyType>> Containers = new();

	public PropertyFile()
	{
	}

	public bool Load(BinaryReader reader, PropertyContainer<KeyType> container)
	{
		Containers.Push(container);
		return ReadBinary(reader);
	}

	protected bool ReadBinary(BinaryReader reader)
	{
		if (reader.ReadUInt32(Endian.Big) != 0x50434242)
		{
			reader.BaseStream.Position -= 4;
			return ReadContiguousBinary(reader);
		}
		else
		{
			return ReadBlockBinary(reader);
		}
	}

	protected bool ReadContiguousBinary(BinaryReader reader)
	{
		while (!reader.IsEOS())
		{
			if (!ReadBinarySection(reader))
			{
				return false;
			}
		}

		return true;
	}

	protected bool ReadBlockBinary(BinaryReader reader)
	{
		// TODO: This format stores shit in a non-linear format with data offsets + lengths
		// Making this incredibly tedious and shitty since we'll need to make sure we're reading and seeking from the correct location
		return false;
	}

	protected bool ReadBinarySection(BinaryReader reader)
	{
		if (reader.Read(out byte section_count, Endian) == 0)
		{
			return false;
		}

		for (int i = 0; i < section_count; ++i)
		{
			if (reader.Read(out SectionType section_type, Endian) == 0)
			{
				return false;
			}

			if (reader.Read(out ushort entry_count, Endian) == 0)
			{
				return false;
			}

			bool success = false;

			switch (section_type)
			{
				case SectionType.Container:
					success = ReadBinaryContainers(reader, entry_count);
					break;
				case SectionType.Variant:
					success = ReadBinaryVariants(reader, entry_count);
					break;
				case SectionType.Raw:
					success = ReadBinaryRawData(reader, entry_count);
					break;
				case SectionType.HashedContainer:
					success = ReadBinaryHashedContainers(reader, entry_count);
					break;
				case SectionType.HashedVariant:
					success = ReadBinaryHashedVariants(reader, entry_count);
					break;
			}

			if (!success)
			{
				return false;
			}
		}

		return true;
	}

	protected bool ReadBinaryContainers(BinaryReader reader, ushort count)
	{
		PropertyContainer<KeyType> parent = Containers.Peek();

		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out string key, Endian) == 0)
			{
				return false;
			}

			Containers.Push(parent.CreateContainer(key));

			if (!ReadBinarySection(reader))
			{
				parent.Delete(key);
			}

			Containers.Pop();
		}

		return true;
	}

	protected bool ReadBinaryHashedContainers(BinaryReader reader, ushort count)
	{
		PropertyContainer<KeyType> parent = Containers.Peek();

		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out KeyType key, Endian) == 0)
			{
				return false;
			}

			Containers.Push(parent.CreateContainer(key));

			if (!ReadBinarySection(reader))
			{
				parent.Delete(key);
			}

			Containers.Pop();
		}

		return true;
	}

	protected bool ReadBinaryVariants(BinaryReader reader, ushort count)
	{
		PropertyContainer<KeyType> parent = Containers.Peek();

		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out string key, Endian) == 0)
			{
				return false;
			}

			if (ReadBinaryVariant(reader, out PropertyVariant variant))
			{
				parent.SetVariant(key, variant);
			}
		}

		return true;
	}

	protected bool ReadBinaryHashedVariants(BinaryReader reader, ushort count)
	{
		PropertyContainer<KeyType> parent = Containers.Peek();

		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out KeyType key, Endian) == 0)
			{
				return false;
			}

			if (ReadBinaryVariant(reader, out PropertyVariant variant))
			{
				parent.SetVariant(key, variant);
			}
		}

		return true;
	}

	protected bool ReadBinaryVariant(BinaryReader reader, out PropertyVariant variant)
	{
		if (reader.Read(out VariantType variant_type, Endian) == 0)
		{
			variant = default;
			return false;
		}

		switch (variant_type)
		{
			case VariantType.Integer:
				variant = new(reader.ReadInt32(Endian));
				break;
			case VariantType.Float:
				variant = new(reader.ReadSingle(Endian));
				break;
			case VariantType.String:
				variant = new(reader.ReadString(reader.Read<ushort>(Endian)));
				break;
			case VariantType.Vec2:
				variant = new(new Vec2(reader.Read<float>(2, Endian)));
				break;
			case VariantType.Vec3:
				variant = new(new Vec3(reader.Read<float>(3, Endian)));
				break;
			case VariantType.Vec4:
				variant = new(new Vec4(reader.Read<float>(4, Endian)));
				break;
			case VariantType.Mat3x3:
				variant = new(new Mat3x3(reader.Read<float>(3 * 3, Endian)));
				break;
			case VariantType.Mat3x4:
				// TODO: Convert 3x4 to 4x4?
				variant = new(new Mat3x4(reader.Read<float>(3 * 4, Endian)));
				break;
			case VariantType.VecInt:
				variant = new(reader.Read<int>(reader.Read<uint>(Endian), Endian));
				break;
			case VariantType.VecFloat:
				variant = new(reader.Read<float>(reader.Read<uint>(Endian), Endian));
				break;
			default:
				variant = default;
				return false;
		}

		return true;
	}

	protected bool ReadBinaryRawData(BinaryReader reader, ushort count)
	{
		// TODO: Do I need to support this?
		byte[] bytes = reader.ReadBytes(count);

		if ((bytes?.Length ?? 0) != count)
		{
			return false;
		}

		return true;
	}
}

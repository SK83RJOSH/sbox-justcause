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

public partial class PropertyContainer<KeyType> where KeyType : struct, IConvertible
{
	public Stack<PropertyContainer<KeyType>> Containers = new();

	public static bool Read(BinaryReader reader, out PropertyContainer<KeyType> container, Endian endian = Endian.Little)
	{
		if (reader.Read(out string magic, 4) == 4 && magic != "PCBB")
		{
			return ReadBlocked(reader, out container, endian);
		}
		else
		{
			reader.BaseStream.Position -= 4;
			return ReadContiguous(reader, out container, endian);
		}
	}

	private static bool ReadContiguous(BinaryReader reader, out PropertyContainer<KeyType> container, Endian endian)
	{
		container = new();

		while (!reader.IsEOS())
		{
			if (!ReadSection(reader, container, endian))
			{
				container = default;
				return false;
			}
		}

		return true;
	}

	protected static bool ReadBlocked(BinaryReader reader, out PropertyContainer<KeyType> container, Endian endian)
	{
		// TODO: This format stores shit in a non-linear format with data offsets + lengths
		// Making this incredibly tedious and shitty since we'll need to make sure we're reading and seeking from the correct location
		container = default;
		return false;
	}

	private static bool ReadSection(BinaryReader reader, PropertyContainer<KeyType> container, Endian endian)
	{
		if (reader.Read(out byte section_count, endian) == 0)
		{
			return false;
		}

		for (int i = 0; i < section_count; ++i)
		{
			if (reader.Read(out SectionType section_type, endian) == 0)
			{
				return false;
			}

			if (reader.Read(out ushort entry_count, endian) == 0)
			{
				return false;
			}

			bool success = false;

			switch (section_type)
			{
				case SectionType.Container:
					success = ReadContainers(reader, container, entry_count, endian);
					break;
				case SectionType.Variant:
					success = ReadVariants(reader, container, entry_count, endian);
					break;
				case SectionType.Raw:
					success = ReadRawData(reader, entry_count);
					break;
				case SectionType.HashedContainer:
					success = ReadHashedContainers(reader, container, entry_count, endian);
					break;
				case SectionType.HashedVariant:
					success = ReadHashedVariants(reader, container, entry_count, endian);
					break;
			}

			if (!success)
			{
				return false;
			}
		}

		return true;
	}

	private static bool ReadContainers(BinaryReader reader, PropertyContainer<KeyType> container, ushort count, Endian endian)
	{
		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out string key, endian) < sizeof(uint) + 1)
			{
				return false;
			}

			if (!ReadSection(reader, container.CreateContainer(key), endian))
			{
				container.Delete(key);
			}
		}

		return true;
	}

	private static bool ReadHashedContainers(BinaryReader reader, PropertyContainer<KeyType> container, ushort count, Endian endian)
	{
		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out KeyType key, endian) == 0)
			{
				return false;
			}

			if (!ReadSection(reader, container.CreateContainer(key), endian))
			{
				container.Delete(key);
			}
		}

		return true;
	}

	private static bool ReadVariants(BinaryReader reader, PropertyContainer<KeyType> container, ushort count, Endian endian)
	{
		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out string key, endian) == 0)
			{
				return false;
			}

			if (ReadVariant(reader, out PropertyVariant variant, endian))
			{
				container.SetVariant(key, variant);
			}
		}

		return true;
	}

	private static bool ReadHashedVariants(BinaryReader reader, PropertyContainer<KeyType> container, ushort count, Endian endian)
	{
		for (int i = 0; i < count; ++i)
		{
			if (reader.Read(out KeyType key, endian) == 0)
			{
				return false;
			}

			if (ReadVariant(reader, out PropertyVariant variant, endian))
			{
				container.SetVariant(key, variant);
			}
		}

		return true;
	}

	private static bool ReadVariant(BinaryReader reader, out PropertyVariant variant, Endian endian)
	{
		if (reader.Read(out VariantType variant_type, endian) == 0)
		{
			variant = default;
			return false;
		}

		switch (variant_type)
		{
			case VariantType.Integer:
				variant = new(reader.ReadInt32(endian));
				break;
			case VariantType.Float:
				variant = new(reader.ReadSingle(endian));
				break;
			case VariantType.String:
				variant = new(reader.ReadString(reader.Read<ushort>(endian)));
				break;
			case VariantType.Vec2:
				variant = new(new Vec2(reader.Read<float>(2, endian)));
				break;
			case VariantType.Vec3:
				variant = new(new Vec3(reader.Read<float>(3, endian)));
				break;
			case VariantType.Vec4:
				variant = new(new Vec4(reader.Read<float>(4, endian)));
				break;
			case VariantType.Mat3x3:
				variant = new(new Mat3x3(reader.Read<float>(3 * 3, endian)));
				break;
			case VariantType.Mat3x4:
				// TODO: Convert 3x4 to 4x4?
				variant = new(new Mat3x4(reader.Read<float>(3 * 4, endian)));
				break;
			case VariantType.VecInt:
				variant = new(reader.Read<int>(reader.Read<uint>(endian), endian));
				break;
			case VariantType.VecFloat:
				variant = new(reader.Read<float>(reader.Read<uint>(endian), endian));
				break;
			default:
				variant = default;
				return false;
		}

		return true;
	}

	private static bool ReadRawData(BinaryReader reader, ushort count)
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

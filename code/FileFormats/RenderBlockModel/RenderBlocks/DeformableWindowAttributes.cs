namespace JustCause.FileFormats.RenderBlockModel.RenderBlocks;

using JustCause.FileFormats.Utilities;
using System.IO;

[System.Flags]
public enum DeformableWindowFlags : uint
{

	DarkWindow = 1 << 0,
}

public struct DeformableWindowAttributes
{
	public DeformableWindowFlags Flags;

	public static bool Read(BinaryReader reader, out DeformableWindowAttributes attributes, Endian endian)
	{
		attributes = default;
		return reader.Read(out attributes.Flags, endian);
	}
}

public static partial class BinaryReaderExtensions
{
	public static bool Read(this BinaryReader reader, out DeformableWindowAttributes attributes, Endian endian = default)
	{
		return DeformableWindowAttributes.Read(reader, out attributes, endian);
	}
}

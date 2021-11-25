namespace JustCause.FileFormats.SmallArchive;

using JustCause.FileFormats.Utilities;
using System.Collections.Generic;
using System.IO;

public partial class SmallArchive : IBinaryReader<SmallArchive>
{
	public partial class File : IBinaryReader<File>
	{
		public static bool Read(BinaryReader reader, out File file, Endian endian = Endian.Little)
		{
			file = default;

			if (reader.Read(out string name, endian) < sizeof(uint) + 1)
			{
				return false;
			}

			if (reader.Read(out uint offset, endian) < sizeof(uint))
			{
				return false;
			}

			if (reader.Read(out uint size, endian) < sizeof(uint))
			{
				return false;
			}

			file = new(name, offset, size);
			return true;
		}
	}

	public static bool Read(BinaryReader reader, out SmallArchive archive, Endian endian = Endian.Little)
	{
		archive = default;

		if (reader.Read(out uint magic_length, Endian.Little) < sizeof(uint))
		{
			return false;
		}

		// invalid SARC magic length
		if (magic_length != 4 && magic_length != 67108864)
		{
			return false;
		}

		endian = magic_length == 4 ? Endian.Little : Endian.Big;

		// invalid SARC magic
		if (reader.Read(out string magic, 4) < 4 || magic != "SARC")
		{
			return false;
		}

		// unsupported SARC version
		if (reader.Read(out uint version, endian) < sizeof(uint) || version < 1 || version > 2)
		{
			return false;
		}

		uint buffer_size = reader.ReadUInt32();
		long buffer_start = reader.BaseStream.Position;
		Dictionary<uint, File> files = new();

		while (reader.BaseStream.Position < buffer_start + buffer_size - 15)
		{
			if (!File.Read(reader, out File file, endian))
			{
				return false;
			}

			files.Add(file.Name.HashJenkins(), file);
		}

		reader.BaseStream.Seek(0, SeekOrigin.Begin);

		// Invalid stream length
		if (reader.BaseStream.Length > int.MaxValue)
		{
			return false;
		}

		byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);

		archive = new(files, data);
		return true;
	}
}

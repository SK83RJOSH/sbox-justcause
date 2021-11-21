namespace JustCause.FileFormats.SmallArchive;

using JustCause.FileFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class SmallArchiveFile
{
	public class Entry : IBinaryFormat
	{
		public string Name;
		public uint Offset;
		public uint Size;

		public void Deserialize(BinaryReader reader, Endian endian)
		{
			Name = reader.ReadString(Encoding.ASCII, endian);
			Offset = reader.ReadUInt32(endian);
			Size = reader.ReadUInt32(endian);

			long previous_position = reader.BaseStream.Position;
			reader.BaseStream.Position = Offset;
			reader.BaseStream.Position = previous_position;
		}
	}

	public Endian Endian;
	public uint Version;
	public List<Entry> Entries = new();

	public void Deserialize(BinaryReader reader)
	{
		uint magic_length = reader.ReadUInt32(Endian.Little);

		if (magic_length != 4 && magic_length != 67108864)
		{
			throw new FormatException("invalid SARC magic length");
		}

		Endian = magic_length == 4 ? Endian.Little : Endian.Big;

		string magic = reader.ReadString(4, Encoding.ASCII);

		if (magic != "SARC")
		{
			throw new FormatException("invalid SARC magic");
		}

		Version = reader.ReadUInt32(Endian);

		if (Version < 1 || Version > 2)
		{
			throw new FormatException("unsupported SARC version");
		}

		uint buffer_size = reader.ReadUInt32();
		long buffer_start = reader.BaseStream.Position;

		Entries.Clear();

		while (reader.BaseStream.Position < buffer_start + buffer_size - 15)
		{
			Entry entry = new Entry();
			entry.Deserialize(reader, Endian);
			Entries.Add(entry);
		}
	}
}

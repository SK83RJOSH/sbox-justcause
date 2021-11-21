namespace JustCause.FileFormats.SmallArchive;

using System;
using System.Collections.Generic;
using System.IO;

public class SmallArchive
{
	private SmallArchiveFile File = new();
	private byte[] Data = Array.Empty<byte>();
	private Dictionary<string, SmallArchiveFile.Entry> Entries = new();

	public MemoryStream this[string name] => GetFile(name);
	public bool ContainsFile(string name) => Entries.ContainsKey(name);

	public SmallArchive(Stream stream)
	{
		BinaryReader reader = new(stream);

		stream.Seek(0, SeekOrigin.Begin);
		File.Deserialize(reader);

		stream.Seek(0, SeekOrigin.Begin);
		Data = reader.ReadBytes((int)stream.Length);

		foreach (SmallArchiveFile.Entry entry in File.Entries)
		{
			if (Entries.ContainsKey(entry.Name))
			{
				continue;
			}

			Entries.Add(entry.Name, entry);
		}
	}

	public MemoryStream GetFile(string name)
	{
		if (ContainsFile(name))
		{
			SmallArchiveFile.Entry entry = Entries[name];
			return new MemoryStream(Data, (int)entry.Offset, (int)entry.Size, false);
		}

		return null;
	}

	public bool TryGetFile(string name, out MemoryStream value)
	{
		value = GetFile(name);
		return value != null;
	}
}

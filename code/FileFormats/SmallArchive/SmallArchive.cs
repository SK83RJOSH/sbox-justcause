namespace JustCause.FileFormats.SmallArchive;

using JustCause.FileFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

public partial class SmallArchive
{
	public partial class File
	{
		public string Name { get; private set; }
		public uint Offset { get; private set; }
		public uint Size { get; private set; }

		public File(string name, uint offset, uint size)
		{
			Name = name;
			Offset = offset;
			Size = size;
		}
	}

	private Dictionary<uint, File> Files = new();
	private byte[] Data = Array.Empty<byte>();

	public MemoryStream this[uint key] => GetFile(key);
	public MemoryStream this[string name] => GetFile(name);

	public bool ContainsFile(uint key) => Files.ContainsKey(key);
	public bool ContainsFile(string name) => Files.ContainsKey(name.HashJenkins());

	public MemoryStream GetFile(string name) => GetFile(name.HashJenkins());
	public bool TryGetFile(string name, out MemoryStream value) => TryGetFile(name.HashJenkins(), out value);

	public SmallArchive(Dictionary<uint, File> files, byte[] data)
	{
		Files = files;
		Data = data;
	}

	public MemoryStream GetFile(uint key)
	{
		if (ContainsFile(key))
		{
			File file = Files[key];
			return new MemoryStream(Data, (int)file.Offset, (int)file.Size, false);
		}

		return null;
	}

	public bool TryGetFile(uint name, out MemoryStream value)
	{
		value = GetFile(name);
		return value != null;
	}
}

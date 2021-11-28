namespace JustCause.Entities;

using JustCause.FileFormats.Utilities;
using JustCause.Resources;
using Sandbox;
using System.Collections.Generic;
using System.IO;
using System.Text;

[Library]
public partial class RenderBlockModel : AnimEntity
{
	[Net]
	public string ArchivePath { get; private set; }

	[Net]
	public string ArchiveFileName { get; private set; }

	private ResourceLoader ResourceLoader = new();

	public RenderBlockModel()
	{

	}

	public RenderBlockModel(string archive_path, string archive_file_name)
	{
		ArchivePath = archive_path;
		ArchiveFileName = archive_file_name;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		ResourceLoader.LoadArchive("assets\\general.blz");
		var archive = ResourceLoader.LoadArchive(ArchivePath);

		if (!archive.TryGetFile(ArchiveFileName, out MemoryStream stream))
		{
			return;
		}

		List<byte> characters = new();
		BinaryReader reader = new(stream);

		while (reader.Read(out byte character) && (character != '\r' || character == '\n'))
		{
			characters.Add(character);
		}

		string lod = Encoding.ASCII.GetString(characters.ToArray());

		if (lod != "")
		{
			string[] parts = lod.Split('\\');

			if (ResourceLoader.LoadModel(parts[parts.Length - 1].ToLower(), out Model model))
			{
				SetModel(model);
			}
		}
	}
}

namespace JustCause.Entities;

using JustCause.FileFormats.Physics;
using JustCause.FileFormats.Utilities;
using JustCause.Resources;
using Sandbox;
using System.IO;

[Library]
public partial class RenderBlockModel : AnimEntity
{
	[Net]
	public string ArchivePath { get; private set; }

	[Net]
	public string LodFileName { get; private set; }

	[Net]
	public string PfxFileName { get; private set; }

	private ResourceLoader ResourceLoader = new();

	public RenderBlockModel()
	{

	}

	public RenderBlockModel(string archive_path, string lod_file_name, string pfx_file_name)
	{
		ArchivePath = archive_path;
		LodFileName = lod_file_name;
		PfxFileName = pfx_file_name;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		ResourceLoader.LoadArchive("assets\\general.blz");
		var archive = ResourceLoader.LoadArchive(ArchivePath);

		if (!archive.TryGetFile(LodFileName, out MemoryStream lod_stream))
		{
			return;
		}

		BinaryReader lod_reader = new(lod_stream);

		if (!lod_reader.Read(out string lod, -1))
		{
			return;
		}

		lod = lod.Split('\r')[0];

		if (lod != "")
		{
			string[] parts = lod.Split('\\');

			if (ResourceLoader.LoadModel(parts[parts.Length - 1].ToLower(), out Model model))
			{
				SetModel(model);
			}
		}

		if (PfxFileName?.Length == 0 || !archive.TryGetFile(PfxFileName, out MemoryStream pfx_stream))
		{
			return;
		}


		BinaryReader pfx_reader = new(pfx_stream);

		if (!PfxHeader.Read(pfx_reader, out PfxHeader header, default))
		{
			return;
		}

		Log.Info(header.Type);

		if (!Packfile.Read(pfx_reader, out Packfile packfile))
		{
			return;
		}

		Log.Info(packfile.Header.ContentsVersion);
	}
}

namespace JustCause.Entities;

using JustCause.Resources;
using JustCause.FileFormats.PropertyContainer;
using Sandbox;
using System.IO;
using PropertyContainer = FileFormats.PropertyContainer.PropertyContainer<uint>;
using PropertyFile = FileFormats.PropertyContainer.PropertyFile<uint>;

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
		SetModel(ResourceLoader.LoadModel(ArchiveFileName));

		if (archive.TryGetFile("v005_tuktuk_civ.mvdoll"/*ee.epe*/, out MemoryStream prop_stream))
		{
			PropertyFile file = new();
			PropertyContainer container = new();

			if (file.Load(new BinaryReader(prop_stream), container))
			{
				if (!container.GetContainer("_vdoll", out PropertyContainer vdoll))
				{
					return;
				}

				if (!vdoll.GetContainer("_parts", out PropertyContainer parts))
				{
					return;
				}

				foreach (PropertyContainer part in parts.GetContainers())
				{
					if (part.GetValue("model_shrt", out string model_name))
					{
						Log.Info($"Model Name: {model_name}");
					}

					foreach (PropertyContainer child_part in part.GetContainers())
					{
						if (child_part.GetValue("model_shrt", out string child_model_name))
						{
							Log.Info($"Model Name: {child_model_name}");
						}
					}
				}

				Log.Info("Success!");
			}
		}
	}
}

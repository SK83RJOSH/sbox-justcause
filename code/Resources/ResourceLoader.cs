namespace JustCause.Resources;

using JustCause.FileFormats.SmallArchive;
using Sandbox;
using System.Collections.Generic;

public class ResourceLoader
{
	private static Dictionary<string, Model> LoadedModels = new();
	private static Dictionary<string, SmallArchive> LoadedArchives = new();
	private static Dictionary<string, Texture> LoadedTextures = new();

	~ResourceLoader()
	{
		foreach (string path in LoadedModels.Keys)
		{
			ResourceManager.ReleaseModel(path);
		}

		foreach (string path in LoadedArchives.Keys)
		{
			ResourceManager.ReleaseArchive(path);
		}

		foreach (string path in LoadedTextures.Keys)
		{
			ResourceManager.ReleaseTexture(path);
		}
	}

	public Model LoadModel(string path)
	{
		LoadModel(path, out Model model);
		return model;
	}

	public SmallArchive LoadArchive(string path)
	{
		LoadArchive(path, out SmallArchive archive);
		return archive;
	}

	public Texture LoadTexture(string path)
	{
		LoadTexture(path, out Texture texture);
		return texture;
	}

	public bool LoadModel(string path, out Model model)
	{
		if (ResourceManager.LoadModel(path, out model))
		{
			LoadedModels[path] = model;
			return true;
		}

		model = default;
		return false;
	}

	public bool LoadArchive(string path, out SmallArchive archive)
	{
		if (ResourceManager.LoadArchive(path, out archive))
		{
			LoadedArchives[path] = archive;
			return true;
		}

		archive = default;
		return false;
	}

	public bool LoadTexture(string path, out Texture texture)
	{
		if (ResourceManager.LoadTexture(path, out texture))
		{
			LoadedTextures[path] = texture;
			return true;
		}

		texture = default;
		return false;
	}
}

namespace JustCause.Resources;

using JustCause.FileFormats.DirectDrawSurface;
using JustCause.FileFormats.RenderBlockModel;
using JustCause.FileFormats.RenderBlockModel.RenderBlocks;
using JustCause.FileFormats.RenderBlockModel.Utilities;
using JustCause.FileFormats.SmallArchive;
using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using JustCauseMaterial = FileFormats.RenderBlockModel.DataTypes.Material;
using JustCausePrimitiveType = FileFormats.RenderBlockModel.DataTypes.PrimitiveType;

public static class ResourceManager
{
	public static readonly bool DisableCache = true;

	private static Dictionary<string, Model> LoadedModels = new();
	private static Dictionary<string, SmallArchive> LoadedArchives = new();
	private static Dictionary<string, Texture> LoadedTextures = new();

	public static Model LoadModel(string path)
	{
		LoadModel(path, out Model model);
		return model;
	}

	public static SmallArchive LoadArchive(string path)
	{
		LoadArchive(path, out SmallArchive archive);
		return archive;
	}

	public static Texture LoadTexture(string path)
	{
		LoadTexture(path, out Texture texture);
		return texture;
	}

	private static Material CreateMaterial(string path, ref JustCauseMaterial material)
	{
		Material result = Material.Load(path).CreateCopy();

		if (LoadTexture(material.Texture0, out Texture diffuse_texture))
		{
			result.OverrideTexture("Color", diffuse_texture);
		}

		if (LoadTexture(material.Texture0, out Texture normal_texture))
		{
			result.OverrideTexture("Normal", normal_texture);
		}

		if (LoadTexture(material.Texture0, out Texture properties_texture))
		{
			result.OverrideTexture("Properties", properties_texture);
		}

		return result;
	}

	private static MeshPrimitiveType GetPrimitiveType(ref JustCauseMaterial material)
	{
		switch (material.PrimitiveType)
		{
			case JustCausePrimitiveType.PointSprite:
			case JustCausePrimitiveType.IndexedPointSprite:
				return MeshPrimitiveType.Points;
			case JustCausePrimitiveType.TriangleList:
			case JustCausePrimitiveType.IndexedTriangleList:
				return MeshPrimitiveType.Triangles;
			case JustCausePrimitiveType.TriangleStrip:
			case JustCausePrimitiveType.IndexedTriangleStrip:
				return MeshPrimitiveType.TriangleStrip;
			case JustCausePrimitiveType.LineList:
				return MeshPrimitiveType.Lines;
		}

		throw new Exception($"Invalid {nameof(JustCausePrimitiveType)}: {material.PrimitiveType}");
	}

	public static bool LoadModel(string path, out Model model)
	{
		if (LoadedModels.ContainsKey(path) && !DisableCache)
		{
			model = LoadedModels[path];
			return true;
		}

		RenderBlockModelFile rbm_file = null;

		foreach (SmallArchive archive in LoadedArchives.Values)
		{
			if (archive.TryGetFile(path, out MemoryStream rbm_stream))
			{
				rbm_file = new RenderBlockModelFile();
				rbm_file.Deserialize(new BinaryReader(rbm_stream));
				break;
			}
		}

		if (rbm_file == null)
		{
			model = default;
			return false;
		}

		List<Mesh> meshes = new List<Mesh>();

		foreach (IRenderBlock block in rbm_file.Blocks)
		{
			Mesh mesh = new();
			Material material = null;
			MeshPrimitiveType primitive_type = MeshPrimitiveType.Triangles;
			VertexBuffer vertex_buffer = new();
			int vertex_count = 0;

			vertex_buffer.Init(true);

			if (block is CarPaint car_paint)
			{
				material = CreateMaterial("materials/general.vmat", ref car_paint.Material);
				primitive_type = GetPrimitiveType(ref car_paint.Material);

				foreach (var car_paint_vertex in car_paint.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = car_paint_vertex.Position.AsSourceCoord();
					vertex.Normal = car_paint_vertex.Normal.AsSourceNorm();
					vertex.Tangent = new Vector4(car_paint_vertex.Tangent.AsSourceNorm(), MathF.Sign(car_paint_vertex.Tangent.Packed));
					vertex.Color = Color.White;
					vertex.TexCoord0 = new Vector2(car_paint_vertex.UVL.X, car_paint_vertex.UVL.Y);
					vertex_buffer.Add(vertex);
					vertex_count++;
				}

				foreach (short index in car_paint.Indices)
				{
					vertex_buffer.AddRawIndex(index);
				}
			}
			else if (block is CarPaintSimple car_paint_simple)
			{
				material = CreateMaterial("materials/general.vmat", ref car_paint_simple.Material);
				primitive_type = GetPrimitiveType(ref car_paint_simple.Material);

				foreach (var car_paint_simple_vertex in car_paint_simple.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = car_paint_simple_vertex.Position.AsSourceCoord();
					vertex.Normal = car_paint_simple_vertex.Normal.AsSourceCoord();
					vertex.Tangent = new Vector4(car_paint_simple_vertex.Tangent.AsSourceCoord(), 1f);
					vertex.Color = Color.White;
					vertex.TexCoord0 = car_paint_simple_vertex.UV.AsSource();
					vertex_buffer.Add(vertex);
					vertex_count++;
				}

				foreach (short index in car_paint_simple.Indices)
				{
					vertex_buffer.AddRawIndex(index);
				}
			}
			else if (block is DeformableWindow deformable_window)
			{
				material = CreateMaterial("materials/general.vmat", ref deformable_window.Material);
				primitive_type = GetPrimitiveType(ref deformable_window.Material);

				foreach (var deformable_vertex in deformable_window.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = deformable_vertex.Position.AsSourceCoord();
					vertex.Normal = deformable_vertex.Normal.AsSourceCoord();
					vertex.Tangent = new Vector4(deformable_vertex.Tangent.AsSourceCoord(), 1f);
					vertex.Color = Color.Black * 0.5f;
					vertex.TexCoord0 = deformable_vertex.UV.AsSource();
					vertex_buffer.Add(vertex);
					vertex_count++;
				}

				foreach (short index in deformable_window.Indices)
				{
					vertex_buffer.AddRawIndex(index);
				}
			}
			else if (block is General general)
			{
				material = CreateMaterial("materials/general.vmat", ref general.Material);
				primitive_type = GetPrimitiveType(ref general.Material);

				foreach (var general_vertex in general.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = general_vertex.Position.AsSourceCoord() * general.Attributes.VertexInfo.Scale;
					vertex.Normal = general_vertex.Normal.AsSourceCoord();
					vertex.Tangent = new Vector4(general_vertex.Tangent.AsSourceCoord(), 1f);
					vertex.Color = general_vertex.Color.AsSource();
					vertex.TexCoord0 = general_vertex.UVs[0].AsSource();
					vertex.TexCoord1 = general_vertex.UVs[1].AsSource();
					vertex_buffer.Add(vertex);
					vertex_count++;
				}

				foreach (short index in general.Indices)
				{
					vertex_buffer.AddRawIndex(index);
				}
			}
			else if (block is Lambert lambert)
			{
				material = CreateMaterial("materials/general.vmat", ref lambert.Material);
				primitive_type = GetPrimitiveType(ref lambert.Material);

				foreach (var general_vertex in lambert.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = general_vertex.Position.AsSourceCoord() * lambert.Attributes.VertexInfo.Scale;
					vertex.Normal = general_vertex.Normal.AsSourceCoord();
					vertex.Tangent = new Vector4(general_vertex.Tangent.AsSourceCoord(), 1f);
					vertex.Color = general_vertex.Color.AsSource();
					vertex.TexCoord0 = general_vertex.UVs[0].AsSource();
					vertex.TexCoord1 = general_vertex.UVs[1].AsSource();
					vertex_buffer.Add(vertex);
					vertex_count++;
				}

				foreach (short index in lambert.Indices)
				{
					vertex_buffer.AddRawIndex(index);
				}
			}

			if (material != null && vertex_count > 0)
			{
				mesh.Material = material;
				mesh.PrimitiveType = primitive_type;
				mesh.Bounds = new BBox(rbm_file.Min.AsSourceCoord(), rbm_file.Max.AsSourceCoord());
				mesh.CreateBuffers(vertex_buffer, false);
				meshes.Add(mesh);
			}
		}

		model = Model.Builder.AddMeshes(meshes.ToArray()).Create();

		if (!DisableCache)
		{
			LoadedModels[path] = model;
		}

		return true;
	}

	public static bool LoadArchive(string path, out SmallArchive archive)
	{
		if (LoadedArchives.ContainsKey(path))
		{
			archive = LoadedArchives[path];
			return true;
		}

		if (!FileSystem.Mounted.FileExists(path))
		{
			archive = default;
			return false;
		}

		archive = new SmallArchive(FileSystem.Mounted.OpenRead(path));
		
		LoadedArchives[path] = archive;
		return true;
	}

	public static bool LoadTexture(string path, out Texture texture)
	{
		Dds dds_file = null;

		if (LoadedTextures.ContainsKey(path))
		{
			texture = LoadedTextures[path];
			return true;
		}

		foreach (SmallArchive archive in LoadedArchives.Values)
		{
			if (archive.TryGetFile(path, out MemoryStream texture_stream))
			{
				dds_file = Dds.Create(texture_stream);
				break;
			}
		}

		if (dds_file == null)
		{
			texture = default;
			return false;
		}

		var mip_level = dds_file.MipMaps[0];
		byte[] data = new byte[mip_level.DataLen];

		for (int i = 0; i < mip_level.DataLen; i++)
		{
			data[i] = dds_file.Data[mip_level.DataOffset + i];
		}

		texture = Texture.Create(mip_level.Width, mip_level.Height, ImageFormat.RGBA8888).WithData(data).Finish();

		if (!DisableCache)
		{
			LoadedTextures[path] = texture;
		}

		return true;
	}

	public static bool ReleaseModel(string path)
	{
		return LoadedModels.Remove(path);
	}

	public static bool ReleaseArchive(string path)
	{
		return LoadedArchives.Remove(path);
	}

	public static bool ReleaseTexture(string path)
	{
		return LoadedTextures.Remove(path);
	}
}

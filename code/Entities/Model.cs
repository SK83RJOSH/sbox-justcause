namespace JustCause.Entities;

using JustCause.RenderBlockModel.Utilities;
using Sandbox;
using System.Collections.Generic;
using System.IO;

[Library]
public partial class RenderBlockEntity : AnimEntity
{
	[Net]
	public string Path { get; private set; }

	private List<Mesh> Meshes = new();

	public RenderBlockEntity()
	{

	}

	public RenderBlockEntity(string path)
	{
		Path = path;
	}

	internal Material CreateMaterial(string path, ref RenderBlockModel.Common.Material material)
	{
		Material result = Material.Load(path).CreateCopy();
		result.OverrideTexture("Color", Texture.Load("assets/" + material.Texture0.Replace("dds", "png")));
		result.OverrideTexture("Normal", Texture.Load("assets/" + material.Texture1.Replace("dds", "png")));
		result.OverrideTexture("Properties", Texture.Load("assets/" + material.Texture2.Replace("dds", "png")));
		return result;
	}

	internal MeshPrimitiveType GetPrimitiveType(ref RenderBlockModel.Common.Material material)
	{
		switch (material.PrimitiveType)
		{
			case RenderBlockModel.Common.PrimitiveType.PointSprite:
			case RenderBlockModel.Common.PrimitiveType.IndexedPointSprite:
				return MeshPrimitiveType.Points;
			case RenderBlockModel.Common.PrimitiveType.TriangleList:
			case RenderBlockModel.Common.PrimitiveType.IndexedTriangleList:
				return MeshPrimitiveType.Triangles;
			case RenderBlockModel.Common.PrimitiveType.TriangleStrip:
			case RenderBlockModel.Common.PrimitiveType.IndexedTriangleStrip:
				return MeshPrimitiveType.TriangleStrip;
			case RenderBlockModel.Common.PrimitiveType.LineList:
				return MeshPrimitiveType.Lines;
		}

		throw new System.Exception($"Invalid {nameof(RenderBlockModel.Common.PrimitiveType)}: {material.PrimitiveType}");
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		using BinaryReader reader = new(FileSystem.Mounted.OpenRead(Path, FileMode.Open));
		RenderBlockModel.ModelFile rbm_file = new();
		rbm_file.Deserialize(reader);

		Log.Info($"Path: {Path}, Position: {Position}, Blocks: {rbm_file.Blocks.Count}");

		foreach (IRenderBlock block in rbm_file.Blocks)
		{
			Mesh mesh = new();
			Material material = null;
			MeshPrimitiveType primitive_type = MeshPrimitiveType.Triangles;
			VertexBuffer vertex_buffer = new();
			int vertex_count = 0;

			vertex_buffer.Init(true);

			if (block is RenderBlockModel.RenderBlock.CarPaint car_paint)
			{
				material = CreateMaterial("materials/general.vmat", ref car_paint.Material);
				primitive_type = GetPrimitiveType(ref car_paint.Material);

				foreach (var car_paint_vertex in car_paint.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = car_paint_vertex.Position.AsSourcePosition();
					vertex.Normal = car_paint_vertex.Normal.AsSource();
					vertex.Tangent = new Vector4(car_paint_vertex.Tangent.AsSource(), 1f);
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
			else if (block is RenderBlockModel.RenderBlock.CarPaintSimple car_paint_simple)
			{
				material = CreateMaterial("materials/general.vmat", ref car_paint_simple.Material);
				primitive_type = GetPrimitiveType(ref car_paint_simple.Material);

				foreach (var car_paint_simple_vertex in car_paint_simple.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = car_paint_simple_vertex.Position.AsSourcePosition();
					vertex.Normal = car_paint_simple_vertex.Normal.AsSource();
					vertex.Tangent = new Vector4(car_paint_simple_vertex.Tangent.AsSource(), 1f);
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
			else if (block is RenderBlockModel.RenderBlock.DeformableWindow deformable_window)
			{
				material = CreateMaterial("materials/general.vmat", ref deformable_window.Material);
				primitive_type = GetPrimitiveType(ref deformable_window.Material);

				foreach (var deformable_vertex in deformable_window.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = deformable_vertex.Position.AsSourcePosition();
					vertex.Normal = deformable_vertex.Normal.AsSource();
					vertex.Tangent = new Vector4(deformable_vertex.Tangent.AsSource(), 1f);
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
			else if (block is RenderBlockModel.RenderBlock.General general)
			{
				material = CreateMaterial("materials/general.vmat", ref general.Material);
				primitive_type = GetPrimitiveType(ref general.Material);

				foreach (var general_vertex in general.Vertices)
				{
					Vertex vertex = default;
					vertex.Position = general_vertex.Position.AsSourcePosition() * general.Attributes.Scale;
					vertex.Normal = general_vertex.Normal.AsSource();
					vertex.Tangent = new Vector4(general_vertex.Tangent.AsSource(), 1f);
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

			if (material != null && vertex_count > 0)
			{
				mesh.Material = material;
				mesh.PrimitiveType = primitive_type;
				mesh.Bounds = new BBox(rbm_file.Min.AsSourcePosition(), rbm_file.Max.AsSourcePosition());
				mesh.CreateBuffers(vertex_buffer, false);
				Meshes.Add(mesh);
			}
		}

		SetModel(Model.Builder.AddMeshes(Meshes.ToArray()).Create());
	}
}

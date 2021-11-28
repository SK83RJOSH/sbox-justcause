namespace JustCause.FileFormats.RenderBlockModel;

using JustCause.FileFormats.Utilities;
using JustCause.FileFormats.RenderBlockModel.RenderBlocks;
using System.Collections.Generic;
using System.IO;
using System;

public static class BlockTypeFactory
{
	private static readonly Dictionary<uint, string> HashesToNames = new();

	public static string GetName(uint type)
	{
		if (HashesToNames.ContainsKey(type))
		{
			return HashesToNames[type];
		}

		return null;
	}

	private static bool Read(BinaryReader reader, out IRenderBlock render_block, uint type, Endian endian)
	{
		render_block = null;

		string name = GetName(type);

		if (name == null)
		{
			return false;
		}

		switch (name)
		{
			case "CarPaint":
				{ return reader.Read(out CarPaint block, endian) && (render_block = block) != null; }
			case "CarPaintSimple":
				{ return reader.Read(out CarPaintSimple block, endian) && (render_block = block) != null; }
			case "DeformableWindow":
				{ return reader.Read(out DeformableWindow block, endian) && (render_block = block) != null; }
			case "General":
				{ return reader.Read(out General block, endian) && (render_block = block) != null; }
			case "Halo":
				{ return reader.Read(out Halo block, endian) && (render_block = block) != null; }
			case "Lambert":
				{ return reader.Read(out Lambert block, endian) && (render_block = block) != null; }
			default:
				throw new NotSupportedException($"unhandled block type {name} (0x{type:X8})");
		}
	}

	private static bool VerifyBlockFooter(BinaryReader reader, Endian endian)
	{
		if (!reader.Read(out uint footer, endian) || footer != 0x89ABCDEF)
		{
			throw new FormatException("invalid block footer (data corrupt or misread)");
		}

		return true;
	}

	public static bool Read(BinaryReader reader, out IRenderBlock render_block, Endian endian)
	{
		render_block = default;

		if (!reader.Read(out uint type, endian))
		{
			return false;
		}

		return Read(reader, out render_block, type, endian) && VerifyBlockFooter(reader, endian);
	}

	static BlockTypeFactory()
	{
		string[] Names = new[]
		{
			"2DTex1",
			"2DTex2",
			"3DText",
			"AOBox",
			"Beam",
			"BillboardFoliage",
			"Box",
			"Bullet",
			"CarPaint",
			"CarPaintSimple",
			"CirrusClouds",
			"Clouds",
			"Creatures",
			"DecalDeformable",
			"DecalSimple",
			"DecalSkinned",
			"DeformableWindow",
			"Facade",
			"Flag",
			"FogGradient",
			"Font",
			"General",
			"Grass",
			"GuiAnark",
			"Halo",
			"Lambert",
			"Leaves",
			"Lights",
			"Line",
			"Merged",
			"NvWaterHighEnd",
			"Occluder",
			"Open",
			"Particle",
			"Skidmarks",
			"SkinnedGeneral",
			"SkyGradient",
			"SoftClouds",
			"SplineRoad",
			"Stars",
			"Terrain",
			"TerrainForest",
			"TerrainForestFin",
			"TreeImpostorTrunk",
			"TreeImpostorTop",
			"Triangle",
			"VegetationBark",
			"VegetationFoliage",
			"WaterGodrays",
			"WaterHighEnd",
			"WaterWaves",
			"Weather",
			"Window",
		};

		foreach (string Name in Names)
		{
			HashesToNames.Add(Name.HashJenkins(), Name);
		}
	}
}

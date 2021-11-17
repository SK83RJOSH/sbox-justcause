namespace JustCause.RenderBlockModel;

using JustCause.RenderBlockModel.Utilities;
using System;
using System.Collections.Generic;

public static class BlockTypeFactory
{
	private static readonly Dictionary<uint, string> HashesToNames = new();
	private static readonly Dictionary<uint, Type> HashesToTypes = new();

	private static void Register<T>(string name) where T : IRenderBlock
	{
		HashesToTypes.Add(name.HashJenkins(), typeof(T));
	}

	public static string GetName(uint type)
	{
		if (HashesToNames.ContainsKey(type))
		{
			return HashesToNames[type];
		}

		return null;
	}

	public static IRenderBlock Create(uint type)
	{
		if (HashesToTypes.ContainsKey(type))
		{
			return Sandbox.Library.Create<object>(HashesToTypes[type]) as IRenderBlock;
		}

		return null;
	}

	public static IRenderBlock Create(string type)
	{
		return Create(type.HashJenkins());
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

		Register<RenderBlock.CarPaint>("CarPaint");
		Register<RenderBlock.CarPaintSimple>("CarPaintSimple");
		Register<RenderBlock.DeformableWindow>("DeformableWindow");
		Register<RenderBlock.General>("General");
		Register<RenderBlock.Halo>("Halo");
	}
}

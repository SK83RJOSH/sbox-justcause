namespace JustCause.Resources.PaperDoll;

using JustCause.Entities;
using JustCause.FileFormats.PropertyContainer;
using System;
using System.Collections.Generic;
using PropertyContainer = FileFormats.PropertyContainer.PropertyContainer<uint>;

public enum VehicleUpgrade
{
	Acceleration,
	Armor,
	Handling,
	TopSpeed,
	Weapon,
}

public sealed class VehicleRuleset
{
	private List<string> Rules = new();

	public IEnumerable<string> GetRules() => Rules;

	public void AddRule(VehicleUpgrade upgrade, byte value)
	{
		Rules.Add(upgrade + "Upgrade" + value);
	}

	public void AddRule(string spawnset)
	{
		Rules.Add(spawnset);
	}

	public bool Empty()
	{
		return Rules.Count == 0;
	}
}

public enum VehicleType
{
	Car,
	Helicopter,
	Boat,
	Airplane,
	Motorcycle,
}

public struct ResourceDescription
{
	public uint Id;
	public string ModelPath;
	public string PfxPath;
	public int MaxHealth;
	public int DamageHealth;
	public int BreakAt;
	public bool ShouldBreak;
	public bool AlwaysEnabled;
	public bool ShouldExplode;
	public Mat3x4 World;
}

public struct WheelInfo
{
	public bool IsWheel;
	public bool IsSuspension;
	public bool IsStaticSuspension;
	public Vec3 Scale;
	public int Slot;
}

public struct EffectInfo
{
	public int ID;
	public int Usage;
	public Mat3x4 World;
}

public struct RotorInfo
{
	public bool IsRotor;
	public int Slot;
	public int RotorPart;
}
public struct SteeringInfo
{
	public bool IsSteering;
	public bool IsUpperSteering;
}

public struct VehicleSettings
{
	public bool IsUsed;
	public bool IsCabbed;
	public string DisplayName;
}

public sealed class VehiclePart
{
	public uint GUID;
	public string Name;
	public string Type;
	public bool IsReference;
	public bool SkinToParent;
	public VehiclePart Parent;
	public ResourceDescription ResourceDescription;
	public PropertyContainer Modules;
	public PropertyContainer SmartObjects;
	public WheelInfo WheelInfo;
	public RotorInfo RotorInfo;
	public SteeringInfo SteeringInfo;
	public List<VehiclePart> Children = new();
	public List<EffectInfo> Effects = new();

	public bool IsValid()
	{
		return
			WheelInfo.IsWheel || WheelInfo.IsSuspension || WheelInfo.IsStaticSuspension ||
			RotorInfo.IsRotor || SteeringInfo.IsSteering || SteeringInfo.IsUpperSteering;
	}

	public bool ReadModules()
	{
		if (Modules == null)
		{
			return false;
		}

		foreach (PropertyContainer module in Modules.GetContainers())
		{
			if (!module.GetValue("_class", out string class_name) || class_name != "CPartDamageModule")
			{
				continue;
			}

			module.GetValue("max_health", out ResourceDescription.MaxHealth);
			module.GetValue("damage_health", out ResourceDescription.DamageHealth);
			module.GetValue("break_at", out ResourceDescription.BreakAt);
			module.GetValue("always_enabled", out ResourceDescription.AlwaysEnabled);
			module.GetValue("should_break", out ResourceDescription.ShouldBreak);
			module.GetValue("should_explode", out ResourceDescription.ShouldExplode);
		}

		return true;
	}

	public bool ReadEffects(PropertyContainer effects)
	{
		if (effects == null)
		{
			return false;
		}

		Effects.Capacity = effects.GetContainerCount();

		EffectInfo effect_info;

		foreach (PropertyContainer effect in effects.GetContainers())
		{
			if (!effect.GetValue("_class", out string class_name) || class_name != "CEffectProp")
			{
				continue;
			}

			effect.GetValue("effect", out effect_info.ID);
			effect.GetValue("usage", out effect_info.Usage);
			effect.GetValue("world", out effect_info.World);

			Effects.Add(effect_info);
		}

		return true;
	}
}

public sealed class VehicleCollectionParts
{
	public uint GUID;
	public string Name;
	public List<VehiclePart> Parts = new();
	public List<uint> Excludes = new();
	public VehicleSettings VehicleSettings;
}

public sealed class VehicleCollection
{
	public uint GUID;
	public string Name;
	public List<VehicleCollectionParts> Parts = new();

	public bool GetRandomEntry(out VehicleCollectionParts entry)
	{
		int part_count = Parts.Count;

		if (part_count == 0)
		{
			entry = default;
			return false;
		}

		entry = Parts[Random.Shared.Next(part_count)];
		return true;
	}
}

public sealed class VehicleFamily
{
	public string VehicleID;
	public List<VehiclePart> GlobalParts = new();
	public List<VehicleCollection> Collections = new();

	public void GetParts(out List<VehiclePart> parts, out List<uint> excludes, out VehicleSettings settings)
	{
		parts = new(GlobalParts);
		excludes = new();
		settings = default;

		foreach (VehicleCollection collection in Collections)
		{
			if (!collection.GetRandomEntry(out VehicleCollectionParts collection_parts))
			{
				continue;
			}

			parts.AddRange(collection_parts.Parts);
			excludes.AddRange(collection_parts.Excludes);

			if (collection_parts.VehicleSettings.IsUsed)
			{
				settings = collection_parts.VehicleSettings;
			}
		}
	}

	public void GetParts(PropertyContainer spawnsets, VehicleRuleset ruleset, out List<VehiclePart> parts, out List<uint> excludes, out VehicleSettings settings)
	{
		parts = new(GlobalParts);
		excludes = new();
		settings = default;

		List<uint> used_collections = new();

		foreach (string spawnset_name in ruleset.GetRules())
		{
			if (!spawnsets.GetContainer(spawnset_name, out PropertyContainer spawnset))
			{
				continue;
			}

			GetParts(spawnset, used_collections, parts, excludes, ref settings);
		}

		foreach (VehicleCollection collection in Collections)
		{
			if (used_collections.Contains(collection.GUID))
			{
				continue;
			}

			if (collection.GetRandomEntry(out VehicleCollectionParts collection_parts))
			{
				parts.AddRange(collection_parts.Parts);
				excludes.AddRange(collection_parts.Excludes);

				if (collection_parts.VehicleSettings.IsUsed)
				{
					settings = collection_parts.VehicleSettings;
				}
			}
		}
	}

	private void GetParts(PropertyContainer spawnset, List<uint> used_collections, List<VehiclePart> parts, List<uint> excludes, ref VehicleSettings settings)
	{
		foreach (PropertyContainer spawnset_entry in spawnset.GetContainers())
		{
			spawnset_entry.GetValue("_collectionGUID", out uint collection_guid);
			spawnset_entry.GetValue("_partGUID", out uint part_entry_guid);

			VehicleCollectionParts collection_parts = FindCollectionParts(collection_guid, part_entry_guid);

			if (collection_parts != null)
			{
				parts.AddRange(collection_parts.Parts);
				excludes.AddRange(collection_parts.Excludes);

				if (collection_parts.VehicleSettings.IsUsed)
				{
					settings = collection_parts.VehicleSettings;
				}

				used_collections.Add(collection_guid);
			}
		}
	}

	private VehicleCollectionParts FindCollectionParts(uint collection_guid, uint part_entry_guid)
	{
		foreach (VehicleCollection collection in Collections)
		{
			if (collection.GUID != collection_guid)
			{
				continue;
			}

			foreach (VehicleCollectionParts collection_parts in collection.Parts)
			{
				if (collection_parts.GUID == part_entry_guid)
				{
					return collection_parts;
				}
			}
		}

		return null;
	}
}

public sealed class VehiclePaperdoll
{
	public VehicleType GetVehicleType() => VehicleType;
	public int GetNumberOfWheels() => NumberOfWheels;
	public int GetNumberOfRotors() => NumberOfRotors;

	private string[] Excludes = new string[] {
		"exclude_0",
		"exclude_1",
		"exclude_2",
		"exclude_3",
		"exclude_4",
		"exclude_5",
		"exclude_6",
		"exclude_7",
		"exclude_8",
		"exclude_9",
	};

	private VehicleType VehicleType;
	private int NumberOfWheels;
	private int NumberOfRotors;
	private PropertyContainer Spawnsets;
	private PropertyContainer DefaultModules;
	private List<VehicleFamily> Families = new();
	private List<VehiclePart> Parts = new();
	private Dictionary<uint, VehiclePart> PartsByGUID = new();

	public static bool Create(PropertyContainer properties, PropertyContainer spawnsets, out VehiclePaperdoll vehicle_paperdoll)
	{
		vehicle_paperdoll = new();

		if (!properties.GetContainer("_vdoll", out PropertyContainer vdoll))
		{
			return false;
		}

		spawnsets.GetContainer("_spawnsets", out vehicle_paperdoll.Spawnsets);
		vdoll.GetValue("_vehicletype", out vehicle_paperdoll.VehicleType);
		vdoll.GetValue("nof_wheels", out vehicle_paperdoll.NumberOfWheels);
		vdoll.GetValue("nof_rotors", out vehicle_paperdoll.NumberOfRotors);
		vdoll.GetContainer("_default_modules", out vehicle_paperdoll.DefaultModules);

		if (!vdoll.GetContainer("_parts", out PropertyContainer parts) || !vehicle_paperdoll.ReadParts(parts))
		{
			vehicle_paperdoll = null;
			return false;
		}

		if (vdoll.GetContainer("_families", out PropertyContainer families))
		{
			vehicle_paperdoll.ReadFamilies(families);
		}

		return true;
	}

	private VehiclePaperdoll() { }

	private bool ReadParts(PropertyContainer properties)
	{
		foreach (PropertyContainer part in properties.GetContainers())
		{
			if (!ReadPart(part, null))
			{
				return false;
			}
		}

		return true;
	}

	private bool ReadPart(PropertyContainer properties, VehiclePart parent)
	{
		if (!properties.GetValue("_class", out string class_name) || class_name != "CPartProp")
		{
			return false;
		}

		VehiclePart part = new();


		if (!properties.GetValue("model_shrt", out part.ResourceDescription.ModelPath) || part.ResourceDescription.ModelPath == "")
		{
			properties.GetValue("model", out part.ResourceDescription.ModelPath, "");
		}

		if (!properties.GetValue("pfx_shrt", out part.ResourceDescription.PfxPath) || part.ResourceDescription.PfxPath == "")
		{
			properties.GetValue("pfx", out part.ResourceDescription.PfxPath, "");
		}

		if (properties.GetContainer("_modules", out part.Modules))
		{
			part.ReadModules();
		}

		properties.GetValue("world", out part.ResourceDescription.World);
		properties.GetContainer("_smartobjects", out part.SmartObjects);
		properties.GetValue("is_wheel", out part.WheelInfo.IsWheel);
		properties.GetValue("is_suspension", out part.WheelInfo.IsSuspension);
		properties.GetValue("is_staticsuspension", out part.WheelInfo.IsStaticSuspension);
		properties.GetValue("tyre_scale", out part.WheelInfo.Scale, new Vec3(new float[] { 1, 1, 1 }));
		properties.GetValue("wheel_slot", out part.WheelInfo.Slot);
		properties.GetValue("is_steering", out part.SteeringInfo.IsSteering);
		properties.GetValue("is_upper_steering", out part.SteeringInfo.IsUpperSteering);
		properties.GetValue("is_rotor", out part.RotorInfo.IsRotor);
		properties.GetValue("rotor_slot", out part.RotorInfo.Slot);
		properties.GetValue("rotor_part_type", out part.RotorInfo.RotorPart);

		// TODO: This is SUPPOSED to check if the part is valid or the pfx was a valid rigidbody + entered into cache
		if (part.IsValid() || part.ResourceDescription.PfxPath != "")
		{
			properties.GetValue("_GUID", out part.GUID);
			properties.GetValue("_name", out part.Name);
			properties.GetValue("part_type", out part.Type, "");
			properties.GetValue("skin_to_parent", out part.SkinToParent);

			if (properties.GetContainer("_effects", out PropertyContainer effects))
			{
				part.ReadEffects(effects);
			}

			Parts.Add(part);
			PartsByGUID.Add(part.GUID, part);
		}

		if (parent != null)
		{
			part.Parent = parent;
			parent.Children.Add(part);
		}

		foreach (PropertyContainer child_part in properties.GetContainers())
		{
			ReadPart(child_part, part);
		}

		return true;
	}

	private void ReadFamilies(PropertyContainer properties)
	{
		foreach (var family_container in properties.GetContainerPairs())
		{
			if (!family_container.Value.GetValue("_class", out string class_name) || class_name != "CFamilyProp")
			{
				continue;
			}

			VehicleFamily family = new();
			family_container.Value.GetValue("VehicleID", out family.VehicleID);
			ReadFamily(family_container.Value, family_container.Key, family, null, false);
			Families.Add(family);
		}
	}

	private void ReadFamily(PropertyContainer properties, uint key, VehicleFamily family, VehicleCollection collection, bool collection_head)
	{
		bool new_collection = false;

		if (properties.GetValue("_class", out string class_name))
		{
			switch (class_name)
			{
				case "CNullProp":
					{
						if (collection_head)
						{
							VehicleCollectionParts parts = new();
							properties.GetValue("_GUID", out parts.GUID);
							properties.GetValue("_name", out parts.Name);
							collection.Parts.Add(parts);
						}

						VehiclePart referenced_part = new();
						referenced_part.IsReference = true;
						properties.GetValue("_refGUID", out referenced_part.GUID);
						properties.GetContainer("_modules", out referenced_part.Modules);
						properties.GetContainer("_smartobjects", out referenced_part.SmartObjects);

						HandleReference(referenced_part, family, collection);
					}
					break;
				case "CCollectionProp":
					collection = new VehicleCollection();
					properties.GetValue("_GUID", out collection.GUID);
					properties.GetValue("_name", out collection.Name);
					family.Collections.Add(collection);
					new_collection = true;
					break;
				case "CReferenceProp":
					{
						properties.GetValue("_refGUID", out uint reference_guid);

						if (PartsByGUID.TryGetValue(reference_guid, out VehiclePart referenced_part))
						{
							if (collection_head)
							{
								VehicleCollectionParts parts = new VehicleCollectionParts();
								properties.GetValue("_GUID", out parts.GUID);
								properties.GetValue("_name", out parts.Name);
								collection.Parts.Add(parts);
							}

							HandleReference(referenced_part, family, collection);
						}
					}
					break;
				case "CExcludeProp":
				case "CPaperdollSettingsProp":
					{
						int entry_count = collection?.Parts.Count ?? 0;

						if (!collection_head && entry_count > 0)
						{
							VehicleCollectionParts parts = collection.Parts[entry_count - 1];

							if (class_name == "CExcludeProp")
							{
								foreach (string exclude in Excludes)
								{
									if (properties.GetValue(exclude, out uint exclude_id) && exclude_id > 0)
									{
										parts.Excludes.Add(exclude_id);
									}
								}
							}
							else
							{
								parts.VehicleSettings.IsUsed = true;
								properties.GetValue("is_cabbed", out parts.VehicleSettings.IsCabbed);
								properties.GetValue("DisplayName", out parts.VehicleSettings.DisplayName);
							}
						}
					}
					break;
			}
		}

		foreach (var family_properties in properties.GetContainerPairs())
		{
			ReadFamily(family_properties.Value, family_properties.Key, family, collection, new_collection);
		}
	}

	private void HandleReference(VehiclePart part, VehicleFamily family, VehicleCollection collection)
	{
		if (collection != null)
		{
			int entry_count = collection.Parts.Count;

			if (entry_count > 0)
			{
				collection.Parts[entry_count - 1].Parts.Add(part);
			}
		}
		else
		{
			family.GlobalParts.Add(part);
		}
	}

	public void SpawnInstance(string archive_path, Vector3 position, VehicleRuleset ruleset = null)
	{
		if (Families.Count > 0)
		{
			List<VehiclePart> parts = new();
			List<uint> excludes = new();
			VehicleSettings settings = default;

			if (Spawnsets != null && ruleset != null)
			{
				Families[0].GetParts(Spawnsets, ruleset, out parts, out excludes, out settings);
			}
			else
			{
				Families[0].GetParts(out parts, out excludes, out settings);
			}

			foreach (VehiclePart part in parts)
			{
				if (part.ResourceDescription.ModelPath != null && part.ResourceDescription.ModelPath != "")
				{
					RenderBlockModel entity = new(archive_path, part.ResourceDescription.ModelPath, part.ResourceDescription.PfxPath);

					if (part.WheelInfo.IsWheel || part.WheelInfo.IsSuspension || part.WheelInfo.IsStaticSuspension)
					{
						entity.Scale = part.WheelInfo.Scale.x;
					}

					float APEX_TO_SOURCE = 52.49344f;

					entity.Position = position + new Vector3(
						-part.ResourceDescription.World.data[3 * 3 + 2] * APEX_TO_SOURCE,
						-part.ResourceDescription.World.data[3 * 3 + 0] * APEX_TO_SOURCE,
						part.ResourceDescription.World.data[3 * 3 + 1] * APEX_TO_SOURCE
					);

					//entity.Rotation = Rotation;
				}
				else
				{
					Log.Info(part?.Name ?? "");
				}
			}
		}
	}
}

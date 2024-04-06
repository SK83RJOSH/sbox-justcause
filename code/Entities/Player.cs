namespace JustCause.Entities;

using JustCause.FileFormats.SmallArchive;
using JustCause.Resources;
using JustCause.Resources.PaperDoll;
using Sandbox;
using System.Collections.Generic;
using System.IO;
using PropertyContainer = FileFormats.PropertyContainer.PropertyContainer<uint>;

partial class Player : Sandbox.Player
{
	public readonly Clothing.Container Clothing = new();

	private DamageInfo lastDamage;
	private ResourceLoader ResourceLoader = new();

	public override void Respawn()
	{
		SetModel("models/citizen/citizen.vmdl");

		// Use WalkController for movement (you can make your own PlayerController for 100% control)
		Controller = new WalkController();

		// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
		Animator = new StandardPlayerAnimator();

		// Use ThirdPersonCamera (you can make your own Camera for 100% control)
		Camera = new ThirdPersonCamera();

		// Load player clothing
		Clothing.LoadFromClient(Client);
		Clothing.DressEntity(this);

		// Default stuff
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		base.Respawn();
	}

	public override void TakeDamage(DamageInfo info)
	{
		if (GetHitboxGroup(info.HitboxIndex) == 1)
		{
			info.Damage *= 10.0f;
		}

		lastDamage = info;

		TookDamage(lastDamage.Flags, lastDamage.Position, lastDamage.Force);

		base.TakeDamage(info);
	}

	[ClientRpc]
	public void TookDamage(DamageFlags damageFlags, Vector3 forcePos, Vector3 force)
	{
	}

	public override void Simulate(Client cl)
	{
		base.Simulate(cl);

		if (IsServer && Input.Pressed(InputButton.Use))
		{
			List<string> files = new();
			string archive_path = "";
			string vdoll_path = "";

			/* Silverbolt bby */
			//archive_path = "assets\\arve.v067_speedplane.ee";
			//vdoll_path = "v067_speedplane.vdoll";

			/* Havoc */
			//archive_path = "assets\\arve.v060_attackheli.ee";
			//vdoll_path = "v060_attackheli.mvdoll";

			/* Tuk-Tuk */
			//archive_path = "assets\\lave.v005_tuktuk_civ.ee";
			//vdoll_path = "v005_tuktuk_civ.mvdoll";

			/* APC */
			//archive_path = "assets\\lave.v016_military_apc.ee";
			//vdoll_path = "v016_military_apc.mvdoll";

			/* Truck Buss OwO */
			//archive_path = "assets\\lave.v004_truck_buss.ee";
			//vdoll_path = "v004_truck_buss.mvdoll";

			/* Bus */
			//archive_path = "assets\\lave.v046_buscoach.ee";
			//vdoll_path = "v046_busscoach.mvdoll";

			/* Monster */
			//archive_path = "assets\\lave.v050_large_super_truck.ee";
			//vdoll_path = "v050_large_super_truck.mvdoll";

			/* Limo */
			//archive_path = "assets\\lave.v109_limo.ee";
			//vdoll_path = "v109_limo.mvdoll";

			/* balloon */
			//archive_path = "assets\\ballonfighter.ee";
			//files = new() { "gb400_lod1-a", "gb400_lod1-b", "gb400_lod1-c", "gb400_lod1-d" };

			/* concrete barrier */
			//archive_path = "assets\\dropoffpoint.car.ee";
			//files = new() { "go180_lod1-b" };

			/* JUMBO jet */
			//archive_path = "assets\\arve.v114_jumbojet.ee";
			//vdoll_path = "v114_jumbojet.mvdoll";

			/* dish */
			//archive_path = "assets\\f3m02.radarstation.nlz";
			//files = new() { "go098_lod1-a", "go098_lod1-b" };

			/* djonk */
			//archive_path = "assets\\seve.v088_djonk.ee";
			//vdoll_path = "v088_djonk.vdoll";

			/* corvette */
			//archive_path = "assets\\lave.v025_sportcar.ee";
			//vdoll_path = "v025_sportcar.mvdoll";

			/* im a cowboy baby */
			archive_path = "assets\\lave.v041_tractor.ee";
			vdoll_path = "v041_tractor.mvdoll";

			if (ResourceLoader.LoadArchive(archive_path, out SmallArchive archive) && archive.TryGetFile(vdoll_path, out MemoryStream prop_stream))
			{
				if (!PropertyContainer.Read(new BinaryReader(prop_stream), out PropertyContainer vehicle_properties))
				{
					return;
				}

				PropertyContainer vehicle_spawnsets = new();

				// TODO: This needs to be read from the main vehicle file; but this will do for now
				if (archive.TryGetFile(vdoll_path.Replace("vdoll", "sst").Replace(".m", "."), out MemoryStream sst_stream))
				{
					PropertyContainer.Read(new BinaryReader(sst_stream), out vehicle_spawnsets);
				}

				if (!VehiclePaperdoll.Create(vehicle_properties, vehicle_spawnsets, out VehiclePaperdoll paperdoll))
				{
					return;
				}

				VehicleRuleset ruleset = new();
				ruleset.AddRule("Default");
				paperdoll.SpawnInstance(archive_path, Position, ruleset);

				Log.Info("Success!");
			}

			foreach (string part in files)
			{
				RenderBlockModel entity = new(archive_path, part + ".lod", null);
				entity.Position = Position;
				entity.Rotation = Rotation;
			}
		}
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomeRagdollOnClient(Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone(lastDamage.HitboxIndex));
		EnableAllCollisions = false;
		EnableDrawing = false;
	}
}

namespace JustCause.Entities;

using JustCause.FileFormats.PropertyContainer;
using JustCause.FileFormats.SmallArchive;
using JustCause.Resources;
using Sandbox;
using System.Collections.Generic;
using System.IO;
using PropertyContainer = FileFormats.PropertyContainer.PropertyContainer<uint>;
using PropertyFile = FileFormats.PropertyContainer.PropertyFile<uint>;

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

			/* Havoc */
			archive_path = "assets\\arve.v060_attackheli.ee";
			vdoll_path = "v060_attackheli.mvdoll";

			/* Tuk-Tuk */
			//archive_path = "assets\\lave.v005_tuktuk_civ.ee";
			//vdoll_path = "v005_tuktuk_civ.mvdoll";

			/* APC */
			//archive_path = "assets\\lave.v016_military_apc.ee";
			//vdoll_path = "v016_military_apc.mvdoll";

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

			if (ResourceLoader.LoadArchive(archive_path, out SmallArchive archive) && archive.TryGetFile(vdoll_path, out MemoryStream prop_stream))
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
							RenderBlockModel entity = new(archive_path, model_name);
							entity.Position = Position;
							entity.Rotation = Rotation;
						}

						foreach (PropertyContainer child_part in part.GetContainers())
						{
							if (child_part.GetValue("model_shrt", out string child_model_name))
							{
								RenderBlockModel entity = new(archive_path, child_model_name);
								entity.Position = Position;
								entity.Rotation = Rotation;
							}
						}
					}

					Log.Info("Success!");
				}
			}

			foreach (string part in files)
			{
				RenderBlockModel entity = new(archive_path, part + ".rbm");
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

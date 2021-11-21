namespace JustCause.Entities;

using Sandbox;
using System.Collections.Generic;

partial class Player : Sandbox.Player
{
	public readonly Clothing.Container Clothing = new();

	private DamageInfo lastDamage;

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
			string archive = "";
			List<string> files = new();

			/* Havoc */
			//archive = "arve.v060_attackheli.ee";
			//files = new() { "v060-body_m_lod1", "v060_lod1-tail", "v060_lod1-door_fu1" };

			/* Tuk-Tuk */
			archive = "lave.v005_tuktuk_civ.ee";
			files = new() { "v005civ-body_m_lod1", "v005civ_lod1-uppersteering" };

			/* APC */
			//archive = "lave.v016_military_apc.ee";
			//files = new() { "v016_lod1-base", "v016_lod1-lavett", "v016_lod1-mount", "v016_lod1-tanksuspension", "v016_lod1-weapon", "v016-body_m_lod1" };

			/* balloon */
			//archive = "ballonfighter.ee";
			//files = new() { "gb400_lod1-a", "gb400_lod1-b", "gb400_lod1-c", "gb400_lod1-d" };

			/* concrete barrier */
			//archive = "dropoffpoint.car.ee";
			//files = new() { "go180_lod1-b" };

			/* JUMBO jet */
			//archive = "arve.v114_jumbojet.ee";
			//files = new() { "v114-wing_cr1_m_lod1", "v114-wing_cl1_m_lod1", "v114-body_m_lod1", "v114_lod1-suspension_cr1", "v114_lod1-suspension_fd", "v114_lod1-stabilizer_bc1", "v114_lod1-suspension_cl1" };

			/* dish */
			//archive = "f3m02.radarstation.nlz";
			//files = new() { "go098_lod1-a", "go098_lod1-b" };

			foreach (string part in files)
			{
				RenderBlockModel entity = new("assets\\" + archive, part + ".rbm");
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

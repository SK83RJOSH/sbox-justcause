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
			List<string> parts = new();

			/* Havoc */
			//parts = new() { "v060-body_m_lod1", "v060_lod1-tail", "v060_lod1-door_fu1" };

			/* Tuk-Tuk */
			//parts = new() { "v005civ-body_m_lod1", "v005civ_lod1-uppersteering" };

			/* APC */
			//parts = new() { "v016_lod1-base", "v016_lod1-lavett", "v016_lod1-mount", "v016_lod1-tanksuspension", "v016_lod1-weapon", "v016-body_m_lod1" };

			foreach (string part in parts)
			{
				RenderBlockEntity entity = new("assets\\" + part + ".rbm");
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

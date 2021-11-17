namespace JustCause;

using Sandbox;

public partial class Game : Sandbox.Game
{
	public UI.HUD HUD { get; private set; }
		
	public override void ClientSpawn()
	{
		base.ClientSpawn();
		HUD = new UI.HUD();
	}

	public override void ClientJoined(Client client)
	{
		base.ClientJoined(client);

		// Create a pawn and assign it to the client.
		var player = new Entities.Player();
		client.Pawn = player;
		player.Respawn();
	}
}

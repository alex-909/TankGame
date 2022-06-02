using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DapperDino.Mirror.Tutorials.Lobby;
using DapperDino.Tutorials.Lobby;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class DeathManager : NetworkBehaviour
{
	#region System References

	private PlayerSpawnSystem spawnSystem;
	public PlayerSpawnSystem SpawnSystem
	{
		get
		{
			if (spawnSystem != null) { return spawnSystem; }
			return spawnSystem = (PlayerSpawnSystem)FindObjectOfType(typeof(PlayerSpawnSystem));
		}
	}

	private CreateMap mapCreator;
	private CreateMap MapCreator
	{
		get
		{
			if (mapCreator != null) { return mapCreator; }
			return mapCreator = FindObjectOfType<CreateMap>();
		}
	}

	private NetworkManagerTG room;
	private NetworkManagerTG Room
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as NetworkManagerTG;
		}
	}

	private PlayerScoreManager playerScoreManager;
	private PlayerScoreManager PlayerScoreManager
	{
		get 
		{
			if (playerScoreManager != null) { return playerScoreManager; }
			return playerScoreManager = FindObjectOfType<PlayerScoreManager>();
		}
	}
	#endregion

	public List<PlayerScript> AllPlayers { get; } = new List<PlayerScript>();
	public List<PlayerScript> AlivePlayers { get; } = new List<PlayerScript>();
	public List<PlayerScript> DeadPlayers { get; } = new List<PlayerScript>();

	[SerializeField] private Animator animator = null;
	[SerializeField] private GameObject WinnerUI;
	private string triggerName = "Active";

	[Server]
	public void Register(PlayerScript player)
	{
		//Debug.Log("added player to list");
		AlivePlayers.Add(player);
		AllPlayers.Add(player);

		if (AllPlayers.Count == Room.GamePlayers.Count) 
		{
			PlayerScoreManager.SendPlayerList(AllPlayers.ToArray());
		}
		//SendScoreUpdate();
	}

	

	#region PlayerDeath
	public void Died(PlayerScript player)
	{
		//SendScoreUpdate();
		//Debug.Log("Player died!");

		for (int i = 0; i < AlivePlayers.Count; i++)
		{
			if (AlivePlayers[i].Equals(player))
			{
				//Debug.Log("found the player");
				PlayerScoreManager.IncreaseStatInPlayer(PlayerStat.Deaths, player.networkGamePlayerIdentity);

				DeadPlayers.Add(player);
				AlivePlayers.Remove(player);

				RpcDisablePlayer(player);
				CheckForRestart();
			}
		}
	}
	public void PlayerGotKill(NetworkIdentity player) 
	{
		PlayerScoreManager.IncreaseStatInPlayer(PlayerStat.Kills, player);
	}
	[ClientRpc]
	private void RpcDisablePlayer(PlayerScript player)
	{
		player.gameObject.SetActive(false);
	}

	[Server]
	private void CheckForRestart()
	{
		if (AlivePlayers.Count > 1) { return; }

		PlayerScoreManager.IncreaseStatInPlayer(PlayerStat.RoundsWon, AlivePlayers[0].networkGamePlayerIdentity);
		//SendScoreUpdate();
		RpcShowWinner(AlivePlayers[0]);
	}
	#endregion

	#region Winner + Animation

	[ClientRpc]
	private void RpcShowWinner(PlayerScript player)
	{
		CmdDestroyAllBullets();

		var textColor = WinnerUI.GetComponent<TextMeshProUGUI>();
		textColor.color = GameAssets.i.GetColor(player.GetComponent<ApplyMaterials>().tankColor);
		textColor.text = $"{player.playerName} has won the Round!";

		animator.enabled = true;
		animator.SetTrigger(triggerName);
	}
	public void AnimationEnded()
	{
		Debug.Log("animation end call");
		animator.enabled = false;
		WinnerUI.SetActive(false);

		CmdReloadScene();
	}

	[Command(requiresAuthority = false)]
	private void CmdReloadScene() 
	{
		if (SceneManager.GetActiveScene().name.Equals(SceneNames.Scene_Map_02_Playground))
		{
			Room.ServerChangeScene(SceneNames.Scene_Map_02_Playground);
		}
		else 
		{
			Room.ServerChangeScene(SceneNames.Scene_Map_01);
		}
	}

	#endregion

	#region DestroyBullets
	[Command]
	private void CmdDestroyAllBullets() 
	{
		RpcDestroyAllBullets();
	}
	[ClientRpc]
	private void RpcDestroyAllBullets()
	{
		//RpcDestroyAllBullets();
		Debug.Log("destroying all the bullets");
		BulletMove[] bullets = FindObjectsOfType<BulletMove>();
		foreach (BulletMove bullet in bullets)
		{
			if (bullet) { NetworkServer.Destroy(bullet.gameObject); }
		}
	}
	#endregion

	#region Unused right now:

	private static System.Random rng = new System.Random();
	private Transform[] ShuffleSpawnPoints(List<Transform> points)
	{
		var shuffledPoints = points.OrderBy(a => rng.Next());
		return shuffledPoints.ToArray<Transform>();
	}

	[ClientRpc]
	private void RpcActivatePlayer(PlayerScript player)
	{
		player.gameObject.SetActive(true);
	}

	[Command(requiresAuthority = false)]
	private void CmdRespawn()
	{
		//RespawnAllPlayers();
	}


	[Server]
	private void ResetMap()
	{
		MapCreator.ResetMap();
	}

	[Server]
	private void RespawnAllPlayers()
	{
		AlivePlayers.Clear();
		AlivePlayers.AddRange(AllPlayers);

		List<Transform> spawnPointsList = PlayerSpawnSystem.spawnPoints;

		//Transform[] spawnPoints = ShuffleSpawnPoints(spawnPointsList);

		//CmdDestroyAllBullets();

		//ResetMap();

		Debug.Log("respawning all players");

		for (int i = 0; i < AllPlayers.Count; i++)
		{
			//Debug.Log("in loop: i = " + i);
			var player = AllPlayers[i];

			//player.BlockInput();
			//player.SetPosition(spawnPoints[i].position, spawnPoints[i].rotation);
			//RpcActivatePlayer(player);
			//player.Respawn();
		}

		AlivePlayers.Clear();
		AlivePlayers.AddRange(AllPlayers);
		DeadPlayers.Clear();

		Debug.Log("enabling round animator");
		//RoundSystem.EnableAnimator();
	}
	#endregion

}

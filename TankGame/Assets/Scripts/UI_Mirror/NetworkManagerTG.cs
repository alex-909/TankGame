using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DapperDino.Mirror.Tutorials.Lobby
{
	public class NetworkManagerTG : NetworkManager
    {
        [SerializeField] private int minPlayers = 2;
        [SerializeField] private string menuScene;

        [Header("Maps")]
        //[SerializeField] private int numberOfRounds = 1;
        //[SerializeField] private MapSet mapSet = null;

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerTG roomPlayerPrefab = null;

        [Header("Game")]
        [SerializeField] private NetworkGamePlayerTG gamePlayerPrefab = null;
        [SerializeField] private GameObject playerSpawnSystem = null;
        [SerializeField] private GameObject playerScoreSystem = null;
        [SerializeField] private GameObject mapCreatorSystem = null;
        [SerializeField] private GameObject roundSystem = null;
        [SerializeField] private GameObject deathManager = null;
        [SerializeField] private GameObject scoreManager = null;

        //private MapHandler mapHandler;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied;
        public static event Action OnServerStopped;

        public List<NetworkRoomPlayerTG> RoomPlayers { get; } = new List<NetworkRoomPlayerTG>();
        public List<NetworkGamePlayerTG> GamePlayers { get; } = new List<NetworkGamePlayerTG>();

        public override void OnStartServer() 
        {
            spawnPrefabs.Clear();
            spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        } 

        public override void OnStartClient()
        {
            spawnPrefabs.Clear();
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

		[Obsolete]
		public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            OnClientConnected?.Invoke();
        }

		[Obsolete]
		public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().name != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;

                NetworkRoomPlayerTG roomPlayerInstance = Instantiate(roomPlayerPrefab);

                roomPlayerInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerTG>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            OnServerStopped?.Invoke();

            RoomPlayers.Clear();
            GamePlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < minPlayers) { return false; }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady) { return false; }
            }

            return true;
        }

        public void StartGame(String sceneName)
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                if (!IsReadyToStart()) { return; }

                //mapHandler = new MapHandler(mapSet, numberOfRounds);

                //ServerChangeScene(mapHandler.NextMap);
                //Debug.Log("in start game method");
                ServerChangeScene(sceneName);
            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            // From menu to game
            if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
            {
                //take all the roomplayers
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);

                    gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName); //sync some variables

                    //switch roomplayer to gameplayer with same connection
                    NetworkServer.Destroy(conn.identity.gameObject);
                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                }

                GameObject playerScoreManager = Instantiate(playerScoreSystem);
                NetworkServer.Spawn(playerScoreManager);
                DontDestroyOnLoad(playerScoreManager);
            }
            else if (newSceneName.StartsWith("Scene_Map"))
            {
                //idk
            }

            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName.Equals(SceneNames.Scene_Map_02_Playground)) 
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);

                GameObject roundSystemInstance = Instantiate(roundSystem);
                NetworkServer.Spawn(roundSystemInstance);

                GameObject deathManagerInstance = Instantiate(deathManager);
                NetworkServer.Spawn(deathManagerInstance);

                GameObject scoreManagerInstance = Instantiate(scoreManager);
                NetworkServer.Spawn(scoreManagerInstance);
            }
            else if (sceneName.StartsWith("Scene_Map"))
            {
                //doesnt need to stay between scenes: (Spawnsystem | mapCreatorSystem | roundSystem)
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);

                GameObject mapCreatorSystemInstance = Instantiate(mapCreatorSystem);
                NetworkServer.Spawn(mapCreatorSystemInstance);

                GameObject roundSystemInstance = Instantiate(roundSystem);
                NetworkServer.Spawn(roundSystemInstance);

                GameObject deathManagerInstance = Instantiate(deathManager);
                NetworkServer.Spawn(deathManagerInstance);

                GameObject scoreManagerInstance = Instantiate(scoreManager);
                NetworkServer.Spawn(scoreManagerInstance);
            }
        }

		public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            OnServerReadied?.Invoke(conn);
        }
    }
}

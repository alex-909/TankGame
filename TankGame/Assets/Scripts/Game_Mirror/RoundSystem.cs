using Assets.Scripts.Game_Mirror;
using DapperDino.Mirror.Tutorials.Lobby;
using Mirror;
using System.Linq;
using UnityEngine;

namespace DapperDino.Tutorials.Lobby
{
    public class RoundSystem : NetworkBehaviour
    {
        private readonly string triggerName = "Active";

        [SerializeField] private Animator animator = null;

        private NetworkManagerTG room;
        private NetworkManagerTG Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerTG;
            }
        }

        public void CountdownEnded()
        {
            animator.enabled = false;
        }

        public void EnableAnimator()
        {
            RpcStartCountdown();
        }

        #region Server

        public override void OnStartServer()
        {
            NetworkManagerTG.OnServerStopped += CleanUpServer;
            NetworkManagerTG.OnServerReadied += CheckToStartRound;
        }

        [ServerCallback]
        private void OnDestroy() => CleanUpServer();

        [Server]
        private void CleanUpServer()
        {
            NetworkManagerTG.OnServerStopped -= CleanUpServer;
            NetworkManagerTG.OnServerReadied -= CheckToStartRound;
        }

        [ServerCallback]
        public void StartRound()
        {
            Debug.Log("The Round is now started!");
            RpcStartRound();
        }
        [Server]
        private void CheckToStartRound(NetworkConnection conn)
        {
            if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count) { return; }

            animator.enabled = true;
            animator.SetTrigger(triggerName);

            RpcStartCountdown();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcStartCountdown()
        {
            Debug.Log("counting down!");
            animator.enabled = true;
            animator.SetTrigger(triggerName);
        }

        [ClientRpc]
        private void RpcStartRound()
        {
            Debug.Log("player is free to move");
            InputManager.RemoveAbsolute(ActionMapNames.Player);
        }

        #endregion
    }
}
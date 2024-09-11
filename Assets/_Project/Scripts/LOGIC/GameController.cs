using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameController : SingletonNetwork<GameController>
{
    public static bool IsPaused { get; set; }

    [HideInInspector]
    public List<ulong> deadPlayerIds = new();

    public bool IsWin => deadPlayerIds.Count == ClientManager.Instance.ClientCount;

    public bool IsWinner(ulong clientId)
    {
        return deadPlayerIds.Count == ClientManager.Instance.ClientCount
               && clientId == deadPlayerIds.Last();
    }

    public int GetRankOfPlayerByClientId(ulong clientId)
    {
        // tìm thứ tự chết của player
        var deadIndex = deadPlayerIds.IndexOf(clientId);
        var rank = ClientManager.Instance.ClientCount - deadIndex;

        Debug.Log("Dead index: " + deadIndex + " Rank: " + rank);
        return rank;
    }

    [ServerRpc(RequireOwnership = false)]
    public void NewPlayerDeadServerRpc(ulong clientId)
    {
        DebugColor.Log("Server update player dead list", DebugColor.Cyan);
        deadPlayerIds.Add(clientId);
        NewPlayerDeadClientRpc(deadPlayerIds.ToArray());

        // * Check xem có còn 1 người còn lại cuối cùng không
        if (deadPlayerIds.Count == ClientManager.Instance.ClientCount - 1)
        {
            // tìm client Id không nằm trong list dead 
            var winnerClientId = ClientManager.Instance.ClientConnectedIds.Except(deadPlayerIds).First();
            WaitEndGameClientRpc(winnerClientId);
        }
    }

    [ClientRpc]
    private void NewPlayerDeadClientRpc(ulong[] newPlayerDeadIds)
    {
        DebugColor.Log("Client update player dead list", DebugColor.Cyan);
        deadPlayerIds = newPlayerDeadIds.ToList();

        // Kiểm tra show UI Endgame trên client
        if (deadPlayerIds.Contains(NetworkUtils.GetLocalClientId()))
            GamePlayScreen.Instance.GameOver();
    }

    [ClientRpc]
    private void WaitEndGameClientRpc(ulong winnerClientId)
    {
        DebugColor.Log("Client end game", DebugColor.Cyan);

        // Chờ 2s để cho client cuối chết
        if (NetworkUtils.GetLocalClientId() == winnerClientId)
            WaitToEndGame();
    }

    private async void WaitToEndGame()
    {
        await UniTask.Delay(2000);
        NewPlayerDeadServerRpc(NetworkUtils.GetLocalClientId());
    }
}
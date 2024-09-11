using System;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[Serializable]
public class PlayerInfo
{
    public int idInfo;
    public Sprite tankPreview;
    public GameObject tankPrefab;
}

public class ClientManager : SingletonNetworkPersistent<ClientManager>
{
    public const int MaxPlayers = 4;
    public int MapSelected;

    public List<PlayerInfo> playerInfosList = new(MaxPlayers);

    public List<GameObject> mapPrefabs = new();

    /// <summary>
    /// Quản lý client bằng cách: IdClient - IdPlayerInfo
    /// IdClient có thể có nhiều nhưng sẽ chỉ link đến 1 trong 4 IdPlayerInfo
    /// </summary>
    private Dictionary<ulong, int> _clientPlayerInfos = new(MaxPlayers);
    public List<ulong> ClientConnectedIds => _clientPlayerInfos.Keys.ToList();
    public int ClientCount => _clientPlayerInfos.Count;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    // * ONLY SERVER CAN RECEIVE THIS CALLBACK
    private void OnClientConnected(ulong clientId)
    {
        DebugColor.Log($"Client {clientId} connected", DebugColor.Yellow);

        // tìm 1 PlayerInfo chưa dùng và set cho Client này
        var idPlayerInfoNotUse = FindIdPlayerInfoNotUse();
        _clientPlayerInfos.Add(clientId, idPlayerInfoNotUse);

        // gửi lại list client info cho tất cả client
        var clientsData = new ClientPlayerInfoData
        {
            keys = _clientPlayerInfos.Keys.ToArray(),
            values = _clientPlayerInfos.Values.ToArray()
        };

        NewPlayerJoinedClientRpc(clientsData);
    }

    private int FindIdPlayerInfoNotUse()
    {
        var idPlayerInfoNotUse = 0;

        foreach (var playerInfo in playerInfosList)
        {
            if (!_clientPlayerInfos.ContainsValue(playerInfo.idInfo))
            {
                idPlayerInfoNotUse = playerInfo.idInfo;
                break;
            }
        }

        return idPlayerInfoNotUse;
    }

    [ClientRpc]
    private void NewPlayerJoinedClientRpc(ClientPlayerInfoData clientPlayerInfoData)
    {
        var keys = clientPlayerInfoData.keys;
        var values = clientPlayerInfoData.values;

        // Update lại list client info
        _clientPlayerInfos = new Dictionary<ulong, int>();

        for (var i = 0; i < keys.Length; i++)
            _clientPlayerInfos[keys[i]] = values[i];

        UILobbyScreen.Instance.WaitOtherPlayers();
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        DebugColor.Log($"Client {clientId} disconnected", DebugColor.Red);
    }

    public int GetPlayerIndexByClientId(ulong clientId)
    {
        var index = _clientPlayerInfos.Keys.ToList().FindIndex(e => e == clientId);
        return index;
    }

    public string GetPlayerNameByClientId(ulong clientId)
    {
        var index = GetPlayerIndexByClientId(clientId);
        return "Player " + (index + 1);
    }

    public PlayerInfo GetPlayerInfoByClientId(ulong clientId)
    {
        var idPlayerInfo = _clientPlayerInfos[clientId];
        return playerInfosList[idPlayerInfo];
    }


    [ServerRpc(RequireOwnership = false)]
    public void SelectMapServerRpc(int mapId)
    {
        MapSelected = mapId;
        SelectMapClientRpc(MapSelected);
    }

    [ClientRpc]
    private void SelectMapClientRpc(int mapId)
    {
        MapSelected = mapId;
        UILobbyScreen.Instance.waitingScreen.RefreshSelectMap(MapSelected);
    }


    private struct ClientPlayerInfoData : INetworkSerializable
    {
        public ulong[] keys;
        public int[] values;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref keys);
            serializer.SerializeValue(ref values);
        }
    }
}
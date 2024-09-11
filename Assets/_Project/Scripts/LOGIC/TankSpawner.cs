using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cysharp.Threading.Tasks;

public class TankSpawner : NetworkBehaviour
{
    public List<Vector3> spawnPositions;

    public override async void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            // * WAIT ALL CLIENT CHANGE SCENE COMPLETE
            await UniTask.Delay(1000);
            PreparePlayersServerRpc();
        }
    }

    [ServerRpc]
    private void PreparePlayersServerRpc()
    {
        var clientIds = ClientManager.Instance.ClientConnectedIds;

        foreach (var clientId in clientIds)
        {
            var playerInfo = ClientManager.Instance.GetPlayerInfoByClientId(clientId);

            NetworkObjectSpawner.SpawnNewNetworkObjectChangeOwnershipToClient(
                playerInfo.tankPrefab,
                RandomPosInMap(),
                Quaternion.identity,
                clientId
            );
        }

        SpawnMapClientRpc();
    }

    [ClientRpc]
    private void SpawnMapClientRpc()
    {
        var mapId = ClientManager.Instance.MapSelected - 1;
        Debug.Log("Spawn map complete! " + mapId);
        var mapPrefab = ClientManager.Instance.mapPrefabs[mapId];
        Instantiate(mapPrefab);
    }

    private Vector3 RandomPosInMap()
    {
        var randomIndex = Random.Range(0, spawnPositions.Count);
        var pos = spawnPositions[randomIndex];
        spawnPositions.RemoveAt(randomIndex);
        return pos;

        var randomX = Random.Range(-5f, 5f);
        var randomY = Random.Range(-3f, 3f);
        return new Vector3(randomX, randomY, 0f);
    }
}
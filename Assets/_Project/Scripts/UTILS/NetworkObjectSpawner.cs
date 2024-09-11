using UnityEngine;
using Unity.Netcode;

public static class NetworkObjectSpawner
{
    public static NetworkObject SpawnNewNetworkObject(
        GameObject prefab,
        Vector3 position = default,
        Quaternion rotation = default,
        bool destroyWithScene = true)
    {
#if UNITY_EDITOR
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("ERROR: Spawning not happening in the server!");
        }
#endif

        // We're first instantiating the new instance in the host client
        var newGameObject = Object.Instantiate(prefab, position, rotation);

        // Replicating that same new instance to all connected clients
        var newGameObjectNetworkObject = newGameObject.GetComponent<NetworkObject>();
        newGameObjectNetworkObject.Spawn(destroyWithScene);

        return newGameObjectNetworkObject;
    }

    public static NetworkObject SpawnNewNetworkObjectAsPlayerObject(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        ulong newClientOwnerId,
        bool destroyWithScene = true)
    {
#if UNITY_EDITOR
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("ERROR: Spawning not happening in the server!");
        }
#endif
        // We're first instantiating the new instance in the host client
        var newGameObject = Object.Instantiate(prefab, position, rotation);

        // Replicating that same new instance to all connected clients
        var newGameObjectNetworkObject = newGameObject.GetComponent<NetworkObject>();
        newGameObjectNetworkObject.SpawnAsPlayerObject(newClientOwnerId, destroyWithScene);

        // ! SpawnAsPlayerObject() similar with
        // ! NetworkManager.Singleton.ConnectedClients[newClientOwnerId].PlayerObject = newGameObjectNetworkObject;

        return newGameObjectNetworkObject;
    }

    public static NetworkObject SpawnNewNetworkObjectChangeOwnershipToClient(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        ulong newClientOwnerId,
        bool destroyWithScene = true)
    {
#if UNITY_EDITOR
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("ERROR: Spawning not happening in the server!");
        }
#endif
        // We're first instantiating the new instance in the host client
        var newGameObject = Object.Instantiate(prefab, position, rotation);

        // Replicating that same new instance to all connected clients
        var newGameObjectNetworkObject = newGameObject.GetComponent<NetworkObject>();
        newGameObjectNetworkObject.SpawnWithOwnership(newClientOwnerId, destroyWithScene);

        // * Trao quyền sở hữu, điều khiển object này cho 1 client.
        // * Client có quyền thay đổi vị trí, ... của nhân vật, sau đó sẽ được copy tới các client khác.

        return newGameObjectNetworkObject;
    }
    
    /* ---------------------------------------------------------------------------------------------------- */

    public static void DespawnNetworkObject(NetworkObject networkObject)
    {
#if UNITY_EDITOR
        if (!NetworkManager.Singleton.IsServer)
        {
            // Debug.LogError("ERROR: De-spawning not happening in the server!");
            return;
        }
#endif
        // if I'm an active on the networking session, tell all clients to remove
        // the instance that owns this NetworkObject
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
    }
}
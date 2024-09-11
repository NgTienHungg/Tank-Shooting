using UnityEngine;
using Unity.Netcode;

public class TankController : NetworkBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject fireEffectPrefab;

    private int _damage;
    private float _shootForce;
    private float _shootRate;

    public void Init(TankConfig tankConfig)
    {
        _damage = tankConfig.Damage;
        _shootForce = tankConfig.ShootForce;
        _shootRate = tankConfig.ShootCooldown;
    }

    [ServerRpc]
    public void RequestFireServerRpc(ulong clientId)
    {
        DebugColor.Log("SERVER FIRE BULLET!!!", DebugColor.Orange);

        var bulletPosition = firePoint.position;
        var bulletRotation = firePoint.rotation;

        // Spawn bullet on server and change ownership to client
        var bulletNetworkObject = NetworkObjectSpawner.SpawnNewNetworkObjectChangeOwnershipToClient(
            bulletPrefab,
            bulletPosition,
            bulletRotation,
            clientId
        );

        // Fire bullet on clients
        var bulletData = new BulletData()
        {
            networkId = bulletNetworkObject.NetworkObjectId,
            direction = firePoint.up,
            damage = _damage,
            shootForce = _shootForce
        };

        FireBulletClientRpc(bulletData);
    }

    [ClientRpc]
    private void FireBulletClientRpc(BulletData bulletData)
    {
        // instantiate fire effect at fire point
        Instantiate(fireEffectPrefab, firePoint.position, firePoint.rotation);

        // find new bullet just spawned to Fire
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bulletData.networkId, out var bulletNetworkObject))
        {
            var bullet = bulletNetworkObject.GetComponent<Bullet>();
            bullet.Init(bulletData.damage);
            bullet.Fire(bulletData.direction, bulletData.shootForce);
        }
    }

    private struct BulletData : INetworkSerializable
    {
        public ulong networkId;
        public Vector2 direction;
        public int damage;
        public float shootForce;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref networkId);
            serializer.SerializeValue(ref direction);
            serializer.SerializeValue(ref damage);
            serializer.SerializeValue(ref shootForce);
        }
    }
}
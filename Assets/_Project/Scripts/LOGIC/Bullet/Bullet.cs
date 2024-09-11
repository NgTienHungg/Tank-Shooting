using UnityEngine;
using Unity.Netcode;
using Cysharp.Threading.Tasks;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private GameObject explosionPrefab;

    private int _damage;

    public void Init(int damage)
    {
        _damage = damage;
    }

    public void Fire(Vector2 direction, float shootForce)
    {
        rigidBody.velocity = direction * shootForce;
    }

    private async void Start()
    {
        // auto destroy game object after 5 seconds
        await UniTask.Delay(5000);

        if (NetworkObject != null)
        {
            // DebugColor.Log("Despawn bullet after 5s", DebugColor.Red);
            NetworkObjectSpawner.DespawnNetworkObject(NetworkObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (IsOwner)
        {
            Debug.Log("Day la owner, isServer = " + IsServer);
        }

        DebugColor.Log("Bullet OnCollisionEnter2D", DebugColor.Orange);

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
        }

        RequestExplosionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestExplosionServerRpc()
    {
        ShowExplosionClientRpc(transform.position, transform.rotation);
    }

    [ClientRpc]
    private void ShowExplosionClientRpc(Vector3 position, Quaternion rotation)
    {
        Instantiate(explosionPrefab, position, rotation);
        Destroy(gameObject);
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Tank")]
public class TankConfig : ScriptableObject
{
    // HEALTH
    [SerializeField] private int maxHp = 1000;

    // MOVEMENT
    [SerializeField] private float moveSpeed = 5.0f;

    // CONTROLLER
    [SerializeField] private int damage = 100;
    [SerializeField] private float shootForce = 10.0f;
    [SerializeField] private float shootCooldown = 0.5f;

    public int MaxHp => maxHp;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float ShootForce => shootForce;
    public float ShootCooldown => shootCooldown;
}
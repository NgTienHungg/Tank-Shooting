using UnityEngine;
using Unity.Netcode;

public class TankMovement : NetworkBehaviour
{
    //Move by this transform, but rotate by viewTransform
    [SerializeField] private Transform viewTransform;
    [SerializeField] private Transform barrelTransform;

    private const float Tolerance = 0.1f;

    private float _moveSpeed;
    private Camera _camera;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _camera = Camera.main;
    }

    public void Init(TankConfig tankConfig)
    {
        _moveSpeed = tankConfig.MoveSpeed;
    }

    public void MoveAndRotate(float horizontal, float vertical)
    {
        // movement by input WADS
        var movement = new Vector3(horizontal, vertical);
        transform.position += movement * (Time.deltaTime * _moveSpeed);

        if (Mathf.Abs(movement.x) < Tolerance) return;
        var targetDirection = movement;
        targetDirection.z = 0;
        viewTransform.up = targetDirection;
    }

    public void RotateBarrel(Vector3 mousePosition)
    {
        var mouseWorldPosition = _camera.ScreenToWorldPoint(mousePosition);
        var direction = mouseWorldPosition - transform.position;
        direction.z = 0;
        barrelTransform.up = direction;
    }
}
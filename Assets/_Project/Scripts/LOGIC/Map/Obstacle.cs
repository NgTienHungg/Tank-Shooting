using Unity.Netcode;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        DebugColor.Log(other.gameObject.name, DebugColor.Orange);
    }
}
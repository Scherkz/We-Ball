using UnityEngine;

public abstract class SpecialShot : MonoBehaviour
{
    public abstract void Init(PlayerController playerController, Player player, Rigidbody2D body);
}

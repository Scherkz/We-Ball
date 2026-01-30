using UnityEngine;

public class BoostPad : MonoBehaviour
{
    private float boostMultiplier = 1.75f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null)
            return;

        Vector2 velocity = rb.linearVelocity;
        velocity.x *= boostMultiplier;
        rb.linearVelocity = velocity;
    }
}
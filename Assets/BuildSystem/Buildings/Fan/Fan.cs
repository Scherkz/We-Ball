using UnityEngine;

public class Fan : Building
{
    public float force;

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(transform.up * force);
    }
}

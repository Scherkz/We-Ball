using UnityEngine;

public class SpringBlock : MonoBehaviour
{
    [Range(0f, 89f)] public float maxAngleFromUp = 20f;
    [Range(0f, 1.5f)] public float restitution = 1.0f;
    public float extraKick = 0.0f;
    public float minHitSpeed = 0.1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        var rb = collision.rigidbody;
        if (!rb) return;
        
        var normal = collision.GetContact(0).normal;

        var cosLimit = Mathf.Cos(maxAngleFromUp * Mathf.Deg2Rad);
        if (Vector2.Dot(normal, transform.up) < cosLimit) return;

        var v = rb.linearVelocity;
        var vInto = Vector2.Dot(v, normal);
        if (vInto >= -minHitSpeed) return;

        var reflected = v - (1f + restitution) * vInto * normal;
        reflected += (Vector2)transform.up * (extraKick * Mathf.Abs(vInto));

        rb.linearVelocity = reflected;
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BlueShellMissile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float steeringAccel = 30f; 
    [SerializeField] private float lifeTime = 8f;
    
    [SerializeField] private GameObject explosionVfxPrefab;
    [SerializeField] private float knockbackForce = 35f;
    
    [SerializeField] private float explodeDistance = 0.35f;

    private Rigidbody2D rb;
    private Transform target;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
    
    public void Launch(Transform targetTransform, float initialAngleDeg)
    {
        target = targetTransform;
        
        rb.rotation = initialAngleDeg;
        rb.angularVelocity = 0f;
        
        Vector2 forward = Quaternion.Euler(0, 0, rb.rotation) * Vector2.up;
        rb.linearVelocity = forward * speed;

        if (lifeTime > 0f)
        {
            Destroy(gameObject, lifeTime);
        }
    }
    
    private void FixedUpdate()
    {
        if (target == null || rb == null)
            return;

        Vector2 toTarget = (Vector2)target.position - rb.position;
        
        if (toTarget.sqrMagnitude <= explodeDistance * explodeDistance)
        {
            Explode();
            return;
        }

        var desiredVel = toTarget.normalized * speed;
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            desiredVel,
            steeringAccel * Time.fixedDeltaTime
        );
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        var hitPlayer = otherCollider.GetComponentInParent<Player>();
        if (hitPlayer != null) 
        { 
            Knockback(otherCollider); 
            Explode(); 
        }
    }
    
    private void Knockback(Collider2D other)
    {
        var playerRb = other.GetComponentInParent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            playerRb.AddForce(randomDir * knockbackForce, ForceMode2D.Impulse);
        }
    }
    
    private void Explode()
    {
        if (explosionVfxPrefab != null)
        {
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}

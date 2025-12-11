using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BlueShellMissile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float rotateSpeedDegPerSec = 360f;
    [SerializeField] private float lifeTime = 8f;
    
    [SerializeField] private GameObject explosionVfxPrefab;
    [SerializeField] private float knockbackForce = 15f;

    private Rigidbody2D rb;
    private Transform target;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }
    
    public void Launch(Transform targetTransform, float initialAngleDeg)
    {
        target = targetTransform;
        transform.rotation = Quaternion.Euler(0f, 0f, initialAngleDeg);

        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }

        if (lifeTime > 0f)
        {
            Destroy(gameObject, lifeTime);
        }
    }
    
    private void FixedUpdate()
    {
        if (target == null || rb == null)
            return;

        Vector2 toTarget = ((Vector2)target.position - rb.position).normalized;

        float desiredAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = rb.rotation;
        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            desiredAngle,
            rotateSpeedDegPerSec * Time.fixedDeltaTime);

        rb.rotation = newAngle;
        rb.linearVelocity = transform.up * speed;
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform == target || otherCollider.CompareTag("Player"))
        {
            var playerRb = otherCollider.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(randomDir * knockbackForce, ForceMode2D.Impulse);
            }
            
            Explode();
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

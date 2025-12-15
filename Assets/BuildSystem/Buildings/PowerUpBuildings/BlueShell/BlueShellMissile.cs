using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BlueShellMissile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float steeringAcceleration = 30f; 
    [SerializeField] private float lifeTimeSeconds = 8f;
    [SerializeField] private float activationDelaySeconds = 1f;
    
    [SerializeField] private GameObject explosionVfxPrefab;
    [SerializeField] private float knockbackForce = 35f;

    private Rigidbody2D rb;
    private Transform target;

    private float spawnTime;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Launch(Transform targetTransform, float initialAngleDeg)
    {
        spawnTime = Time.time;
        target = targetTransform;
        
        rb.rotation = initialAngleDeg;
        rb.angularVelocity = 0f;
        
        Vector2 forward = Quaternion.Euler(0, 0, rb.rotation) * Vector2.up;
        rb.linearVelocity = forward * speed;

        if (lifeTimeSeconds > 0f)
        {
            Destroy(gameObject, lifeTimeSeconds);
        }
    }
    
    private void FixedUpdate()
    {
        if (target == null || rb == null)
            return;

        var toTarget = target.position - transform.position;
        toTarget.z = 0;
        
        // move the missile upwards until it is able to explode at the target
        if (Time.time < spawnTime + activationDelaySeconds)
        {
            toTarget = Vector3.up;
        }

        var desiredVel = toTarget.normalized * speed;
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            desiredVel,
            steeringAcceleration * Time.fixedDeltaTime
        );

        if (toTarget.sqrMagnitude > 0.001f)
        {
            var missileAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
            rb.MoveRotation(missileAngle);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (Time.time < spawnTime + activationDelaySeconds)
            return;
        
        var randomDir = Random.insideUnitCircle.normalized;
        otherCollider.SendMessageUpwards("ApplyForceImpulseMessage", randomDir * knockbackForce, SendMessageOptions.DontRequireReceiver);
        
        Explode();
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

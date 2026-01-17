using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketMissile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float steeringAcceleration = 30f;
    [SerializeField] private float lifeTimeSeconds = 8f;
    [SerializeField] private float activationDelaySeconds = 1f;

    [SerializeField] private GameObject explosionVfxPrefab;
    [SerializeField] private float knockbackForce = 35f;

    [SerializeField] private AudioSource startSfx;

    private Rigidbody2D rb;
    private Transform target;
    private Vector3 initialDirection;

    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Transform targetTransform, float initialAngleDeg)
    {
        if (startSfx != null)
            startSfx.Play();

        spawnTime = Time.time;
        target = targetTransform;
        initialDirection = Quaternion.Euler(0, 0, initialAngleDeg) * Vector3.up;

        RotateInDirection(initialDirection);
        rb.angularVelocity = 0f;

        Invoke(nameof(Explode), lifeTimeSeconds);
    }

    private void FixedUpdate()
    {
        if (target == null || rb == null)
            return;

        if (!target.gameObject.activeInHierarchy)
        {
            Explode();
            return;
        }

        var toTarget = target.position - transform.position;
        toTarget.z = 0;

        // move the missile straight forwards until it is able to explode at the target
        if (Time.time < spawnTime + activationDelaySeconds)
        {
            toTarget = initialDirection;
        }

        var desiredVel = toTarget.normalized * speed;
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            desiredVel,
            steeringAcceleration * Time.fixedDeltaTime
        );

        if (toTarget.sqrMagnitude > 0.001f)
        {
            RotateInDirection(toTarget);
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
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void RotateInDirection(Vector3 direction)
    {
        // rotatets transform.right towards the target
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }
}

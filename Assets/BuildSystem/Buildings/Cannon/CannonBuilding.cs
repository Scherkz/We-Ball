using UnityEngine;
using System.Collections;
using Unity.VisualScripting;


public class CannonBuilding : Building
{
    private bool isFree;
    private Coroutine shootCoroutine;
    private float shootPower = 15f;
    private Collider2D myCollider;
    private Collider2D otherCollider;
    private GameObject playerGameObject;

    private void Awake()
    {
        isFree = true;
        myCollider = GetComponentInChildren<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (isFree)
        {
            playerGameObject = collision.gameObject;
            isFree = false;
            otherCollider = collision.collider;
            Physics2D.IgnoreCollision(myCollider, otherCollider, true);
            FreezePlayer();
            shootCoroutine = StartCoroutine(ShootAfterDelay());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == playerGameObject)
        {
            if (shootCoroutine != null)
                StopCoroutine(shootCoroutine);
        }
    }

    IEnumerator ShootAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        Shoot();
    }

    private void Shoot()
    {
        DefreezePlayer();
        Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();

        Vector2 dir = Quaternion.Euler(0, 0, -45f) * rotationAnchor.transform.up;
        rb.linearVelocity = dir * shootPower;
    }

    private void FreezePlayer()
    {
        Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider c in playerGameObject.GetComponentsInChildren<Collider>())
            c.enabled = false;

        foreach (SpriteRenderer sp in playerGameObject.GetComponentsInChildren<SpriteRenderer>())
            sp.enabled = false;

        Vector2 midPoint = transform.position;
        midPoint.x += 0.5f;
        midPoint.y += 0.5f;
        playerGameObject.transform.position = midPoint;
    }

    private void DefreezePlayer()
    {
        Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        foreach (Collider c in playerGameObject.GetComponentsInChildren<Collider>())
            c.enabled = true;

        foreach (SpriteRenderer sp in playerGameObject.GetComponentsInChildren<SpriteRenderer>())
            sp.enabled = true;
    }

    private void Update()
    {
        if (isFree) return;

        Vector2 posA = transform.position;
        Vector2 posB = playerGameObject.transform.position;
        float distance = Vector2.Distance(posA, posB);
        if (distance > 2f)
        {
            Physics2D.IgnoreCollision(myCollider, otherCollider, false);
            isFree = true;
            playerGameObject = null;
        }
    }
}
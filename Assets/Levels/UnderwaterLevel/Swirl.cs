using UnityEngine;

public class Swirl : MonoBehaviour
{
    [SerializeField] private float pullStrength; 
    [SerializeField] private float rotationStrength;
    [SerializeField] private float gravityScale;
    [SerializeField] private float levelGravity;

    private Vector2 direction;
    private Vector2 pull;
    private Vector2 rotation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.gravityScale = gravityScale;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.attachedRigidbody.gravityScale = levelGravity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Rigidbody2D rb = collision.attachedRigidbody;

        direction = transform.position - collision.transform.position;
        pull = direction.normalized * pullStrength;
        rotation = new Vector3(direction.y, -direction.x).normalized * rotationStrength;

        rb.AddForce(pull + rotation);


    }
}

using UnityEngine;

public class BlackholeBuilding : Building
{
    public float rotationSpeed = 5;
    public float attractionForce;
    public float flingForce;

    private Transform rotationAnchor;

    private void Awake()
    {
        rotationAnchor = transform.Find("Anchor");
    }

    private void FixedUpdate()
    {
        rotationAnchor.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Attract object
        var direction = transform.position - collision.transform.position;
        collision.attachedRigidbody.AddForce(direction.normalized * (attractionForce / direction.sqrMagnitude));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Fling object in opposite direction
        var direction = collision.otherRigidbody.transform.position - transform.position;
        collision.otherRigidbody.AddForce(direction.normalized * flingForce);
    }

}

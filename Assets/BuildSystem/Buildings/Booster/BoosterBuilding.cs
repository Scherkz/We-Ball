using UnityEngine;

public class BoosterBuilding : Building
{
    public float boostForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(rotationAnchor.transform.right * boostForce, ForceMode2D.Impulse);
    }
}
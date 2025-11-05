using UnityEngine;

public class Windmill : Building
{
    public float rotationSpeed = 5;

    private Transform rotationAnchor;

    private void Awake()
    {
        rotationAnchor = transform.Find("Anchor");
    }

    private void FixedUpdate()
    {
        rotationAnchor.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime));
    }
}

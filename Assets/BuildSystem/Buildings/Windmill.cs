using UnityEngine;

public class Windmill : MonoBehaviour
{
    public float rotationSpeed = 5;

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime));
    }
}

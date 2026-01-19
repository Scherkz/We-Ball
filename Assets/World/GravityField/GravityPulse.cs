using UnityEngine;

public class GravityPulse : MonoBehaviour
{
    private float maxRadius;
    private float currentRadius;
    private float spriteBaseSize;
    private float secondsPerAnimationCycle;
    private float animationSpeed;

    private void Start()
    {
        CircleCollider2D parentCollider = GetComponentInParent<CircleCollider2D>();
        maxRadius = parentCollider.radius;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteBaseSize = sr.sprite.bounds.size.x;

        secondsPerAnimationCycle = 4f;
        animationSpeed = maxRadius / secondsPerAnimationCycle;
    }

    private void Update()
    {
        currentRadius -= animationSpeed * Time.deltaTime;
        if (currentRadius < 0)
        {
            currentRadius = maxRadius;
        }
        float desiredDiameter = currentRadius * 2f;
        float scale = desiredDiameter / spriteBaseSize;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}

using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PushAwayShot : SpecialShot
{
    private PlayerController playerController;
    private Player player;
    private Rigidbody2D body;

    private float maximalImpactRange;

    [Range(0, 50f)]
    [SerializeField] private float maximalImpactForce;

    [SerializeField] private GameObject specializedShotVFX;
    [SerializeField] private GameObject explodeVFX;
    private GameObject currentExplodeVFX;

    [Range(0, 1f)]
    [SerializeField] private float speedWeight;
    private float distanceWeight;

    // Maximum speed a ball can reach with a standard shot, is used to calculate the speed factor
    private const float maxBallSpeed = 20f;

    public void Awake()
    {
        distanceWeight = 1 - speedWeight;
    }

    public override void Init(PlayerController playerController, Player player, Rigidbody2D body)
    {
        this.playerController = playerController;
        maximalImpactRange = playerController.GetMaximalCollisionRange();
        this.player = player;
        this.body = body;

        if (playerController != null)
        {
            playerController.BallEnterCollisionEvent += HandleCollision;
            playerController.OnToggleSpecialShotVFX += ToggleSpecialShotVFX;
        }

        currentSpecializedShotVFX = Instantiate(specializedShotVFX, playerController.transform);
        currentSpecializedShotVFX.SetActive(false);
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.BallEnterCollisionEvent -= HandleCollision;
            playerController.OnToggleSpecialShotVFX -= ToggleSpecialShotVFX;
        }

        if (currentExplodeVFX != null)
        {
            Destroy(currentExplodeVFX);
        }

    }

    // Handle collision for push-away shots
    private void HandleCollision(Collision2D collision)
    {
        if (!playerController.IsSpecialShotEnabled()) return;

        PushAwayImpact(collision);
        player.UsedSpecialShot();
        playerController.DisableSpecialShot();

    }

    private void PushAwayImpact(Collision2D collision)
    {
        // Impact from current player position
        Vector2 impactPosition = transform.position;
        // Get all rigidbodies in a radius
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(impactPosition, maximalImpactRange);
        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            // Only affect player rigidbodies
            if (!overlappingCollider.CompareTag("Player")) continue;

            // Apply force away from impact position
            Rigidbody2D otherBallBody = overlappingCollider.GetComponent<Rigidbody2D>();
            if (otherBallBody != null && otherBallBody != this.body)
            {
                Vector2 pushDirection = (otherBallBody.position - impactPosition).normalized;

                float distance = Vector2.Distance(otherBallBody.position, impactPosition);
                float distanceFactor = Mathf.SmoothStep(1f, 0, distance / maximalImpactRange);

                float speedFactor = Mathf.SmoothStep(0, 1f, this.body.linearVelocity.magnitude / maxBallSpeed);

                float forceFactor = speedFactor * speedWeight + distanceFactor * distanceWeight;
                float forceMagnitude = Mathf.Lerp(0f, maximalImpactForce, forceFactor);
                otherBallBody.AddForce(pushDirection * forceMagnitude, ForceMode2D.Impulse);
            }
        }

        Destroy(currentSpecializedShotVFX);
        currentExplodeVFX = Instantiate(explodeVFX, playerController.transform);
    }
}

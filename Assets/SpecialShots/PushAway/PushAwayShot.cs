using System;
using UnityEngine;

public class PushAwayShot : SpecialShot
{
    private bool collisionHappenedDuringPushAwayShot = false;

    PlayerController playerController;
    Player player;
    Rigidbody2D body;

    public override void Init(PlayerController playerController, Player player, Rigidbody2D body)
    {
        this.playerController = playerController;
        this.player = player;
        this.body = body;
        
        if(playerController != null)
        {
            playerController.BallCollisionEvent += HandleCollision;
        }
        
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.BallCollisionEvent -= HandleCollision;
        }

    }

    // Handle collision for push-away shots
    private void HandleCollision(Collision2D collision)
    {
        // Dont trigger in the spawing phase
        if (!playerController.IsFirstShotTakenAfterRoundStart()) return;

        if (!playerController.IsSpecialShotEnabled()) return;

        if (collisionHappenedDuringPushAwayShot) return;

        PushAwayImpact(collision);
        player.UsedSpecialShot();
        collisionHappenedDuringPushAwayShot = true;

    }

    private void PushAwayImpact(Collision2D collision)
    {
        Debug.Log("PushAwayImpact triggered");

        // Impact from current player position
        Vector2 impactPosition = transform.position;
        // Get all rigidbodies in a radius
        float maximalImpactRange = 10f;
        float maximalImpactForce = 35f;
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(impactPosition, maximalImpactRange);
        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            // Only affect player rigidbodies
            if (!overlappingCollider.CompareTag("Player")) continue;

            // Apply force away from impact position
            Rigidbody2D otherBallBody = overlappingCollider.GetComponent<Rigidbody2D>();
            if (otherBallBody != null && otherBallBody != this.body)
            {
                // Only the faster ball should apply a force to the other balls
                // Prevents applying forces in both directions
                if (otherBallBody.linearVelocity.magnitude > this.body.linearVelocity.magnitude) continue;

                Vector2 pushDirection = (otherBallBody.position - impactPosition).normalized;
                float distance = Vector2.Distance(otherBallBody.position, impactPosition);
                float forceMagnitude = Mathf.Lerp(maximalImpactForce, 0f, (float)Math.Pow((distance / maximalImpactRange), 2));
                otherBallBody.AddForce(pushDirection * forceMagnitude, ForceMode2D.Impulse);
            }
        }
    }
}

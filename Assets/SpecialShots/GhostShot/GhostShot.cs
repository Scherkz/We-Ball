using UnityEngine;

public class GhostShot : SpecialShot
{
    private PlayerController playerController;
    private Player player;
    private SpriteRenderer ballSprite;

    [SerializeField] private GameObject specializedShotVFX;

    public override void Init(PlayerController playerController, Player player, Rigidbody2D body)
    {
        this.playerController = playerController;
        this.player = player;

        if (playerController != null)
        {
            playerController.BallExitBuildingTriggerEvent += ExitCollisionObject;
            playerController.OnToggleSpecialShotVFX += ToggleSpecialShotVFX;
            playerController.OnToggleSpecialShotActivation += ToggleSpecialShotActivation;
            ballSprite = playerController.GetComponent<SpriteRenderer>();
        }

        currentSpecializedShotVFX = Instantiate(specializedShotVFX, playerController.transform);
        currentSpecializedShotVFX.SetActive(false);
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.BallExitBuildingTriggerEvent -= ExitCollisionObject;
            playerController.OnToggleSpecialShotVFX -= ToggleSpecialShotVFX;
            playerController.OnToggleSpecialShotActivation -= ToggleSpecialShotActivation;
        }

        if (ballSprite != null)
            ballSprite.color = new Color(ballSprite.color.r, ballSprite.color.g, ballSprite.color.b, 1f);

    }

    private void ToggleSpecialShotActivation(bool enable)
    {
        if(enable)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        this.playerController.transform.gameObject.layer = LayerMask.NameToLayer("GhostBall");
        ballSprite.color = new Color(ballSprite.color.r, ballSprite.color.g, ballSprite.color.b, 0.75f);
    }

    private void Deactivate()
    {
        this.playerController.transform.gameObject.layer = LayerMask.NameToLayer("Player");
        ballSprite.color = new Color(ballSprite.color.r, ballSprite.color.g, ballSprite.color.b, 1f);
    }

    private void ExitCollisionObject(Collider2D collider)
    {
        if (!playerController.IsSpecialShotEnabled()) return;

        Deactivate();

        player.UsedSpecialShot();
        playerController.DisableSpecialShot();

        Destroy(currentSpecializedShotVFX);
    }
}

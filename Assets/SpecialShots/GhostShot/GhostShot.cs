using UnityEngine;

public class GhostShot : SpecialShot
{
    private PlayerController playerController;
    private Player player;
    private Rigidbody2D body;

    [SerializeField] private GameObject specializedShotVFX;

    public override void Init(PlayerController playerController, Player player, Rigidbody2D body)
    {
        this.playerController = playerController;
        this.player = player;
        this.body = body;

        if (playerController != null)
        {
            playerController.BallExitBuildingTriggerEvent += ExitCollisionObject;
            playerController.OnToggleSpecialShotVFX += ToggleSpecialShotVFX;
            playerController.OnToggleSpecialShotActivation += ToggleSpecialShotActivation;
        }

        currentSpecializedShotVFX = Instantiate(specializedShotVFX, playerController.transform);
        currentSpecializedShotVFX.SetActive(false);
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.BallExitBuildingTriggerEvent -= ExitCollisionObject;
        }

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
        Debug.Log("Ghost Shot: Activated - Player is now a GhostBall");
        Debug.Log("Ghost Shot: Current Layer is " + LayerMask.LayerToName(this.playerController.transform.gameObject.layer));
    }

    private void Deactivate()
    {
        this.playerController.transform.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void ExitCollisionObject(Collider2D collider)
    {
        Debug.Log("Ghost Shot: Exiting collision with " + collider.gameObject.name);

        Deactivate();

        player.UsedSpecialShot();
        playerController.DisableSpecialShot();

        Destroy(currentSpecializedShotVFX);
    }
}

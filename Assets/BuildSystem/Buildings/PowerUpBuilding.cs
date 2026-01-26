using UnityEngine;

public abstract class PowerUpBuilding : Building
{
    [SerializeField] protected GameObject collectVfxPrefab;

    private bool isCollected;

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (isCollected)
            return;

        if (!collider.gameObject.CompareTag("Player"))
            return;

        var pickupPlayer = collider.GetComponentInParent<Player>();
        var controller = pickupPlayer.GetPlayerController();

        isCollected = true;

        if (collectVfxPrefab != null)
        {
            Instantiate(collectVfxPrefab, transform.position, Quaternion.identity);
        }

        OnCollected(pickupPlayer, controller);

        var buildGrid = GetComponentInParent<BuildGrid>();
        if (buildGrid != null)
        {
            buildGrid.RemoveBuildingAtPosition(transform.position);
            return;
        }
    }

    //should be overwritten by each power up
    protected abstract void OnCollected(Player player, PlayerController controller);
}

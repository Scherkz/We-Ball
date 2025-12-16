using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class PowerUpBuilding : Building
{
    [SerializeField] protected GameObject collectVfxPrefab;
    
    private bool isCollected;
    
    private SpriteRenderer[] renderers = { };
    private Collider2D[] colliders = { };
    
    //To activate in GameManager, to reset all the hidden power-ups
    public virtual void ResetForNextRound()
    {
        isCollected = false;
        SetVisible(true);
    }
    
    protected virtual void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        colliders = GetComponentsInChildren<Collider2D>(true);

        foreach (var col in colliders)
        {
            col.isTrigger = true;
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (isCollected)
            return;
        
        Player pickupPlayer = collider.GetComponentInParent<Player>();
        if (!collider.gameObject.CompareTag("Player"))
            return;

        PlayerController controller = collider.GetComponent<PlayerController>();
        if (controller == null)
            return;
        
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
        
        Destroy(gameObject);
    }
    
    //should be overwritten by each power up
    protected abstract void OnCollected(Player player, PlayerController controller);
    
    //activates or deactivates all the collider and sprites 
    private void SetVisible(bool value)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = value;
        }
        
        foreach (var col in colliders)
        {
            col.enabled = value;
        }
    }
}

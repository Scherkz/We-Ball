using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class PowerUpBuilding : Building
{
    public bool hideOnPickupInsteadOfDestroy = false;
    
    public GameObject collectVfxPrefab;
    
    private bool isCollected;
    
    private SpriteRenderer[] renderers;
    private Collider2D[] colliders;
    
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
    
    protected virtual void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if (isCollected)
            return;

        PlayerController controller = playerCollider.GetComponent<PlayerController>();
        Player pickupPlayer = playerCollider.GetComponentInParent<Player>();
        
        if (pickupPlayer == null || controller == null) return;
        
        isCollected = true;

        if (collectVfxPrefab != null)
        {
            Instantiate(collectVfxPrefab, transform.position, Quaternion.identity);
        }

        OnCollected(pickupPlayer, controller);

        if (hideOnPickupInsteadOfDestroy)
        {
            SetVisible(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    //should be overwritten by each power up
    protected abstract void OnCollected(Player player, PlayerController controller);
    
    //activates or deactivates all the collider and sprites 
    private void SetVisible(bool value)
    {
        if (renderers != null)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = value;
            }
        }

        if (colliders != null)
        {
            foreach (var col in colliders)
            {
                col.enabled = value;
            }
        }
    }
}

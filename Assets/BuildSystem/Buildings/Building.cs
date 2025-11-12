using UnityEngine;

public class Building : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void SetTint(Color tint)
    {
        spriteRenderer.color = tint;
    }

    public virtual void SetRenderingOrder(int sortingLayerId, int sortingOrder)
    {
        spriteRenderer.sortingLayerID = sortingLayerId;
        spriteRenderer.sortingOrder = sortingOrder;
    }
}

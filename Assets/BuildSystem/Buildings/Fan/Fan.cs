using UnityEngine;

public class Fan : Building
{
    public float force;

    private SpriteRenderer windEffectSpriteRenderer;

    private void Awake()
    {
        windEffectSpriteRenderer = transform.Find("EffectArea").GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(transform.up * force);
    }

    public override void SetRenderingOrder(int sortingLayerId, int sortingOrder)
    {
        base.SetRenderingOrder(sortingLayerId, sortingOrder);

        windEffectSpriteRenderer.sortingLayerID = sortingLayerId;
        windEffectSpriteRenderer.sortingOrder = sortingOrder;
    }
}

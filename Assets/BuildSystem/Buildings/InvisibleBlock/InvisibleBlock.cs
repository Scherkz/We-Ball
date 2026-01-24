using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : Building
{
    [SerializeField] protected GameObject collectVfxPrefab;

    public bool isInvisibleBuilding = true;

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player entered invisible block area.");
        if (!collision.otherRigidbody.gameObject.CompareTag("Player"))
            return;

        var player = collision.otherRigidbody.GetComponentInParent<Player>();
        if (player == null)
            return;

        SetVisibility(false);
    }

    public void SetVisibility(bool visibility)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visibility;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void SetTint(Color tint)
    {
        spriteRenderer.color = tint;
    }
}

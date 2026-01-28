using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class InvisibleBlock : Building
{
    [SerializeField] protected GameObject collectVfxPrefab;

    public bool isInvisibleBuilding = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(Pulse());
    }

    public IEnumerator Pulse()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float fadeInDuration = 0.3f;
            float fadeOutDuration = 0.2f;
            float elapsed = 0f;
            
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;

            spriteRenderer.enabled = true;
            
            // Fade in
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsed / fadeInDuration);
                spriteRenderer.color = color;
                yield return null;
            }
            
            // Fade out
            elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(1f - (elapsed / fadeOutDuration));
                spriteRenderer.color = color;
                yield return null;
            }
            spriteRenderer.enabled = false;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

}

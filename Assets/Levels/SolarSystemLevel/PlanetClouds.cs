using UnityEngine;

public class PlanetClouds : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;

    private float loopOffset;
    private float startOffset;

    private void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        loopOffset = spriteRenderer.size.x * 0.5f * Mathf.Abs(transform.localScale.x);
        startOffset = loopOffset * Random.value;
    }

    private void Update()
    {
        float xOffset = Mathf.Repeat(Time.time * scrollSpeed + startOffset, loopOffset);
        float x = -(loopOffset * 0.5f) + xOffset;
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }
}

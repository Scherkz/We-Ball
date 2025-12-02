using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MapNode : MonoBehaviour
{
    // The map name must match the scene name of the level
    [SerializeField] public string mapName;
    [SerializeField] public Sprite mapIcon;

    private void Start()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = mapIcon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessageUpwards("OnMapNodeVoted", this, SendMessageOptions.DontRequireReceiver);
    }
}
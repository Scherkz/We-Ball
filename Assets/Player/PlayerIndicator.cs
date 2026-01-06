using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] private GameObject playerBall; // player ball

    private Camera worldCam;
    private SpriteRenderer spriteRenderer;
    private const float EDGE_PADDING = 0.025f;

    private void OnEnable()
    {
            EventBus.Instance.OnLevelLoaded += SearchForCamera;
    }

    private void OnDisable()
    {
            EventBus.Instance.OnLevelLoaded -= SearchForCamera;
    }

    private void Awake()
    {
        worldCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerBall == null)
        {
            Debug.Log("Please assign player ball to playerIndicator");
        }
    }

    private void Update()
    {
        if (playerBall == null || worldCam == null) return;

        Vector3 viewportPosition = worldCam.WorldToViewportPoint(playerBall.transform.position);

        bool offscreen =
            viewportPosition.x < 0 || viewportPosition.x > 1 ||
            viewportPosition.y < 0 || viewportPosition.y > 1;

        spriteRenderer.enabled = offscreen;

        if (!offscreen) return;

        spriteRenderer.flipX = viewportPosition.x < 0.5f; // if the player ball is out of bounds on the left side, flip the icon to the left (flip x values)
        spriteRenderer.flipY = viewportPosition.y < 0.5f; // if the player ball is out of bounds at the bottom, flip the icon upside down (flip y values)

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, EDGE_PADDING, 1f - EDGE_PADDING);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, EDGE_PADDING, 1f - EDGE_PADDING);

        Vector3 worldPos = worldCam.ViewportToWorldPoint(viewportPosition);
        transform.position = worldPos;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    private void SearchForCamera(Level level, bool isLobby)
    {
        worldCam = Camera.main;
    }
}

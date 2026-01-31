using UnityEngine;

public class UnderwaterLevel : Level
{
    [SerializeField] private PlayerRegistry playerRegistry;
    [SerializeField] private float levelGravity;

    private new void Start()
    {
        foreach (var player in playerRegistry.players)
        {
            SetupPlayerGravityScale(player, levelGravity);
        }

        base.Start();
    }

    private void OnDestroy()
    {
        foreach (var player in playerRegistry.players)
        {
            SetupPlayerGravityScale(player, 1f);
        }
    }

#if UNITY_EDITOR
    // if level gets loaded in lobby mode make sure to setup joining players correctly
    private void OnEnable()
    {
        EventBus.Instance.OnPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(Player player)
    {
        this.CallNextFrame(SetupPlayerGravityScale, player, levelGravity);
    }
#endif

    private void SetupPlayerGravityScale(Player player, float gravityScale)
    {
        if (player == null)
            return;

        var playerController = player.GetPlayerController();
        if (playerController == null)
            return;

        var rb = playerController.GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }
}
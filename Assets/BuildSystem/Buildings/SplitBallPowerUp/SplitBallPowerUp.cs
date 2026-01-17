using UnityEngine;
using UnityEngine.InputSystem;

public class SplitBallPowerUp : PowerUpBuilding
{
    [SerializeField] private PlayerRegistry playersRegistry;
    [SerializeField] private float colorSwitchSeconds = 0.5f;

    [SerializeField] private float spawnOffset = 0.4f;
    [SerializeField] private float spreadDegrees = 45f;

    private Color[] colors;
    private int colorIndex = 0;
    private float secondsSinceLastSwitch = 0.0f;

    private void Start()
    {
        colors = new Color[playersRegistry.players.Count];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = playersRegistry.players[i].GetColor();
        }

        spriteRenderer.color = colors[colorIndex];
    }

    private void Update()
    {
        secondsSinceLastSwitch += Time.deltaTime;

        if (secondsSinceLastSwitch > colorSwitchSeconds)
        {
            colorIndex = (colorIndex + 1) % colors.Length;
            spriteRenderer.color = colors[colorIndex];

            secondsSinceLastSwitch = 0;
        }
    }

    protected override void OnCollected(Player player, PlayerController controller)
    {
        var playerInput = player.GetComponent<PlayerInput>();

        // Clone Player
        var cloneInput = PlayerInput.Instantiate(
            player.gameObject,
            controlScheme: "Gamepad",
            pairWithDevice: playerInput.devices[0]
        );

        // Setup Clone
        var clonePlayer = cloneInput.gameObject.AddComponent<ClonePlayer>();
        clonePlayer.CallNextFrame(clonePlayer.Setup, player, spreadDegrees);

        // Apply spread
        var playerRb = player.GetPlayerController().GetComponent<Rigidbody2D>();

        var moveDir = playerRb.linearVelocity;
        playerRb.linearVelocity = Quaternion.Euler(0, 0, spreadDegrees * 0.5f) * moveDir;

        var orthoDir = new Vector2(-moveDir.y, moveDir.x).normalized;
        player.transform.position += (Vector3) (spawnOffset * orthoDir);
        clonePlayer.transform.position -= (Vector3) (spawnOffset * orthoDir);
    }
}

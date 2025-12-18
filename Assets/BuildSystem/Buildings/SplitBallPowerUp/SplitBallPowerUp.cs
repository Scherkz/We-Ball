using UnityEngine;
using UnityEngine.InputSystem;

public class SplitBallPowerUp : PowerUpBuilding
{
    [SerializeField] private Vector2 cloneSpawnOffset = new (0.6f, 0f);
    [SerializeField] private float spreadDegrees = 45f;

    protected override void OnCollected(Player player, PlayerController controller)
    {
        var originalBall = controller.gameObject;
        
        var originalRb = originalBall.GetComponent<Rigidbody2D>();
        var snapLinearVel = originalRb != null ? originalRb.linearVelocity : Vector2.zero;
        var snapAngularVel = originalRb != null ? originalRb.angularVelocity : 0f;
        
        var clone = Instantiate(originalBall, originalBall.transform.parent);
        clone.name = originalBall.name + "_Clone";
        
        clone.transform.position = originalBall.transform.position + (Vector3)cloneSpawnOffset;
        clone.transform.rotation = originalBall.transform.rotation;
        
        var cloneController = clone.GetComponent<PlayerController>();
        if (cloneController != null)
        {
            cloneController.SetResetOnStart(false);          
            cloneController.SetColor(player.GetColor());     
        }
        
        var cloneRb = clone.GetComponent<Rigidbody2D>();
        if (cloneRb != null)
        {
            var speed =snapLinearVel.magnitude;
            var dir = snapLinearVel / speed;

            var angle = Random.value < 0.5f ? -spreadDegrees : spreadDegrees;
            var spreadDir = (Vector2)(Quaternion.Euler(0f, 0f, angle) * dir);

            var perp = new Vector2(-dir.y, dir.x);
            var side = Mathf.Sign(angle);
            clone.transform.position = originalBall.transform.position + (Vector3)(perp * cloneSpawnOffset * side);

            cloneRb.linearVelocity = spreadDir * speed;
            cloneRb.angularVelocity = snapAngularVel;
            cloneRb.WakeUp();
        }
        
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null && cloneController != null)
        {
            var forwarder = clone.AddComponent<CloneInputForwarder>();
            forwarder.Init(playerInput, cloneController);
        }
        
        var lifetimerGO = new GameObject($"SplitBallLifetime_{player.name}");
        var lifetimer = lifetimerGO.AddComponent<SplitBallLifetime>();
        lifetimer.Init(player, clone);
    }
}

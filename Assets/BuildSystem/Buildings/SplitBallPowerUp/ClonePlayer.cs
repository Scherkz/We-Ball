using UnityEngine;

public class ClonePlayer : MonoBehaviour
{
    private Player original;
    private Player clone;

    public void Setup(Player player, float spreadDegrees)
    {
        original = player;
        clone = GetComponent<Player>();

        original.OnFinishedRound += OnPlayerFinishedRound;
        clone.OnFinishedRound += OnCloneFinishedRound;

        var originalController = original.GetPlayerController();
        var cloneController = clone.GetPlayerController();
        cloneController.SetColor(original.GetColor());
        cloneController.DontResetOnStart(false);

        clone.StartPlayingPhase(cloneController.transform.position);

        var playerRb = originalController.GetComponent<Rigidbody2D>();
        var cloneRb = cloneController.GetComponent<Rigidbody2D>();
        cloneRb.linearVelocity = Quaternion.Euler(0, 0, -spreadDegrees) * playerRb.linearVelocity;
        cloneRb.angularVelocity = playerRb.angularVelocity;
    }

    private void OnDestroy()
    {
        original.OnFinishedRound -= OnPlayerFinishedRound;
    }

    private void OnPlayerFinishedRound()
    {
        Destroy(gameObject);
    }

    private void OnCloneFinishedRound()
    {
        original.SendMessage("OnEnterFinishArea");
        Destroy(gameObject);
    }
}

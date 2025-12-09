using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        Debug.Log(collision.tag);
        var player = collision.transform.parent.gameObject.GetComponent<Player>();
        var playerController = collision.transform.gameObject.GetComponent<PlayerController>();
        var spawnPoint = player.getSpawnPoint();
        playerController.transform.position = spawnPoint;
        playerController.ResetSelf();
    }
}

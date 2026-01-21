using UnityEngine;

public class KillArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessageUpwards("OnEnterKillAreaMessage", null, SendMessageOptions.DontRequireReceiver);
    }
}

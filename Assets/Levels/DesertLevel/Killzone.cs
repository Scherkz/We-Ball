using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessageUpwards("OnEnterKillArea", null, SendMessageOptions.DontRequireReceiver);
    }
}

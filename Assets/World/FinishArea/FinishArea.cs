using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FinishArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessageUpwards("OnEnterFinishArea", null, SendMessageOptions.DontRequireReceiver);
    }
}

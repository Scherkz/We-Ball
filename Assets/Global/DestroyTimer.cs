using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float destroyDelaySeconds = 0f;

    private void Start()
    {
        Destroy(gameObject, destroyDelaySeconds);
    }
}

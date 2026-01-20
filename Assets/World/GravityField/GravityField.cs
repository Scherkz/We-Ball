using UnityEngine;

public class GravityField : MonoBehaviour
{
    private const string TimeOffsetShaderPropertyName = "_TimeOffset";

    public float gravityFactor = 9.81f;

    [SerializeField][Range(0f, 1f)] private float visualizersOffset = 0.8f;

    private void Awake()
    {
        var radius = GetComponent<CircleCollider2D>().radius;
        var offset = Random.value;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name.StartsWith("Visualizer"))
            {
                child.transform.localScale = new Vector3(radius, radius, 1);

                var propBlock = new MaterialPropertyBlock();
                Debug.Log(offset);
                propBlock.SetFloat(TimeOffsetShaderPropertyName, offset);
                child.GetComponent<SpriteRenderer>().SetPropertyBlock(propBlock);

                offset += visualizersOffset;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var dir = transform.position - collision.transform.position;
        var gravity = dir.normalized * gravityFactor / dir.sqrMagnitude;
        collision.attachedRigidbody.AddForce(gravity, ForceMode2D.Force);
    }
}

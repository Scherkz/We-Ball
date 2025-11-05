using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingBallSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnIntervallSeconds = 1;
    public int maxElements = 10;
    public float randomForce = 5;

    private List<GameObject> queue = new();

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnIntervallSeconds);

            var instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            instance.GetComponent<Rigidbody2D>().AddForce(GetRandomDirection() * randomForce);

            queue.Add(instance);
            if (queue.Count > maxElements)
            {
                var oldest = queue[0];
                queue.RemoveAt(0);
                Destroy(oldest);
            }
        }
    }

    private Vector3 GetRandomDirection()
    {
        var randomAngle = Random.Range(0, 2 * Mathf.PI);
        return new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0);
    }
}

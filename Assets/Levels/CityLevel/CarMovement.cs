using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    public Transform pointA;        // spawn point
    public Transform pointB;        // target point
    public float moveDuration = 2f; // time to move one way
    public float interval = 10f;    // wait time after full cycle

    private void Start()
    {
        transform.position = pointA.position;
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            // Move A to B
            yield return StartCoroutine(MoveOverTime(pointA.position, pointB.position));

            // Move B to A
            yield return StartCoroutine(MoveOverTime(pointB.position, pointA.position));

            // Wait before next loop
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator MoveOverTime(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }
}

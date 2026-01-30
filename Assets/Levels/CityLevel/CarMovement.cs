using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    public Transform pointA; 
    public Transform pointB;        
    public float moveDuration = 2f; 
    public float interval = 10f;    

    private void Start()
    {
        transform.position = pointA.position;
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            yield return StartCoroutine(MoveOverTime(pointA.position, pointB.position));
            yield return StartCoroutine(MoveOverTime(pointB.position, pointA.position));
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

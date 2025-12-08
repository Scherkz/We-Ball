using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Dune : MonoBehaviour
{
    //TODO change public to serialize field
    public float eruptionInterval = 10f;
    public float eruptionDuration = 3f;
    public float pushForce = 1.5f;
    public float eruptionSpeed = 5f;
    public Transform bottomPos;
    public Transform topPos;

    private bool isErupting = false;

    private List<Rigidbody2D> playersInGeyser = new List<Rigidbody2D>();

    private void Start()
    {
        StartCoroutine(EruptionLoop());
    }

    private void Update()
    {
        if (isErupting)
        {
            foreach (var rb in playersInGeyser)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 2f);
            }
        }
    }


    private IEnumerator EruptionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(eruptionInterval);
            StartCoroutine(Erupt());
        }
    }

    private IEnumerator Erupt()
    {
        isErupting = true;

        Vector3 start = bottomPos.position;
        Vector3 end = topPos.position ;
        float distance = Vector3.Distance(start, end);
        float travelTime = distance / eruptionSpeed;
        float t = 0;

        while (t < travelTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / travelTime);

            foreach (var rb in playersInGeyser)
            {
                rb.AddForce(Vector2.up * pushForce);
            }

            yield return null;
        }

        yield return new WaitForSeconds(eruptionDuration);

        transform.position = start;
        isErupting = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            playersInGeyser.Add(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            playersInGeyser.Remove(other.attachedRigidbody);
        }
    }
}

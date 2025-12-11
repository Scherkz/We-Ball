using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dune : MonoBehaviour
{
    //TODO change public to serialize field
    public float eruptionInterval = 10f;
    public float eruptionDuration = 3f;
    public float pushForce = 32f;
    public float eruptionSpeed = 5f;
    public Transform bottomPos;
    public Transform topPos;

    private List<Rigidbody2D> playersInGeyser = new List<Rigidbody2D>();

    private void Start()
    {
        StartCoroutine(EruptionLoop());
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
        Vector3 startPosition = bottomPos.position;
        Vector3 targetPosition = topPos.position;
        float timeElapsed = 0f;

        while (timeElapsed < eruptionDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / eruptionDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        transform.position = targetPosition;

        yield return new WaitForSeconds(eruptionDuration);

        //TODO change to fade out
        transform.position = startPosition;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(transform.up * pushForce);
    }
}

using System.Collections;
using UnityEngine;

public class SandFountain : MonoBehaviour
{
    [SerializeField] private float eruptionHeight = 10f;
    [SerializeField] private float eruptionInterval = 10f;
    [SerializeField] private float eruptionDuration = 3f;
    [SerializeField] private float pushForce = 32f;

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
        var startPosition = transform.position;
        var targetPosition = startPosition;
        targetPosition.y += eruptionHeight;

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

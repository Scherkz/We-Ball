using System.Collections;
using UnityEngine;

public class CannonBuilding : Building
{
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float shootChargeSeconds = 1f;
    [SerializeField] private float shootPower = 15f;

    private Animator animator;

    private bool isOccupied;
    private GameObject player;

    private bool filterTriggerExitEvent = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        isOccupied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOccupied)
            return;

        isOccupied = true;
        filterTriggerExitEvent = true;

        player = collision.gameObject;
        player.SetActive(false);
        player.transform.position = shootPosition.position;

        StartCoroutine(ChargeCoroutine());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (filterTriggerExitEvent)
            return;

        if (collision.gameObject == player)
        {
            isOccupied = false;
            player = null;
        }
    }

    IEnumerator ChargeCoroutine()
    {
        yield return new WaitForSeconds(shootChargeSeconds);

        if (player != null)
            animator.SetTrigger("Shoot");
    }

    // Gets triggered by animator for timing with keyframes
    private void Shoot()
    {
        if (player == null)
        {
            isOccupied = false;
            filterTriggerExitEvent = false;
            player = null;
            return;
        }

        player.SetActive(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        Vector2 dir = Quaternion.Euler(0, 0, -45f) * rotationAnchor.transform.up;
        rb.linearVelocity = dir * shootPower;

        filterTriggerExitEvent = false;
    }
}
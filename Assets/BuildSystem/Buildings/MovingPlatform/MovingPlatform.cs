using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Building
{
    [Header("Platform Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;

    [Header("Movement Points")]
    [SerializeField] private Transform startPointVertical;
    [SerializeField] private Transform endPointVertical;
    [SerializeField] private Transform startPointHorizontal;
    [SerializeField] private Transform endPointHorizontal;

    [Header("Point Markers")]
    [SerializeField] private GameObject startMarker;
    [SerializeField] private GameObject endMarker;

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private Transform currentStartPoint;
    private Transform currentEndPoint;
    private Vector3 realStart;
    private Vector3 realEnd;
    private Coroutine moveRoutine;
    private readonly List<Rigidbody2D> playersOnTop = new List<Rigidbody2D>();

    public override void Init()
    {
        base.Init();

        if (transform == null || currentStartPoint == null || currentEndPoint == null) return;

        boxCollider2D = transform.Find("Platform").GetComponent<BoxCollider2D>();
        rigidBody2D = transform.Find("Platform").GetComponent<Rigidbody2D>();

        Vector2 closestPlatformPointToStart = boxCollider2D.ClosestPoint(currentStartPoint.position);
        Vector2 startOffset = rigidBody2D.position - closestPlatformPointToStart;
        realStart = (Vector2) currentStartPoint.position + startOffset;

        Vector2 closestPlatformPointToEnd = boxCollider2D.ClosestPoint(currentEndPoint.position);
        Vector2 endOffset = rigidBody2D.position - closestPlatformPointToEnd;
        realEnd = (Vector2) currentEndPoint.position + endOffset;

        rigidBody2D.position = realStart;

        //startMarker.SetActive(false);
        //endMarker.SetActive(false);

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveLoop());
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            yield return MoveTo(realEnd);
            yield return new WaitForSeconds(waitTime);
            yield return MoveTo(realStart);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while ((rigidBody2D.position - (Vector2)target).sqrMagnitude > 0.001f)
        {
            Vector3 nextPosition =  Vector3.MoveTowards(
                rigidBody2D.position,
                target,
                moveSpeed * Time.fixedDeltaTime
            );
            Vector2 platformVelocity = ((Vector2)nextPosition - rigidBody2D.position) / Time.fixedDeltaTime;

            rigidBody2D.MovePosition(nextPosition);

            foreach (var player in playersOnTop)
            {
                if (player != null && player.IsTouching(boxCollider2D))
                {
                    Vector2 relativeVelocity = platformVelocity - player.linearVelocity;
                    player.AddForce(relativeVelocity * player.mass, ForceMode2D.Force);
                }

            }

            yield return new WaitForFixedUpdate();
        }

        rigidBody2D.MovePosition(target);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb && !playersOnTop.Contains(rb))
            {
                playersOnTop.Add(rb);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb && playersOnTop.Contains(rb))
            {
                playersOnTop.Remove(rb);
            }
        }
    }

    public override Rotation GetNextRotation(bool _clockWise)
    {
        return rotation switch
        {
            Rotation.Degree0 => Rotation.Degree180,
            Rotation.Degree180 => Rotation.Degree0,
            _ => Rotation.Degree0,
        };
    }

    //protected override void RotateSelf(BuildingData _buildingData, Rotation rotation)
    //{
    //    switch (rotation)
    //    {
    //        default:
    //        case Rotation.Degree0:
    //            currentStartPoint = startPointHorizontal;
    //            currentEndPoint = endPointHorizontal;
    //            UpdateMarkers();
    //            break;
    //        case Rotation.Degree180:
    //            currentStartPoint = startPointVertical;
    //            currentEndPoint = endPointVertical;
    //            UpdateMarkers();
    //            break;
    //    }
    //}

    //private void UpdateMarkers()
    //{
    //    if (startMarker == null || endMarker == null) return;

    //    startMarker.transform.localPosition = currentStartPoint.localPosition;
    //    endMarker.transform.localPosition = currentEndPoint.localPosition;

    //    startMarker.SetActive(true);
    //    endMarker.SetActive(true);
    //}
}

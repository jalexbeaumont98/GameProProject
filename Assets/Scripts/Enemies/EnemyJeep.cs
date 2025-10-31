using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJeep : Enemy
{
    [Header("References")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float stopThreshold = 0.05f; // how close counts as "arrived"

    [Header("Detection & Distance")]
    [SerializeField] private float detectRange = 7f;
    [SerializeField] private float idealDistance = 3f;     // desired distance from player (X plane)
    [SerializeField] private float distanceDeadZone = 0.25f; // tolerance around ideal distance

    [Header("Sprite Facing")]
    [SerializeField] private SpriteRenderer sprite;   // optional: assign; if null we'll try GetComponentInChildren

    private float minX, maxX;
    private Vector2 patrolTarget; // current patrol target (x switches between A and B)

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();

        // Determine patrol bounds from the two points (only X matters for a ground enemy)
        minX = Mathf.Min(pointA.position.x, pointB.position.x);
        maxX = Mathf.Max(pointA.position.x, pointB.position.x);

        // Start by heading toward the closer end
        patrolTarget = (Mathf.Abs(transform.position.x - pointA.position.x) <
                        Mathf.Abs(transform.position.x - pointB.position.x))
                        ? (Vector2)pointB.position : (Vector2)pointA.position;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            MoveOnce(ComputePatrolX());
            return;
        }

        float playerDistX = Mathf.Abs(player.position.x - transform.position.x);
        bool playerDetected = playerDistX <= detectRange;

        float desiredX = playerDetected ? ComputeChaseX() : ComputePatrolX();
        MoveOnce(desiredX);
    }

    private float ComputePatrolX()
    {
        float dir = Mathf.Sign(patrolTarget.x - transform.position.x);
        float nextX = Mathf.MoveTowards(transform.position.x, patrolTarget.x, patrolSpeed * Time.fixedDeltaTime);

        // Flip
        FaceDirection(dir);

        if (Mathf.Abs(transform.position.x - patrolTarget.x) <= stopThreshold)
        {
            patrolTarget = (Mathf.Approximately(patrolTarget.x, pointA.position.x))
                           ? (Vector2)pointB.position
                           : (Vector2)pointA.position;
        }

        return nextX;
    }

    private float ComputeChaseX()
    {
        float dx = player.position.x - transform.position.x;   // + means player to the right
        float absDx = Mathf.Abs(dx);

        float dir = 0f;
        if (absDx > idealDistance + distanceDeadZone) dir = Mathf.Sign(dx);      // approach
        else if (absDx < idealDistance - distanceDeadZone) dir = -Mathf.Sign(dx); // back off
        else dir = 0f;

        if (Mathf.Abs(dir) > 0.01f) FaceDirection(dir);

        return transform.position.x + dir * chaseSpeed * Time.fixedDeltaTime;
    }

    private void MoveOnce(float desiredX)
    {
        // clamp once here
        desiredX = Mathf.Clamp(desiredX, minX, maxX);
        rb.MovePosition(new Vector2(desiredX, rb.position.y));
    }


    private void FaceDirection(float dir)
    {
        if (sprite == null) return;
        if (dir > 0.01f) sprite.flipX = false;
        else if (dir < -0.01f) sprite.flipX = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Patrol bounds
        Gizmos.color = Color.yellow;
        if (pointA && pointB)
        {
            Gizmos.DrawLine(new Vector3(pointA.position.x, transform.position.y, 0),
                            new Vector3(pointB.position.x, transform.position.y, 0));
            Gizmos.DrawWireSphere(pointA.position, 0.15f);
            Gizmos.DrawWireSphere(pointB.position, 0.15f);
        }

        // Detection
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Ideal distance markers (left/right)
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.5f);
        if (player)
        {
            Vector3 left = new Vector3(player.position.x - idealDistance, transform.position.y, 0);
            Vector3 right = new Vector3(player.position.x + idealDistance, transform.position.y, 0);
            Gizmos.DrawWireSphere(left, 0.1f);
            Gizmos.DrawWireSphere(right, 0.1f);
        }
    }


}

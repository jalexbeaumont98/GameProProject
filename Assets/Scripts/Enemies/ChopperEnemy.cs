using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopperEnemy : Enemy
{

    [Header("Chopper References")]
    [SerializeField] SpriteRenderer[] srs;
    [SerializeField] Transform gun;

    [Header("Patrol Behaviour")]
    [SerializeField] protected PolygonCollider2D flyZone;
    [SerializeField] protected float playerChaseRange = 6f;
    [SerializeField] protected float wanderChangeInterval = 3f; // how often to pick a new wander target
    [SerializeField] protected float wanderSpeed = 2;
    [SerializeField] protected float minTargetDistance = 3f;
    [SerializeField] protected float targetYOffset = 1f;
    [SerializeField] protected float acceleration = 5f;

    


    protected Vector2 targetPos;


    [Header("Debug Options")]
    [SerializeField] private bool showGizmos = true;


    private Vector2 wanderTarget;
    private float nextWanderTime;

    protected override void Awake()
    {
        base.Awake();
        PickNewWanderTarget();
    }

    protected override void Start()
    {
        base.Start();


    }

    void PickNewWanderTarget()
    {
        // Pick a random point inside the polygon
        Bounds bounds = flyZone.bounds;
        Vector2 randomPoint;
        int safety = 0;

        do
        {
            randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            safety++;
        }
        while (!flyZone.OverlapPoint(randomPoint) && safety < 50);

        wanderTarget = randomPoint;
        nextWanderTime = Time.time + wanderChangeInterval;
    }

    protected override void Update()
    {
        base.Update();
    }



    private void FixedUpdate()
    {
        Move();
    }

    void MoveToward()
    {
        targetPos.y += targetYOffset; //Chopper wants to be above player, use an offset

        if (Vector2.Distance(transform.position, targetPos) < minTargetDistance) return; //if too close to target return

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        // Calculate desired velocity
        Vector2 desiredVelocity = direction * moveSpeed;

        // Only apply if inside the fly zone
        if (flyZone.OverlapPoint(targetPos))
        {
            rb.velocity = desiredVelocity;
        }
        else
        {
            // Move back toward the nearest point inside the zone
            Vector2 closest = flyZone.ClosestPoint(transform.position);
            Vector2 retreatDir = (closest - (Vector2)transform.position).normalized;
            rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
        }
    }

    void Wander()
    {
        if (Time.time >= nextWanderTime || Vector2.Distance(transform.position, wanderTarget) < 0.2f)
        {
            PickNewWanderTarget();
        }

        targetPos = wanderTarget;

        MoveToward();
    }

    protected override void Move()
    {

        if (isStunned) return;
        
        if (player == null || flyZone == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Decide whether to follow the player or wander
        if (distanceToPlayer <= playerChaseRange)
        {
            currentSpeed = moveSpeed;
            targetPos = player.position;
            MoveToward();
        }
        else
        {
            currentSpeed = wanderSpeed;
            Wander();
        }

        base.Move();

        AnimateMovement();
    }

    protected override void AnimateMovement()
    {

        bool flip = true;
        if (transform.position.x > targetPos.x) flip = false;

        // Flip sprite based on horizontal velocity
        foreach (SpriteRenderer sr in srs)
        {
           sr.flipX = flip;
        }

        if (flip) gun.transform.localPosition = new Vector3(0.36f, gun.transform.localPosition.y, gun.transform.localPosition.z);

        else gun.transform.localPosition = new Vector3(-0.36f, gun.transform.localPosition.y, gun.transform.localPosition.z);


        base.AnimateMovement();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
    }

    protected override IEnumerator StunCo()
    {
        rb.gravityScale = 0.2f;
        isStunned = true;
        yield return new WaitForSeconds(stunTime);

        isStunned = false;
        rb.gravityScale = 0;
    }

    protected override void Die()
    {
        base.Die();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(wanderTarget, 0.2f);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // translucent red
        Gizmos.DrawWireSphere(transform.position, playerChaseRange);

        
        Gizmos.DrawWireSphere(transform.position, minTargetDistance);
    }
}

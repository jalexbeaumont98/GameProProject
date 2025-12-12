using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss : Enemy
{
    public enum BossState
    {
        Dormant,
        GroundMode,
        FlyingMode
    }

    [Header("Boss Settings")]
    [SerializeField] private BossState currentState = BossState.Dormant;

    [Tooltip("All body sprites that should flip when boss changes direction (do NOT include gun sprites here).")]
    [SerializeField] private List<SpriteRenderer> bodySpritesToFlip = new List<SpriteRenderer>();

    [Tooltip("Polygon bounds that define the boss arena / movement area.")]
    [SerializeField] private PolygonCollider2D movementBounds;

    [Header("Ground Mode")]
    [SerializeField] private float groundMoveSpeed = 3f;

    [Header("Flying Mode")]
    [SerializeField] private float flyingMoveSpeed = 4f;

    [Tooltip("How fast the boss lerps its velocity in flying mode.")]
    [SerializeField] private float flyingAcceleration = 8f;

    [Header("Barrage Settings")]
    [SerializeField] private float barrageInterval = 4f;
    [SerializeField] private List<EnemyBossGun> bossGuns = new List<EnemyBossGun>();

    [SerializeField] private float facingDeadzone = 0.2f; // how close before we stop flipping

    [Header("Heath Bar")]
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject winMenu;


    private float barrageTimer;
    private bool facingRight = true;

    protected override void Awake()
    {
        base.Awake();
        // override moveSpeed with groundMoveSpeed for clarity
        currentSpeed = groundMoveSpeed;
    }

    protected override void Start()
    {
        base.Start();

        if (movementBounds == null)
        {
            movementBounds = GetComponent<PolygonCollider2D>();
            if (movementBounds == null)
            {
                Debug.LogWarning("EnemyBoss: No movementBounds PolygonCollider2D assigned.");
            }
        }

        barrageTimer = barrageInterval;
    }

    protected override void Update()
    {
        base.Update();

        if (isStunned) return;

        switch (currentState)
        {
            case BossState.Dormant:
                // Do nothing until activated externally
                return;

            case BossState.GroundMode:
                HandleGroundMode();
                break;

            case BossState.FlyingMode:
                HandleFlyingMode();
                break;
        }

        HandleBarrageTimer();
    }

    // --- Public API ---

    /// <summary>
    /// Activate the boss and set initial state (defaults to GroundMode).
    /// </summary>
    public void ActivateBoss(BossState initialState = BossState.GroundMode)
    {
        currentState = initialState;
        foreach (var gun in bossGuns)
        {
            if (gun != null)
                gun.SetGunActive();
        }
        barrageTimer = barrageInterval;
        UpdateHealthUI(maxHealth,maxHealth);
    }

    public void SetBossState(BossState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case BossState.GroundMode:
                currentSpeed = groundMoveSpeed;
                break;
            case BossState.FlyingMode:
                currentSpeed = flyingMoveSpeed;
                break;
            case BossState.Dormant:
                rb.velocity = Vector2.zero;
                break;
        }
    }

    // --- Ground mode movement ---

    private void HandleGroundMode()
    {
        if (player == null || rb == null) return;

        // Move horizontally towards player, clamped in X by movementBounds
        float dirX = Mathf.Sign(player.position.x - transform.position.x);
        if (Mathf.Abs(dirX) < facingDeadzone)
        return;
        
        Vector2 velocity = new Vector2(dirX * groundMoveSpeed, rb.velocity.y);

        rb.velocity = velocity;

        // Clamp X within arena bounds if we have them
        if (movementBounds != null)
        {
            Bounds b = movementBounds.bounds;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, b.min.x, b.max.x);
            transform.position = pos;
        }

        UpdateFacing(dirX);
    }

    // --- Flying mode movement ---

    private void HandleFlyingMode()
    {
        if (player == null || rb == null) return;

        Vector2 targetPos = player.position;
        Vector2 currentPos = rb.position;

        // Desired direction to player
        Vector2 toPlayer = (targetPos - currentPos).normalized;
        Vector2 desiredVelocity = toPlayer * flyingMoveSpeed;

        // Smooth velocity towards desired
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, flyingAcceleration * Time.deltaTime);

        // Try new position based on velocity
        Vector2 newPos = rb.position + rb.velocity * Time.deltaTime;

        if (movementBounds != null)
        {
            // Keep boss inside polygon arena
            if (movementBounds.OverlapPoint(newPos))
            {
                rb.MovePosition(newPos);
            }
            else
            {
                Vector2 clamped = movementBounds.ClosestPoint(newPos);
                rb.MovePosition(clamped);
            }
        }
        else
        {
            rb.MovePosition(newPos);
        }

        // Facing based on horizontal velocity
        float dirX = rb.velocity.x != 0 ? Mathf.Sign(rb.velocity.x) : Mathf.Sign(player.position.x - transform.position.x);
        UpdateFacing(dirX);
    }

    // --- Barrage logic ---

    private void HandleBarrageTimer()
    {
        if (bossGuns == null || bossGuns.Count == 0) return;

        barrageTimer -= Time.deltaTime;
        if (barrageTimer <= 0f)
        {
            FireBarrage();
            barrageTimer = barrageInterval;
        }
    }

    private void FireBarrage()
    {
        bool delay = false;
        foreach (var gun in bossGuns)
        {
            if (gun != null)
                gun.FireBurst(true);

            delay = !delay;
        }
    }

    // --- Facing / sprite flip ---

    private void UpdateFacing(float dirX)
    {
        // If player is too close horizontally, do nothing
        if (Mathf.Abs(dirX) < facingDeadzone)
            return;

        // Normal facing logic, but note your original was reversed (dirX < 0 = right)
        bool shouldFaceRight = dirX < 0;

        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            FlipBodySprites();
        }
    }


    private void FlipBodySprites()
    {
        /*
        // Flip all body sprites (but not guns, since they're not in this list)
        foreach (var bodySr in bodySpritesToFlip)
        {
            if (bodySr != null)
                bodySr.flipX = !facingRight;
        }
        */
    }

    // Optional: gizmo to show movement bounds center
    private void OnDrawGizmosSelected()
    {
        if (movementBounds != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(movementBounds.bounds.center, movementBounds.bounds.size);
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        UpdateHealthUI(currentHealth, maxHealth);
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }

        if (healthText != null)
        {
            healthText.text = "BOSS: " + currentHP + "/" + maxHP;
        }
    }

    protected override void Die()
    {
        base.Die();

        winMenu.SetActive(true);
    }
}

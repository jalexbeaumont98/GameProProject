using System;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] PlayerShellController shellCon;

    [Header("Attributes")]
    [SerializeField] private int maxHealth;

    int currentHealth;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float airSpeed = 5f;
    [SerializeField] float accelRate = 15f;   // how fast you accelerate
    [SerializeField] float decelRate = 10f;   // how fast you slow down
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f; // how long after leaving ground you can still jump
    //[SerializeField] private float jumpAnimTime = 0.2f;
    float horizontalMovement;
    private float coyoteTimer = 0f;
    private bool wasGrounded = false;


    [Header("Ground Detection Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.05f, 0.05f);

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2;
    [SerializeField] private float fallSpeedMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = 18f;

    [Header("Turret")]
    [SerializeField] private TankTurretController turretController;

    [Header("Chassis")]
    [SerializeField] private ChassisController chassisController;

    [Header("Tracks")]
    [SerializeField] private TracksController tracksController;

    [Header("Debug Options")]
    [SerializeField] private bool showGroundGizmos = true;

    //Events
    public static event Action<float, float> OnPlayerHPChange;
    public static event System.Action OnDashFire;

    void Awake()
    {
        currentHealth = maxHealth;
        OnPlayerHPChange?.Invoke(currentHealth, maxHealth);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GroundCheck())
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        }

        else
        {
            float targetSpeed = horizontalMovement * airSpeed;
            float rate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelRate : decelRate;

            float newX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, rate * Time.deltaTime);

            rb.velocity = new Vector2(newX, rb.velocity.y);
        }

        Gravity();

        tracksController.AnimateTracks(horizontalMovement);

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;

    }

    public void ChangeRound(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            float value = context.ReadValue<float>();

            print(value);
            if (value > 0) value = 1;
            else value = -1;

            turretController.ChangeRound((int)value);
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            turretController.Shoot(false);
        }
    }

    public void AltFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            turretController.Shoot(true);

        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (GroundCheck())
            {
                rb.velocity += new Vector2(rb.velocity.x, jumpForce);
                //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                AnimateJump();
            }

            else if (turretController.HasDashShells())
            {
                rb.velocity += new Vector2(rb.velocity.x, jumpForce);
                //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                OnDashFire?.Invoke();
                AnimateJump();
            }
        }

        else if (context.canceled)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            if (horizontalMovement == 0) return;

            if (GroundCheck()) return;

            if (!turretController.HasDashShells()) return; //from this point on dash must happen because dash ammo has been subtracted.

            float value = horizontalMovement;

            if (value > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + dashForce, rb.velocity.y);
                //rb.velocity = new Vector2(dashForce, rb.velocity.y);

            }

            else
            {
                rb.velocity = new Vector2(rb.velocity.x - dashForce, rb.velocity.y);
                //rb.velocity = new Vector2(-dashForce, rb.velocity.y);
                //AnimateJump();
            }
            OnDashFire?.Invoke();
            print(value);
            AnimateDash((int)value);

        }

    }

    public void AnimateJump()
    {

        chassisController.PlayJumpAnim(); //AnimateChassis
        turretController.PlayJumpAnim(); //AnimateTurret
                                         //SpawnMuzzleFlash
        turretController.FireDash(); //SpawnExplosion
    }

    public void AnimateDash(int direction)
    {
        turretController.PlayDashAnim(direction); //AnimateTurret
                                                  //SpawnMuzzleFlash
        turretController.FireDash(); //SpawnExplosion
    }

    private void ResetDashes()
    {
        turretController.ReloadDashShells();
    }

    public bool GroundCheck()
    {
        bool groundedNow = Physics2D.OverlapBox(
            groundChecker.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        if (groundedNow)
        {
            // Reset timer whenever you touch the ground
            coyoteTimer = coyoteTime;

            if (!wasGrounded)
            {
                ResetDashes();
            }
        }
        else
        {
            // Count down if not grounded
            coyoteTimer -= Time.deltaTime;
        }

        // Store for reference (not strictly needed, but useful if you want transitions)
        wasGrounded = groundedNow;

        // Still "grounded" if on ground OR timer is active
        return groundedNow || coyoteTimer > 0f;
    }

    private void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }

        else
        {
            rb.gravityScale = baseGravity;
        }

    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        OnPlayerHPChange?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    public void Stun()
    {

    }

    private void Die()
    {
        print("player die :(");
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGroundGizmos) return;
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundChecker.position, groundCheckSize);
    }
}

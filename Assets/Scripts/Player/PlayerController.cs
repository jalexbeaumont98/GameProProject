using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Rigidbody2D rb;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float airSpeed = 5f;
    [SerializeField] float accelRate = 15f;   // how fast you accelerate
    [SerializeField] float decelRate = 10f;   // how fast you slow down
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f; // how long after leaving ground you can still jump
    //[SerializeField] private float jumpAnimTime = 0.2f;

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

    [Header("Shooting")]
    [SerializeField] private GameObject jumpExplosion;
    [SerializeField] private List<GameObject> projectiles;
    int projectileIndex = 0;



    [Header("Debug Options")]
    [SerializeField] private bool showGroundGizmos = true;


    float horizontalMovement;

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

    public void Jump(InputAction.CallbackContext context)
    {

        if (GroundCheck())
        {
            if (context.performed)
            {
                rb.velocity += new Vector2(rb.velocity.x, jumpForce);
                AnimateJump();
            }

            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            //todo add higher jump if held down + even higher jump if pressed right when landing


        }

    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (turretController.Shoot())
            {
                turretController.SpawnProjectile(projectiles[projectileIndex]);
            }
        }
    }

    public void AnimateJump()
    {
        chassisController.PlayJumpAnim(); //AnimateChassis
        turretController.PlayJumpAnim(); //AnimateTurret
                                         //SpawnMuzzleFlash
        turretController.SpawnProjectile(jumpExplosion, true); //SpawnExplosion
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

    private void OnDrawGizmosSelected()
    {
        if (!showGroundGizmos) return;
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundChecker.position, groundCheckSize);
    }
}

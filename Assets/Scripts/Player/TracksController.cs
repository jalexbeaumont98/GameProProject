using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksController : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Transform tankBody;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    [Header("Modifiers")]
    [SerializeField] private float springStrength = 10f;


    private float verticalOffset = 0f;
    private float velocity = 0f;


    void Start()
    {
        playerController = tankBody.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Target offset (0 = aligned with body)
        float targetOffset = 0f;

        // If body is not grounded, sag down slightly
        if (!IsGrounded())
            targetOffset = -0.095f;

        // Spring toward target offset
        verticalOffset = Mathf.SmoothDamp(verticalOffset, targetOffset, ref velocity, 1f / springStrength);

        // Apply offset relative to body
        transform.position = tankBody.position + new Vector3(0, verticalOffset, 0);
        transform.rotation = tankBody.rotation;
    }

    bool IsGrounded()
    {
        // Hook this up to your existing ground check
        return playerController.GroundCheck();

    }



    // Start is called before the first frame update
    public void AnimateTracks(float horizontalMovement)
    {


        if (horizontalMovement > 0.01f)
        {
            animator.Play("TreadAnim");
            animator.speed = 1f;
        }

        else if (horizontalMovement < -0.01f)
        {
            animator.Play("TreadReverseAnim");
            animator.speed = 1f;
        }

        else
            animator.speed = 0f; // pause
    }


}

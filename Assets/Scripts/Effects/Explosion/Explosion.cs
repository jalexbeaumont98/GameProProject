using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Destroy after the current animation’s length
        //float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        //Destroy(gameObject, animLength);
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}

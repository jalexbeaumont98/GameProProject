using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : DamageDealingController
{


    [SerializeField] protected float knockbackForce;

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

        AudioManager.Instance.PlaySFX("explosion_basic");
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Rigidbody2D enemyRb = collision.collider.attachedRigidbody;

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (stun > 0)
                {
                    enemy.Stun();
                }

            }

            if (enemyRb != null && knockbackForce > 0)
            {
                // Direction from projectile to enemy
                Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);
            }

        }

        
    }

    public virtual void DestroyObjectSelf()
    {
        Destroy(gameObject);
    }
}

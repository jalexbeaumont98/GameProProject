using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // If we hit an enemy, apply knockback
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

            if (enemyRb != null)
                {
                    // Direction from projectile to enemy
                    Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                    enemyRb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);
                }

            if (maxPiercing > pierces)
            {
                pierces++;
                return;
            }

            // Destroy projectile after impact
            DestroyProjectile();
        }
    }

    
}

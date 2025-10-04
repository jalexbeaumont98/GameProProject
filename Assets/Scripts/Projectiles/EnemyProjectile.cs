using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.attachedRigidbody;

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(damage);

                if (stun > 0)
                {
                    player.Stun();
                }

            }

            if (playerRb != null)
                {
                    // Direction from projectile to enemy
                    Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);
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

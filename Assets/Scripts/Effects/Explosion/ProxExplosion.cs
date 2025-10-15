using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxExplosion : Explosion
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // If we hit an enemy, apply knockback
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.attachedRigidbody;

            if (playerRb != null)
            {
                // Direction from projectile to enemy
                Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);
            }

        }
    }
}

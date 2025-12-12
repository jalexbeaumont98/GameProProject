using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagProjectile : Projectile
{
    [SerializeField] private bool active = false;

    [SerializeField] Collider2D col;

    [SerializeField] protected LayerMask groundLayers;

    bool falling = false;

    protected override void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.rotation = 0;
    }



    /*


    protected override void OnCollisionEnter2D(Collision2D collision)
    {


        // If we hit an enemy, apply knockback
        if (collision.collider.CompareTag("Enemy") && falling)
        {

            falling = false;
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
                // Determine push direction based on relative position
                float pushDir = Mathf.Sign(enemyRb.position.x - rb.position.x);

                Vector2 force = new Vector2(pushDir, 0f) * knockbackForce;
                enemyRb.AddForce(force, ForceMode2D.Impulse);
            }


            if (maxPiercing > pierces)
            {
                pierces++;
                return;
            }

            // Destroy projectile after impact

        }

        if (((1 << collision.gameObject.layer) & groundLayers) != 0)
        {
            col.isTrigger = true;
            falling = false;
        }


    }

    */

    public void InitStag()
    {
        print("gravity enabled for stag!");
        rb.gravityScale = 1;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagHit : Projectile
{

    [SerializeField] protected LayerMask groundLayers;

    protected override void Start()
    {

    }

    protected override void LateUpdate()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {

            print("stag enemy hit!!");

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();


            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (stun > 0)
                {
                    enemy.Stun();
                }

            }


            DestroyProjectile();

        }

        

        if (((1 << collision.gameObject.layer) & groundLayers) != 0)
        {
            DestroyProjectile();
        }

        
    }

}

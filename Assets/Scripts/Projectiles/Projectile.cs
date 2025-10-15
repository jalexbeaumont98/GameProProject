using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class Projectile : DamageDealingController
{
    

    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float lifeTime = 3f;
    [SerializeField] protected float knockbackForce = 10f;
    [SerializeField] protected int maxBounces = 1;
    [SerializeField] protected int maxPiercing = 0;
    [SerializeField] protected GameObject explosion;
    private Rigidbody2D rb;

    protected int bounces = 0;
    protected int pierces = 0;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        // Launch in the direction the projectile is facing
        rb.velocity = transform.right * speed;
        // NOTE: use transform.up if you want "forward" to be the local Y instead
    }

    protected virtual void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) DestroyProjectile();
    }

    protected virtual void LateUpdate()
    {
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle; // sets Rigidbody2D's rotation
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider.CompareTag("Ground"))
        {
            if (maxBounces > bounces)
            {
                bounces++;
                return;
            }

            DestroyProjectile();
        }
    }

    protected virtual void SpawnExplosion()
    {
        if (explosion == null) return;
        Instantiate(explosion, transform.position, quaternion.identity);
    }

    protected virtual void DestroyProjectile()
    {
        SpawnExplosion();
        Destroy(gameObject);
    }

}

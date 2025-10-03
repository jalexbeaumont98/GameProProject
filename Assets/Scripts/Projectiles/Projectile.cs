using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private int maxBounces = 1;
    [SerializeField] private int maxPiercing = 0;
    [SerializeField] private GameObject explosion;
    private Rigidbody2D rb;

    private int bounces = 0;
    private int pierces = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Launch in the direction the projectile is facing
        rb.velocity = transform.right * speed;
        // NOTE: use transform.up if you want "forward" to be the local Y instead
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) DestroyProjectile();
    }

    void LateUpdate()
    {
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle; // sets Rigidbody2D's rotation
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If we hit an enemy, apply knockback
        if (collision.collider.CompareTag("Enemy"))
        {
            Rigidbody2D enemyRb = collision.collider.attachedRigidbody;
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
        else if (collision.collider.CompareTag("Ground"))
        {
            if (maxBounces > bounces)
            {
                bounces++;
                return;
            }

            DestroyProjectile();
        }
    }

    public void SpawnExplosion()
    {
        if (explosion == null) return;
        Instantiate(explosion, transform.position, quaternion.identity);
    }

    public void DestroyProjectile()
    {
        SpawnExplosion();
        Destroy(gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected Transform player;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected GameObject deathExplosion;


    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    protected float currentSpeed;


    [Header("Base Enemy Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;

    [Header("Hitstun")]
    [SerializeField] protected bool isStunnable = true;
    [SerializeField] protected float stunTime;
    protected bool isStunned = false;

    [Header("Debug Options")]
    [SerializeField] public bool showCollisionMessages = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentSpeed = moveSpeed;
    }

    protected virtual void Start()
    {

        player = GameState.Instance.GetPlayer();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();


    }

    protected virtual void Update()
    {

    }

    protected virtual void Move()
    {

    }

    protected virtual void AnimateMovement()
    {

    }

    public virtual void Stun()
    {
        if (isStunnable) StartCoroutine(StunCo());
    }

    protected virtual IEnumerator StunCo()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunTime);

        isStunned = false;
    }

    // Called when enemy takes damage
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0) Die();
    }

    // Called when enemy dies
    protected virtual void Die()
    {
        // Implementation left for subclasses
        Instantiate(deathExplosion, transform.position, quaternion.identity);
        Destroy(gameObject);
    }

    // Handle collisions
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Implementation left for subclasses
        print(gameObject.name + " has collided with " + collision.gameObject.name);

        

    }

    // Optional trigger version
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        // Implementation left for subclasses
        print(gameObject.name + " has collided with the trigger " + collider.gameObject.name);
    }
}

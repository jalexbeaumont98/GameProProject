using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;     // Assign your player transform
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;


    [Header("Attributes")]
    [SerializeField] private float rotationSpeed = 5f; // How quickly it turns
    [SerializeField] private float range = 8f;
    [SerializeField] private float fireRate = 1f; // time between shots


    [Header("Debug Settings")]
    [SerializeField] bool debugMessages = true;

    private float fireTimer = 0f;

    void Start()
    {
        player = GameState.Instance.GetPlayer();
    }

    void Update()
    {
        if (player == null)
            return;


        float angle = 0;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Decide whether to follow the player or wander
        if (distanceToPlayer > range)
        {
            angle = 180f;
        }

        else
        {
            // Direction from gun to player
            Vector2 direction = player.position - transform.position;

            // Calculate the angle in degrees
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            fireTimer -= Time.deltaTime;

            // interval is 1 / fireRate
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = 1f / fireRate;
            }
        }



        // Create the target rotation
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Smoothly rotate toward the target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        DebugMessage("Shoot!");
        Instantiate(projectile, firePoint.position, firePoint.rotation);
    }



    public void DebugMessage(string message)
    {
        if (!debugMessages) return;
        print(message);
    }





}

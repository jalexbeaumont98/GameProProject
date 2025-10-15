using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private float spawnInterval = 0.2f; // seconds between spawns
    [SerializeField] private float minSpawnInterval = 0.1f;
    [SerializeField] private Transform spawnPoint;     // optional spawn position

    private float spawnTimer;
    private float currentSpawnInterval;

    void Start()
    {
        currentSpawnInterval = spawnInterval;
    }

    public void SetSpawnInterval(InputAction.CallbackContext context)
    {
        
        currentSpawnInterval = Mathf.Clamp(spawnInterval - Mathf.Abs(context.ReadValue<Vector2>().x), minSpawnInterval, spawnInterval);
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnObject();
            spawnTimer = 0f; // reset timer
        }
    }

    private void SpawnObject()
    {
        Vector3 position = spawnPoint ? spawnPoint.position : transform.position;
        Instantiate(particle, position, Quaternion.identity);
        
    }
}

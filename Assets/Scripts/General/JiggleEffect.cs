using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiggleEffect : MonoBehaviour
{
    [SerializeField] private float intensity = 0.05f; // how far it jiggles
    [SerializeField] private float speed = 25f;       // how fast it jiggles

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Random offset using Perlin noise for smooth jitter
        float offsetX = (Mathf.PerlinNoise(Time.time * speed, 0f) - 0.5f) * 2f * intensity;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * speed) - 0.5f) * 2f * intensity;

        transform.localPosition = initialPosition + new Vector3(offsetX, offsetY, 0f);
    }
}

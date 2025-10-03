using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 5f;

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // World position of mouse
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = player.position.z; // lock to player's Z (important in 2D)

        // Direction from player to mouse
        Vector3 dir = mouseWorld - player.position;

        // Clamp distance
        if (dir.magnitude > maxDistance)
            dir = dir.normalized * maxDistance;

        // Position = player + clamped vector
        transform.position = player.position + dir;
    }
}

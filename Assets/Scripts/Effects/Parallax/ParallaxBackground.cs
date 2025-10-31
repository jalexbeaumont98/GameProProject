using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] float parallaxSpeedX = 5;
    [SerializeField] private float parallaxSpeedY = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetStartPosition;

    public float spriteWidth;

    [SerializeField] Transform target;

    float startPositionX;

    void Start()
    {
        //target = Camera.main.transform;
        target = GameObject.FindWithTag("Player").transform;
        startPosition = transform.position;
        targetStartPosition = target.position;

        // Get sprite width in world units
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteWidth = sr.bounds.size.x;
        else
            Debug.LogWarning("No SpriteRenderer found! Infinite parallax will not work.");
    }

    private void LateUpdate()
    {
        Vector3 delta = target.position - targetStartPosition;

        // Move layer based on parallax speed
        float newX = startPosition.x + delta.x * parallaxSpeedX;
        float newY = startPosition.y + delta.y * parallaxSpeedY;

        transform.position = new Vector3(newX, newY, startPosition.z);

        // If the camera has moved far enough, reposition behind sibling for seamless looping
        if (Mathf.Abs(target.position.x - transform.position.x) >= spriteWidth)
        {
            float offset = (target.position.x > transform.position.x) ? spriteWidth * 2f : -spriteWidth * 2f;
            startPosition.x += offset;
            transform.position = new Vector3(startPosition.x, transform.position.y, transform.position.z);
        }
    }

    /*

    void Update()
    {
        float relativeDistanceX = target.position.x * -parallaxSpeedX;

        transform.position = new Vector3(startPositionX + relativeDistanceX, transform.position.y, 0);

        float relativeCameraPositionX =
                target.position.x - relativeDistanceX;

        if (relativeCameraPositionX > startPositionX + spriteWidth)
        {
            startPositionX += spriteWidth * 2;
        }
        else if (relativeCameraPositionX < startPositionX - spriteWidth)
        {
            startPositionX -= spriteWidth * 2;
        }

    }
    */
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] float speedX = 5;

    Transform cameraTransform;

    float startPositionX;
    public float spriteWidth;

    void Start()
    {
        cameraTransform = Camera.main.transform;

        startPositionX = transform.position.x;

        spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        speedX = speedX / 20;
    }

    void Update()
    {
        float relativeDistanceX = cameraTransform.position.x * -speedX;

        transform.position = new Vector3(startPositionX + relativeDistanceX, transform.position.y, 0);

        float relativeCameraPositionX =
                cameraTransform.position.x - relativeDistanceX;

        if (relativeCameraPositionX > startPositionX + spriteWidth)
        {
            startPositionX += spriteWidth * 2;
        }
        else if (relativeCameraPositionX < startPositionX - spriteWidth)
        {
            startPositionX -= spriteWidth * 2;
        }

    }
}

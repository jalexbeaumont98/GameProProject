using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParticle : DestroySelf
{

    [SerializeField] private float verticalSpeed = 1f;
    [SerializeField] private float horizontalDriftStrength = 0.5f;

    private float currentDrift;


    void Start()
    {
        // start with a random drift direction
        currentDrift = Random.Range(-horizontalDriftStrength, horizontalDriftStrength);
    }

    void Update()
    {

        // apply both vertical and horizontal movement
        Vector3 movement = new Vector3(currentDrift, verticalSpeed, 0f) * Time.deltaTime;
        transform.position += movement;
    }
    
    public override void DestroyObjectSelf()
    {
        Destroy(gameObject);
    }
}

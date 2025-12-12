using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] int powerupType;
    [SerializeField] bool shell;

    void Start()
    {   if (!shell)
            if (GameState.Instance.powerups[powerupType].unlocked) Destroy(gameObject);
        else   
            if (GameState.Instance.shells[powerupType].unlocked) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            GameState.Instance.OnPowerup(powerupType, shell);
            Destroy(gameObject);
        }

        
    }
}

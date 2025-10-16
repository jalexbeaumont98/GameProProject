using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class GameState : MonoBehaviour
{

    public static GameState Instance { get; private set; }

    public Transform player;

    [SerializeField] public List<ShellData> shells;

    public bool dashUnlocked = true;

    public IReadOnlyList<ShellData> Shells => shells;

    public int maxDashes = 2;

   


    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // optional, keeps it across scenes
    }

    void Start()
    {
        
    }


    public Transform GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;

        }

        if (player == null)
        {
            Debug.LogError("Player object not found! Make sure it’s tagged 'Player'.");
        }

        return player;

    }

    public void OnPowerup(int powerupType)
    {
        print("Powerup achieved!");

        if (powerupType == 0)
        {
            player.GetComponentInChildren<TankTurretController>().SetDashUnlocked();
        }

        if (powerupType == 1)
        {
            shells[0].unlocked = true;
            TankTurretController tc = player.GetComponentInChildren<TankTurretController>();

            tc.SetProjectilesUnlocked(0);
            
        }
    }

    

    
}

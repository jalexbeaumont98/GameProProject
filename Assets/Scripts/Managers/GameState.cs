using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{

    public static GameState Instance { get; private set; }

    public Transform player;

    [SerializeField] public List<ShellData> shells;
    [SerializeField] public List<PUData> powerups;

    public bool dashUnlocked = false;

    public IReadOnlyList<ShellData> Shells => shells;

    public IReadOnlyList<PUData> Powerups => powerups;

    public int maxDashes = 2;

    public string currentLevel;






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


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //SetShellsUnlockedOnSceneChange();
        if (scene.name != "GameOver" && scene.name != "MainMenu")
            currentLevel = scene.name;
    }

    public void PlayerDeath()
    {
        SceneManager.LoadScene("GameOver");
    }


    public Transform GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        }

        if (player == null)
        {
            Debug.Log("Player object not found! Make sure it’s tagged 'Player'.");
        }

        return player;

    }

    public void OnPowerup(int powerupType, bool shell)
    {
        print("Powerup achieved!");
        print(shell);
        print(powerupType);

        if (!shell)
        {
            if (powerupType == 0)
            {
                powerups[0].unlocked = true;
                PlayerController pc = player.GetComponent<PlayerController>();
                pc.SetPowerUpsUnlocked(0);;
            }

            return;
        }

        else
        {
            if (powerupType == 0)
            {
                shells[0].unlocked = true;
                TankTurretController tc = player.GetComponentInChildren<TankTurretController>();

                tc.SetProjectilesUnlocked(0);

            }
        }


    }

    public void SetShellsUnlockedOnSceneChange()
    {
        GetPlayer();
        if (player == null)
        {
            print("player is null");
            return;
        }

        TankTurretController tc = player.GetComponentInChildren<TankTurretController>();

        PlayerController pc = player.GetComponent<PlayerController>();

        for (int j = 0; j < powerups.Count; j++)
        {
            if (powerups[j].unlocked)
            {
                pc.SetPowerUpsUnlocked(j);
            }
        }

        for (int i = 0; i < shells.Count; i++)
        {
            if (shells[i].unlocked)
            {

                tc.SetProjectilesUnlocked(i);
            }
        }

        print("should be set (shells from gamestate)");
    }






}

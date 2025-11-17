using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public static PauseManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference pauseAction; // Global/Pause action
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private TextMeshProUGUI escapeText;

    [Header("Action Map Names")]
    [SerializeField] private string gameplayMap = "Player";
    [SerializeField] private string uiMap = "UI";

    private bool isPaused = false;

    

    

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (playerInput == null) playerInput = FindAnyObjectByType<PlayerInput>();
    }

    private void OnEnable()
    {
        // Make sure the global Pause action is enabled independently
        pauseAction.action.performed += OnPausePerformed;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePerformed;
        pauseAction.action.Disable();
    }

    public void SetMenu(GameObject menu)
    {
        currentMenu = menu;
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;

            escapeText.text = "Exit Menu";

            if (!currentMenu)
            {
                pauseMenu.SetActive(true);
                playerInput.SwitchCurrentActionMap(uiMap); // enable UI navigation
                                                           //Cursor.visible = true;
                                                           //Cursor.lockState = CursorLockMode.None;
            }

            else
            {
                currentMenu.SetActive(true);
            }

        }
        else
        {
            Time.timeScale = 1f;

            escapeText.text = "Pause";

            if (!currentMenu)
            {
                pauseMenu.SetActive(false);
                playerInput.SwitchCurrentActionMap(gameplayMap);
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked; // optional for FPS-style
            }

            else
            {
                currentMenu.SetActive(false);
                currentMenu = null;
            }

        }
    }



    // Hook these from UI buttons
    public void Resume() => TogglePause();
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference pauseAction; // Global/Pause action
    [SerializeField] private GameObject pauseMenu;

    [Header("Action Map Names")]
    [SerializeField] private string gameplayMap = "Player";
    [SerializeField] private string uiMap = "UI";

    private bool isPaused = false;

    private void Awake()
    {
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

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            playerInput.SwitchCurrentActionMap(uiMap); // enable UI navigation
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            playerInput.SwitchCurrentActionMap(gameplayMap);
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked; // optional for FPS-style
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

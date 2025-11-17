using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SignController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform uiObject;       // UI element to show/hide
    [SerializeField] private Transform worldPoint;      // World position to anchor UI to
    [SerializeField] private Camera cam;    
    [SerializeField] private GameObject signMenu;   
    [SerializeField] private PlayerInput playerInput; 
    [SerializeField] private InputActionReference interactAction; // Global/Pause action

    [SerializeField] private string signMessage;

    bool active = false;            

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;
        
        uiObject.gameObject.SetActive(false);

        if (playerInput == null) playerInput = FindAnyObjectByType<PlayerInput>();
    }

    private void OnEnable()
    {
        // Make sure the global Pause action is enabled independently
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (active)
        {
            uiObject.gameObject.SetActive(false);
            PauseManager.Instance.SetMenu(signMenu);
            signMenu.GetComponent<SignUIManager>().SetSignMessage(signMessage);
            PauseManager.Instance.TogglePause();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            active = true;
            uiObject.gameObject.SetActive(true);
            UpdateUIPosition();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            active = false;
            uiObject.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Keep UI pinned to world point while active
        if (uiObject.gameObject.activeSelf)
        {
            UpdateUIPosition();
        }
    }

    private void UpdateUIPosition()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPoint.position);
        uiObject.transform.position = screenPos;
    }
}

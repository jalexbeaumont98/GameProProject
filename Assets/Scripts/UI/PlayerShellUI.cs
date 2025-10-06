using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShellUI : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private UnityEngine.UI.Image currentShellImage;
    [SerializeField] private TMP_Text ammoCountTextbox;
    [SerializeField] private TMP_Text currentShellNameTextbox;


    [SerializeField] private ShellData emptyShell;

    private IReadOnlyList<ShellData> shells;

    void Start()
    {
        shells = GameState.Instance.Shells;
    }

    void OnEnable()
    {
        PlayerShellController.OnShellChange += UpdateShellUI;
    }

    void OnDisable()
    {
        PlayerShellController.OnShellChange -= UpdateShellUI;
    }

    private void UpdateShellUI(int index)
    {
        print("You're attempting to change the equipped shell!");

        ShellData currentShell;
        if (index > 0) currentShell = shells[index];
        else currentShell = emptyShell;

        currentShellImage.sprite = currentShell.shellImage;
        currentShellNameTextbox.text = currentShell.name;
        

    }
}

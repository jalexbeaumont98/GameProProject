using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShellUI : MonoBehaviour
{
    void OnEnable()
    {
        PlayerController.OnRoundChange += UpdateShellUI;
    }

    void OnDisable()
    {
        PlayerController.OnRoundChange -= UpdateShellUI;
    }

    private void UpdateShellUI(int index)
    {
        
    }
}

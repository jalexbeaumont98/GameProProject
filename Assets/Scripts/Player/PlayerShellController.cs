using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShellController : MonoBehaviour
{
    [SerializeField] private IReadOnlyList<ShellData> shells;
    [SerializeField] private ShellData emptyShell;
    [SerializeField] private bool projectilesUnlocked = false;

    private int shellIndex = 0;

    public static event Action<int> OnShellChange;

    void Start()
    {
        shells = GameState.Instance.Shells;
    }

    public ShellData ChangeRound(int direction)
    {

        if (!projectilesUnlocked)
        {
            ShellChangeMessage(true);
            return emptyShell; //return the empty shell
        }

        int attempts = 0;
        int totalShells = shells.Count;

        do
        {
            // Move index and wrap using modulo
            shellIndex = (shellIndex + direction + totalShells) % totalShells;

            attempts++;
            if (attempts > totalShells)
            {
                Debug.LogWarning("No unlocked shells found — returning null shell.");
                ShellChangeMessage(true);
                return emptyShell;
            }

        } while (!shells[shellIndex].unlocked);

        ShellChangeMessage();
        return shells[shellIndex];
    }

    private void ShellChangeMessage(bool empty = false)
    {
        if (empty) OnShellChange?.Invoke(-1);
        else 
            OnShellChange?.Invoke(shellIndex);
    }

    

    
}

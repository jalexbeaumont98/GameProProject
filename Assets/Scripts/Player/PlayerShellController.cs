using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerShellController : MonoBehaviour
{


    [SerializeField] private IReadOnlyList<ShellData> shells;
    [SerializeField] private int[] shellAmmos;
    [SerializeField] private ShellData emptyShell;
    [SerializeField] private ShellData dashShell;
    [SerializeField] private bool projectilesUnlocked = false;

    private int dashShellIndex = 0;
    private int shellIndex = 0;

    private bool reloading;

    private int currentMaxShells;
    private int maxDashShells;
    private int currentShells;
    private int currentDashShells;

    private Coroutine activeReloadCo;
    private Coroutine activeDashReloadCo;

    public static event Action<int, int> OnShellChange;
    public static event Action OnReloadAlt;
    public static event Action OnReloadDash;

    void Start()
    {
        shells = GameState.Instance.Shells;
        shellAmmos = new int[shells.Count];
        for (int i = 0; i < shells.Count; i++)
        {
            shellAmmos[i] = shells[i].maxRounds;
        }
        InitializeDashShell();
        InitializeShell();
        
    }

    void InitializeDashShell()
    {
        //dashShell = shells[GameState.DASH_SHELL_INDEX];
        //dashShellIndex = GameState.DASH_SHELL_INDEX;
        print(dashShell.name);

        currentDashShells = 0;

        if (dashShell.unlocked)
        {
            print("dash shell init");
            //maxDashShells = shells[GameState.DASH_SHELL_INDEX].maxRounds;
            ReloadDashShells();
        }

        if (projectilesUnlocked)
        {
            shellIndex = -1;
            ChangeRound(1);
        }
    }


    void InitializeShell()
    {
        if (!projectilesUnlocked) return;
        currentMaxShells = shells[shellIndex].maxRounds;
        currentShells = 0;
        ReloadShells();
        ShellChangeMessage();

    }

    public ShellData ChangeRound(int direction)
    {

        if (!projectilesUnlocked)
        {
            //ShellChangeMessage(true);
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
        InitializeShell();
        return shells[shellIndex];
    }

    private void ShellChangeMessage(bool empty = false)
    {
        if (empty) OnShellChange?.Invoke(-1, 0);
        else
            OnShellChange?.Invoke(shellIndex, shellAmmos[shellIndex]);
    }

    public void SubtractAltShell()
    {
        if (shellIndex == dashShellIndex)
            HasDashShells();
        else
        {
            currentShells--;
            shellAmmos[shellIndex]--;
        }
    }

    public bool HasAltShells()
    {
        return currentShells > 0;
    }

    public bool HasDashShells()
    {
        if (currentDashShells > 0)
        {
            currentDashShells--;
            return true;
        }
        return false;

    }

    public void ReloadShells()
    {
        
        if (activeReloadCo != null)
        {
            print("stopping reloading to reload diff shell");
            StopCoroutine(activeReloadCo);
        }

        activeReloadCo = StartCoroutine(ReloadShellCoroutine(shells[shellIndex]));
    }

    public void ReloadDashShells()
    {
        if (currentDashShells >= maxDashShells) return;

        if (activeDashReloadCo != null) return;

        activeDashReloadCo = StartCoroutine(ReloadDashShellCoroutine());
        
    }

    private IEnumerator ReloadDashShellCoroutine()
    {
        yield return new WaitForSeconds(dashShell.reloadTime);

        if (currentDashShells < maxDashShells)
            currentDashShells++;

        OnReloadDash?.Invoke();

        activeDashReloadCo = null;

        if (currentDashShells < maxDashShells)
        {
            ReloadDashShells();
        }

    }
    

    private IEnumerator ReloadShellCoroutine(ShellData shell)
    {
        yield return new WaitForSeconds(shell.maxRounds * shell.reloadTime);
        
        currentShells = shell.maxRounds;
        OnReloadAlt?.Invoke();
        activeReloadCo = null;
    }
    
}

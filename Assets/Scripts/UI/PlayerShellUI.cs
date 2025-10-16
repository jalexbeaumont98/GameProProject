using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShellUI : MonoBehaviour
{

    [Header("References Shell Box")]
    [SerializeField] private UnityEngine.UI.Image currentShellImage;
    [SerializeField] private TMP_Text ammoCountTextbox;
    [SerializeField] private TMP_Text currentShellNameTextbox;
    [SerializeField] private ShellData emptyShell;
    [SerializeField] private Transform shelAmmoHorGroup;

    private List<Shell_Indicator_Controller> altShells;
    private int shellIndex;
    private int altAmmoIndex;

    [Header("References Dash Box")]
    [SerializeField] private Transform dashAmmoHorGroup;
    [SerializeField] private UnityEngine.UI.Image dashImage;
    [SerializeField] private GameObject ammoUIShell;

    List<Shell_Indicator_Controller> dashShells;
    private int dashShellIndex;
    
    private IReadOnlyList<ShellData> shells;

    void Start()
    {
        shells = GameState.Instance.Shells;
        InitializeDashAmmo(GameState.Instance.maxDashes);
        
    }

    void OnEnable()
    {
        TankTurretController.OnShellChange += UpdateShellUI;
        TankTurretController.OnReloadDash += UpdateDashAmmoAdd;
        PlayerController.OnDashFire += UpdateDashAmmoSubtract;

        TankTurretController.OnAltFire += UpdateAltAmmoSubtract;
        TankTurretController.OnReloadAlt += UpdateAltAmmoAdd;
    }

    void OnDisable()
    {
        TankTurretController.OnShellChange -= UpdateShellUI;
        TankTurretController.OnReloadDash -= UpdateDashAmmoAdd;
        PlayerController.OnDashFire -= UpdateDashAmmoSubtract;

        TankTurretController.OnAltFire -= UpdateAltAmmoSubtract;
        TankTurretController.OnReloadAlt -= UpdateAltAmmoAdd;
    }

    private void UpdateShellUI(int index, int currentAmmo)
    {
        //print("You're attempting to change the equipped shell!");
        
        ShellData currentShell;
        if (index >= 0) currentShell = shells[index];
        else currentShell = emptyShell;

        currentShellImage.sprite = currentShell.shellImage;
        currentShellNameTextbox.text = currentShell.name;
        shellIndex = index;
        
        InitializeAltShellAmmo(currentAmmo);

    }

    private void InitializeAltShellAmmo(int currentAmmo)
    {

        //print("current ammo: " + currentAmmo);
        if (altShells != null && altShells.Count > 0)
        {
            foreach (Shell_Indicator_Controller ammo in altShells)
            {
                Destroy(ammo.gameObject);
            }
            altShells.Clear();
        }

        altShells = new List<Shell_Indicator_Controller>();

        altAmmoIndex = 0;
        int max = shells[shellIndex].maxRounds;
        for (int i = 0; i < max; i++)
        {
            altShells.Add(Instantiate(ammoUIShell, shelAmmoHorGroup).GetComponent<Shell_Indicator_Controller>());
            if (i > currentAmmo - 1) altShells[i].SetFired();

        }
        altAmmoIndex = currentAmmo;
    }
    
    private void UpdateAltAmmoSubtract()
    {
        UpdateAltAmmo(-1);
    }

    private void UpdateAltAmmoAdd()
    {
        UpdateAltAmmo(1);
    }
    
    private void UpdateAltAmmo(int amount)
    {
        if (amount > 0)
        {
            altShells[Math.Clamp(altAmmoIndex, 0, altShells.Count-1)].SetReloaded();
            altAmmoIndex++;
        }

        else
        {   
            
            altShells[Math.Clamp(altAmmoIndex-1, 0, altShells.Count-1)].SetFired();
            altAmmoIndex--;
            if (altAmmoIndex < 0) altAmmoIndex = 0;
        }
    }

    private void InitializeDashAmmo(int max)
    {
        if (dashShells != null && dashShells.Count > 0)
        {
            foreach (Shell_Indicator_Controller ammo in dashShells)
            {
                Destroy(ammo.gameObject);
            }
            dashShells.Clear();
        }

        dashShells = new List<Shell_Indicator_Controller>();

        dashShellIndex = 0;
        for (int i = 0; i < max; i++)
        {
            dashShells.Add(Instantiate(ammoUIShell, dashAmmoHorGroup).GetComponent<Shell_Indicator_Controller>());
            dashShells[i].SetFired();
            
        }

    }


    private void UpdateDashAmmoSubtract()
    {
        UpdateDashAmmo(-1);
    }

    private void UpdateDashAmmoAdd()
    {
        UpdateDashAmmo(1);
    }

    private void UpdateDashAmmo(int amount)
    {
        if (amount > 0)
        {
            dashShells[dashShellIndex].SetReloaded();
            dashShellIndex++;
        }

        else
        {   
            
            dashShells[Math.Clamp(dashShellIndex-1, 0, dashShells.Count-1)].SetFired();
            dashShellIndex--;
            if (dashShellIndex < 0) dashShellIndex = 0;
        }
    }
    
    

}

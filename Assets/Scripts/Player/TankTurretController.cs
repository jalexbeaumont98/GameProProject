using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TankTurretController : MonoBehaviour
{
    [Header("Body Setup")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private Sprite[] bodySprites; // 8 sprites in order: E, NE, N, NW, W, SW, S, SE

    [Header("Barrel Setup")]
    [SerializeField] private Transform barrelTransform; // child transform with barrel sprite
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Sprite[] barrelSprites;
    [SerializeField] private Sprite jumpSprite;
    [SerializeField] private Sprite leftDashSprite;
    [SerializeField] private Sprite rightDashSprite;
    [SerializeField] private SpriteRenderer barrelSpriteRenderer;

    [Header("Shooting Attributes")]
    [SerializeField] private float fireRate = 3f; // shots per second (higher = faster)
    [SerializeField] private GameObject regularProjectile;
    [SerializeField] private ShellData altShell;

    [Header("Explosions")]
    [SerializeField] private GameObject jumpExplosion;

    [Header("Shell References")]
    [SerializeField] private int[] shellAmmos;
    [SerializeField] private ShellData emptyShell;
    [SerializeField] private bool projectilesUnlocked = false;
    [SerializeField] private bool dashUnlocked = false;

    IReadOnlyList<ShellData> shells;
    private int shellIndex = 0;

    private int currentMaxShells;
    private int maxDashShells;
    private int currentDashShells;

    private Coroutine activeReloadCo;
    private Coroutine activeDashReloadCo;





    private bool lockTurret = false;
    private float nextFireTime = 0f;

    private Camera mainCam;


    public static event System.Action OnFire;
    public static event System.Action OnAltFire;  //bool is whether fire was alt or not

    public static event System.Action<int, int> OnShellChange;
    public static event System.Action OnReloadAlt;
    public static event System.Action OnReloadDash;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        barrelSpriteRenderer = barrelTransform.GetComponent<SpriteRenderer>();

        shells = GameState.Instance.Shells;
        shellAmmos = new int[shells.Count];
        for (int i = 0; i < shells.Count; i++)
        {
            shellAmmos[i] = shells[i].maxRounds;
        }
        InitializeDashShell();
        InitializeShell();

    }

    void Update()
    {
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - transform.position;

        if (!lockTurret)
        {
            SetRotation(dir);
        }


    }

    private void SetRotation(Vector2 dir)
    {
        // --- Rotate turret ---
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        barrelTransform.rotation = Quaternion.Euler(0, 0, angle);

        // --- Choose body sprite ---
        if (angle < 0) angle += 360f;
        int index = Mathf.RoundToInt(angle / 45f) % 8; // 8 directions
        bodyRenderer.sprite = bodySprites[index];


        // Normalize angle to 0–360
        if (angle < 0) angle += 360f;

        // --- Change turret sprite based on angle
        if (angle >= 247.5f && angle < 292.5f)
            barrelSpriteRenderer.sprite = barrelSprites[1];
        else if ((angle >= 202.5f && angle < 247.5f) || (angle >= 292.5f && angle < 337.5f))
            barrelSpriteRenderer.sprite = barrelSprites[1];
        //southWest, SouthEast and South
        else barrelSpriteRenderer.sprite = barrelSprites[0];
    }


    public bool Shoot(bool alt)
    {
        if (!CanShoot()) return false;

        if (!alt)
        {
            OnFire?.Invoke();
            SpawnProjectile(regularProjectile, false);
        }
        else
        {
            if (shells[shellIndex] != null)
            {
                if (!HasAltShells()) return false;
                SubtractAltShell();
                SpawnProjectile(shells[shellIndex].shell);
                OnFire?.Invoke();
                OnAltFire?.Invoke();

                ReloadShells();
            }

        }
        return true;
    }

    public void FireDash()
    {
        SpawnProjectile(jumpExplosion, true);
    }

    private void SpawnProjectile(GameObject projectile, bool overwriteAngle = false)
    {

        quaternion angle = shootPoint.rotation;
        if (overwriteAngle) angle = quaternion.identity;

        Instantiate(projectile, shootPoint.position, angle);
    }

    public bool CanShoot()
    {

        if (lockTurret) return false;

        if (Time.time < nextFireTime)
            return false;

        // Set next fire time
        nextFireTime = Time.time + 1f / fireRate;
        return true;
    }



    public void PlayJumpAnim()
    {
        lockTurret = true;
        AnimateJump();
        StartCoroutine(LockJump());
    }

    public void PlayDashAnim(int direction)
    {
        lockTurret = true;
        AnimateDash(direction);
        StartCoroutine(LockJump());
    }

    public IEnumerator LockJump()
    {

        yield return new WaitForSeconds(0.33f);

        lockTurret = false;

    }

    private void AnimateJump()
    {
        bodyRenderer.sprite = jumpSprite;
        barrelSpriteRenderer.sprite = barrelSprites[1];
        barrelTransform.rotation = Quaternion.Euler(0, 0, -90);
    }

    private void AnimateDash(int direction)
    {
        if (direction < 0)
        {
            bodyRenderer.sprite = rightDashSprite;
            barrelSpriteRenderer.sprite = barrelSprites[0];
            barrelTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        else
        {
            bodyRenderer.sprite = leftDashSprite;
            barrelSpriteRenderer.sprite = barrelSprites[0];
            barrelTransform.rotation = Quaternion.Euler(0, 0, -180);
        }
    }

    public void SetDashUnlocked()
    {
        dashUnlocked = true;
        InitializeDashShell();
    }

    public void SetProjectilesUnlocked(int unlockedShell = 0)
    {
        projectilesUnlocked = true;
        shellIndex = unlockedShell;
        InitializeShell();
    }

    void InitializeDashShell()
    {

        if (!dashUnlocked) return;

        currentDashShells = 0;

        if (GameState.Instance.dashUnlocked)
        {
            print("dash shell init");
            maxDashShells = GameState.Instance.maxDashes;
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

        ShellChangeMessage();

    }

    public void ChangeRound(int direction)
    {

        if (!projectilesUnlocked)
        {
            ShellChangeMessage(true);
            return; //return the empty shell
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
                return;
            }

        } while (!shells[shellIndex].unlocked);

        ShellChangeMessage();
        InitializeShell();

    }

    private void ShellChangeMessage(bool empty = false)
    {
        if (empty) OnShellChange?.Invoke(-1, 0);
        else
            OnShellChange?.Invoke(shellIndex, shellAmmos[shellIndex]);
    }

    public void SubtractAltShell()
    {
        shellAmmos[shellIndex]--;
    }

    public bool HasAltShells()
    {
        if (!projectilesUnlocked) return false;
        return shellAmmos[shellIndex] > 0;
    }

    public bool HasDashShells()
    {

        if (!dashUnlocked) return false;

        if (currentDashShells > 0)
        {
            currentDashShells--;
            return true;
        }
        return false;

    }

    public void ReloadShells()
    {

        if (shellAmmos[shellIndex] >= currentMaxShells) return;

        if (activeReloadCo != null) return;

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
        yield return new WaitForSeconds(0.1f);

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
        yield return new WaitForSeconds(shell.reloadTime);

        if (shellAmmos[shellIndex] < currentMaxShells)
            shellAmmos[shellIndex]++;


        OnReloadAlt?.Invoke();
        activeReloadCo = null;

        if (shellAmmos[shellIndex] < currentMaxShells)
        {
            ReloadShells();
        }
    }

}

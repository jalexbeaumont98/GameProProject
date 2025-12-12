using System.Collections;
using UnityEngine;

public class EnemyBossGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Burst Settings")]
    [SerializeField] private int shotsPerBurst = 5;
    [SerializeField] private float timeBetweenShots = 0.15f;

    [SerializeField] private float angAdjust = 90f;

    private Transform player;
    private Coroutine burstRoutine;

    private bool dormant = true;

    private void Start()
    {
        player = GameState.Instance.GetPlayer();
        if (firePoint == null)
            firePoint = transform;
    }

    public void SetGunActive(bool active = true)
    {
        dormant = false;
    }

    private void Update()
    {
        if (!dormant) RotateTowardPlayer();
    }

    private void RotateTowardPlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;

        // Compute angle in degrees (Z axis rotation for 2D)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angAdjust;

        // Adjust this offset if your gun sprite points "up" instead of "right"
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Called by the boss to fire a burst of shots.
    /// </summary>
    public void FireBurst(bool delay = false)
    {
        if (burstRoutine == null)
        {
            burstRoutine = StartCoroutine(FireBurstCo(delay));
        }
    }

    private IEnumerator FireBurstCo(bool delay = false)
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            if (delay)
                yield return new WaitForSeconds(timeBetweenShots/2);
            FireSingleShot();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        burstRoutine = null;
    }

    private void FireSingleShot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}
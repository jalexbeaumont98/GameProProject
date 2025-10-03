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
    [SerializeField] private SpriteRenderer barrelSpriteRenderer;

    private bool lockTurret = false;

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        barrelSpriteRenderer = barrelTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - transform.position;

        if (!lockTurret)
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


    }

    public bool Shoot()
    {
        return !lockTurret;
    }

    public void SpawnProjectile(GameObject projectile, bool overwriteAngle = false)
    {
        quaternion angle = shootPoint.rotation;
        if (overwriteAngle) angle = quaternion.identity;

        Instantiate(projectile, shootPoint.position, angle);
    }

    public void PlayJumpAnim()
    {
        StartCoroutine(PlayJump());
    }

    public IEnumerator PlayJump()
    {

        bodyRenderer.sprite = jumpSprite;
        barrelSpriteRenderer.sprite = barrelSprites[1];
        barrelTransform.rotation = Quaternion.Euler(0, 0, -90);
        lockTurret = true;

        yield return new WaitForSeconds(0.33f);

        lockTurret = false;

    }

}

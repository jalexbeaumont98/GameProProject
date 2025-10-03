using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChassisController : MonoBehaviour
{

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float jumpAnimDuration;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayJumpAnim()
    {
        StartCoroutine(PlayJumpThenIdle());
    }

    private IEnumerator PlayJumpThenIdle()
    {
        spriteRenderer.sprite = sprites[1];

        yield return new WaitForSeconds(jumpAnimDuration);

        spriteRenderer.sprite = sprites[0];
    }
}

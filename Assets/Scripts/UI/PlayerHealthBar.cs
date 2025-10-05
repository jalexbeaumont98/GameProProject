using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    void OnEnable()
    {
        PlayerController.OnPlayerHPChange += UpdateHealthUI;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerHPChange -= UpdateHealthUI;
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }

        if (healthText != null)
        {
            healthText.text = "HP: " + currentHP + "/" + maxHP;
        }
    }
}

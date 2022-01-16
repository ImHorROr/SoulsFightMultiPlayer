using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameObject healthbarParent;
    [SerializeField] Image healthbarImgae;

    private void Awake()
    {
        health.ClientOnHealthChange += HandelHealthChange;
    }
    private void OnDestroy()
    {
        health.ClientOnHealthChange -= HandelHealthChange;
    }
    private void OnMouseEnter()
    {
        healthbarParent.SetActive(true);
    }
    private void OnMouseExit()
    {
        healthbarParent.SetActive(false);
    }
    void HandelHealthChange(int currentHealth , int maxHealth)
    {
        healthbarImgae.fillAmount = (float)currentHealth / maxHealth;
    }
}

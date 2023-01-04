using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    public TMP_Text purchaseObjectText;
    public TMP_Text ammoText;
    public TMP_Text creditsText;
    public TMP_Text healthText;
    public TMP_Text deathText;
    public static PlayerUI Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself. 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void ShowDeath()
    {
        deathText.gameObject.SetActive(true);
    }
    public void ShowHidePurchaseUI(bool val, string weaponName = "null", int cost = 0)
    { 
        purchaseObjectText.enabled = val;
        if (PlayerCredits.Instance.nearObject != null)
        {
            switch (PlayerCredits.Instance.nearObject.type)
            {
                case PurchasableObject.PurchaseType.Weapon:
                    purchaseObjectText.text = "E - Access " + weaponName + " for " + cost + " credits";
                    break;
                case PurchasableObject.PurchaseType.Health:
                    purchaseObjectText.text = cost + ": " + PlayerCredits.Instance.nearObject.healthToAdd + "HP";
                    break;
                case PurchasableObject.PurchaseType.Door:
                    purchaseObjectText.text = cost + ": Open " + weaponName;
                    break;
                default:
                    break;
            } 
        } 
    }
    private void Update()
    {
        UpdateAmmoCount();
        UpdateCreditsCount();
        UpdateHealthCount();
    }
    public void UpdateCreditsCount()
    { 
        creditsText.text = "Credits: " + PlayerCredits.Instance.credits;
    }
    public void UpdateHealthCount()
    {
        healthText.text = "Health: " + PlayerHealth.Instance.health;
    }
    public void UpdateAmmoCount()
    {
        Gun gun = WeaponSwitcher.Instance.activeWeapon;
        ammoText.text = gun.currentAmmo + "/" + gun.maxAmmo + "\n" + gun.spareAmmo;
    }
}

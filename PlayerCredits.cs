using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCredits : MonoBehaviour
{ 
    public int credits = 0;

    public PurchasableObject nearObject;

    public static PlayerCredits Instance { get; private set; }
    public AudioSource creditAudio;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearObject != null)
        {
            PurchaseWithCredits(nearObject.cost);
        }
    }
    public void PlayPickupSound(AudioClip clip)
    {
        creditAudio.clip = clip;
        creditAudio.PlayOneShot(clip, 1);
    }
    private void AddCredits(int amount)
    {
        credits += amount;
    }
    private bool PurchaseWithCredits(int amount)
    {
        if (credits >= amount && nearObject != null)
        {
            credits -= amount;
             
            switch (nearObject.type)
            {
                case PurchasableObject.PurchaseType.Weapon:
                    if (!WeaponSwitcher.Instance.unlockedWeapons[nearObject.unlockWeaponID].unlocked)
                    {
                        WeaponSwitcher.Instance.SelectWeaponWithID(nearObject.unlockWeaponID);
                        WeaponSwitcher.Instance.UnlockWeapon(nearObject.unlockWeaponID);
                    }
                    WeaponSwitcher.Instance.AddSpareAmmo(nearObject.ammoToAdd, nearObject.unlockWeaponID);
                    break;
                case PurchasableObject.PurchaseType.Health:
                    PlayerHealth.Instance.health += nearObject.healthToAdd;
                    break;
                case PurchasableObject.PurchaseType.Door:
                    PlayerUI.Instance.ShowHidePurchaseUI(false);
                    Destroy(nearObject.doorToOpen);
                    Destroy(nearObject.gameObject);
                    nearObject = null;
                    break;
                default:
                    break;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

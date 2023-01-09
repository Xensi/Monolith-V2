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
    public void AddCredits(int amount)
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
                    int id = nearObject.unlockWeaponID;
                    AddGun(id);
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
                case PurchasableObject.PurchaseType.MysteryBox:
                    int random = Random.Range(0, nearObject.weaponIDsToSelectRandomly.Count);
                    int randID = nearObject.weaponIDsToSelectRandomly[random];
                    AddGun(randID);
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
    private void AddGun(int id) //adds a new gun and switches to it or gives you ammo if you already have it
    {
        if (!WeaponSwitcher.Instance.unlockedWeapons[id].unlocked)
        {
            WeaponSwitcher.Instance.SelectWeaponWithID(id);
            WeaponSwitcher.Instance.UnlockWeapon(id);
        }
        WeaponSwitcher.Instance.AddSpareAmmo(WeaponSwitcher.Instance.gunList[id].spareAmmoWhenTakenFromBox, id);
    }
}

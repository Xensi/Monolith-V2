using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int creditValue = 0;
    public int healthValue = 0;
    public int ammoValue = 0;
    public AudioClip pickupSound;
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) //player layer
        {
            PlayerHealth.Instance.RaiseHealth(healthValue); 
            WeaponSwitcher.Instance.AddSpareAmmo(ammoValue, WeaponSwitcher.Instance.selectedWeapon);
            PlayerCredits.Instance.AddCredits(creditValue);
            PlayerCredits.Instance.PlayPickupSound(pickupSound);
            Destroy(transform.parent.gameObject);
        }
    }
}

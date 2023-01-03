using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int creditValue = 25;
    public AudioClip pickupSound;
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) //player layer
        {
            PlayerCredits.Instance.credits += creditValue;
            PlayerCredits.Instance.PlayPickupSound(pickupSound);
            Destroy(transform.parent.gameObject);
        }
    }
}

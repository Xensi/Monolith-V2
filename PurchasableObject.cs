using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableObject : MonoBehaviour
{

    public enum PurchaseType
    {
        Weapon,
        Health,
        Door
    }
    public PurchaseType type;
    public int unlockWeaponID = 0; 
    public string displayText = "Shotgun"; 
    public int cost = 100;
    public int ammoToAdd = 50;
    public int healthToAdd = 0;
    public bool playerCloseEnough = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) //player layer
        {
            playerCloseEnough = true;
            PlayerCredits.Instance.nearObject = this;
            PlayerUI.Instance.ShowHidePurchaseUI(true, displayText, cost);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7) //player layer
        {
            playerCloseEnough = false;
            PlayerCredits.Instance.nearObject = null;
            PlayerUI.Instance.ShowHidePurchaseUI(false, displayText, cost);
        }
    }

}

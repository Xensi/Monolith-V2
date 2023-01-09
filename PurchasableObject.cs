using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableObject : MonoBehaviour
{

    public enum PurchaseType
    {
        Weapon,
        Health,
        Door,
        MysteryBox,
    }
    [Header("All Settings")]
    public string displayText = "Shotgun";
    public int cost = 100;
    public bool playerCloseEnough = false;
    public PurchaseType type;

    [Header("Weapon Settings")]
    public int unlockWeaponID = 0;
    public int ammoToAdd = 50;
    [Header("Health Settings")]
    public int healthToAdd = 0;
    [Header("Door Settings")]
    public GameObject doorToOpen;
    [Header("Box Settings")]
    public List<int> weaponIDsToSelectRandomly;


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

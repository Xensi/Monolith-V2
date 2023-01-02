using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0;

    public List<UnlockedWeapon> unlockedWeapons;

    private int previousWeapon;

    private void Start()
    {
        SelectWeapon();
    }
    private void Update()
    {
        previousWeapon = selectedWeapon;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        } 
        while (!unlockedWeapons[selectedWeapon].unlocked) //selected weapon not unlocked
        {
            selectedWeapon++; //move until we get one that is unlocked
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
        }
        if (previousWeapon != selectedWeapon)
        {
            SelectWeapon();
        }

    }
    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}

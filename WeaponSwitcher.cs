using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0;
    public Gun activeWeapon;

    public List<UnlockedWeapon> unlockedWeapons;
    public List<Gun> gunList;

    private int previousWeapon;
    public LayerMask canHitLayerMask;
    public static WeaponSwitcher Instance { get; private set; }

    public bool firing = false;

    public Transform loweredPosition;
    public Transform gunNormalPos;
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
        //gun inputs
        if (Input.GetKeyDown(KeyCode.R) && activeWeapon.bulletsInMag < activeWeapon.maxMagSize && activeWeapon.spareAmmo > 0 && !activeWeapon.reloading)
        {
            activeWeapon.Reload();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && activeWeapon.firingType == Gun.FiringType.PumpAction && !activeWeapon.reloading && !activeWeapon.pumpingTheAction)
        {
            activeWeapon.PumpAction();
            return;
        }
        if (!Input.GetKey(KeyCode.E) && activeWeapon.firingType == Gun.FiringType.PumpAction && !activeWeapon.reloading && activeWeapon.pumpingTheAction && activeWeapon.pumpTimer >= activeWeapon.pullBackTime)
        {
            activeWeapon.ReleaseTheAction();
            return;
        }
        if (activeWeapon.bulletsInMag > 0 && !activeWeapon.reloading)
        {
            switch (activeWeapon.firingType)
            {
                case Gun.FiringType.Single: 
                    if (Input.GetMouseButtonDown(0) && !activeWeapon.coolingDown && activeWeapon.bulletInChamber) //single press
                    { 
                        activeWeapon.Shoot();
                    }
                    break;
                case Gun.FiringType.Automatic:
                    if (Input.GetMouseButton(0) && !activeWeapon.coolingDown && activeWeapon.bulletInChamber) //held
                    {
                        activeWeapon.Shoot();
                    }
                    break;
                case Gun.FiringType.Burst:
                    break;
                case Gun.FiringType.PumpAction:
                    if (Input.GetMouseButtonDown(0) && !activeWeapon.coolingDown && activeWeapon.bulletInChamber) //single press
                    {
                        activeWeapon.Shoot();
                    }
                    break;
                default:
                    break;
            }  
        }
        else if (activeWeapon.bulletsInMag <= 0 && !activeWeapon.dryFired)
        {
            switch (activeWeapon.firingType)
            {
                case Gun.FiringType.Single:
                    if (Input.GetMouseButtonDown(0) && !activeWeapon.coolingDown) //single press
                    {
                        activeWeapon.DryFire();
                    }
                    break;
                case Gun.FiringType.Automatic:
                    if (Input.GetMouseButton(0) && !activeWeapon.coolingDown) //held
                    {
                        activeWeapon.DryFire();
                    }
                    break;
                case Gun.FiringType.Burst:
                    break;
                default:
                    break;
            }
        }
        /*float halfTime = activeWeapon.reloadTime / 2; 
        if (activeWeapon.reloading && reloadTimer < halfTime)
        {
            reloadTimer += Time.deltaTime;
            float normalizedTime = reloadTimer / halfTime; // will = 1
            transform.localPosition = Vector3.Lerp(gunNormalPos.localPosition, loweredPosition.localPosition, normalizedTime); //return to zero based on return speed 
        }
        else if (activeWeapon.reloading && reloadTimer >= halfTime && backwardsTimer < halfTime) //returning with lerp is very fast for some reason
        {
            backwardsTimer += Time.deltaTime;
            float normalizedTime = backwardsTimer / halfTime; // will = 1
            transform.localPosition = Vector3.Lerp(loweredPosition.localPosition, gunNormalPos.localPosition, normalizedTime); //return to zero based on return speed 
        }
        else if (activeWeapon.reloading && backwardsTimer >= halfTime)
        {
            activeWeapon.FinishReload();
        }*/
    }
    public float reloadTimer = 0;
    public float backwardsTimer = 0; 

    public void UnlockWeapon(int id)
    {
        unlockedWeapons[id].unlocked = true;
        gunList[id].PlayReloadSound();
    }
    public void SelectWeaponWithID(int id)
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == id)
            {
                weapon.gameObject.SetActive(true);
                activeWeapon = gunList[id];
                Recoil.Instance.UpdateRecoilValues();
                Jolt.Instance.UpdateJoltValues();
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
    public void AddSpareAmmo(int ammo, int id)
    {
        gunList[id].spareAmmo += ammo;
    }
    public void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                activeWeapon = gunList[selectedWeapon];
                Recoil.Instance.UpdateRecoilValues();
                Jolt.Instance.UpdateJoltValues();
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}

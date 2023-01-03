using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    public float recoilX = -2; //up/down
    public float recoilY = 2;
    public float recoilZ = 0.35f;
    public float snappiness = 6;
    public float returnSpeed = 2;
    public static Recoil Instance { get; private set; }
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
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime); //return to zero based on return speed
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void UpdateRecoilValues()
    {
        recoilX = WeaponSwitcher.Instance.activeWeapon.recoilX;
        recoilY = WeaponSwitcher.Instance.activeWeapon.recoilY;
        recoilZ = WeaponSwitcher.Instance.activeWeapon.recoilZ;
    }
    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}

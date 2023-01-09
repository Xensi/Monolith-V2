using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jolt : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;
     
    private Vector3 targetPos;
    public Vector3 defaultPos;
    public Vector3 push;

    public float joltX = -2; //up/down
    public float joltY = 2;
    public float joltZ = 0.35f;
    public float joltSnapSpeed = 6;
    public float joltReturnSpeed = 100f;
    public static Jolt Instance { get; private set; }
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
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, joltReturnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, joltSnapSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);


        targetPos = Vector3.Lerp(targetPos, defaultPos, joltReturnSpeed * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, joltSnapSpeed * Time.fixedDeltaTime);
        
    }
    public void UpdateJoltValues()
    {
        joltX = WeaponSwitcher.Instance.activeWeapon.joltX;
        joltY = WeaponSwitcher.Instance.activeWeapon.joltY;
        joltZ = WeaponSwitcher.Instance.activeWeapon.joltZ;
        joltReturnSpeed = WeaponSwitcher.Instance.activeWeapon.joltReturnSpeed;
    }
    public void FireJolt()
    {
        targetRotation += new Vector3(joltX, Random.Range(-joltY, joltY), Random.Range(-joltZ, joltZ));
        targetPos += push;
    }
}

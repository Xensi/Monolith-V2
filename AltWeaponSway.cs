using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltWeaponSway : MonoBehaviour
{
    public float tiltMult = 2;
    public float smooth = 8;
    public float timer;
    public float waveSliceY;
    public float waveSliceX;
    public Vector3 finalPosition;
    public Vector3 initialPosition;
    public float idleSwayCapMult = 0.05f; 
    public float moveSwaySpeedMultX = 5f;
    public float moveSwaySpeedMultY = 10f;
    public float moveSwayCapMultX = 0.1f;
    public float moveSwayCapMultY = 0.5f;
    public float velocityThreshold = 0.01f;
    public CharacterController controller;
    public float playerSpeed;
    public Vector3 lastPos; 
    private void Start()
    {
        initialPosition = transform.localPosition;
        controller = PlayerMovement.Instance.controller;
        lastPos = controller.transform.position;
    }
    private void Update()
    {
        WeaponTilt();
        Sway();
    }
    private void WeaponTilt()
    { 
        float mouseX = Input.GetAxisRaw("Mouse X") * tiltMult;

        float mouseY = Input.GetAxisRaw("Mouse Y") * tiltMult;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        Vector3 currentPos = controller.transform.position;
        Vector3 delta = currentPos - lastPos;
        playerSpeed = delta.magnitude;

        lastPos = controller.transform.position;
    }
    private void Sway()
    {
        if (!WeaponSwitcher.Instance.firing && PlayerMovement.Instance.isGrounded && !WeaponSwitcher.Instance.activeWeapon.reloading)
        {
            timer += Time.deltaTime;

            if (playerSpeed < velocityThreshold)
            {
                waveSliceY = Mathf.Sin(timer) * idleSwayCapMult;
                waveSliceX = Mathf.Sin(timer) * idleSwayCapMult;
            }
            else if (!Input.GetKey(KeyCode.LeftShift)) //walking
            {
                waveSliceY = Mathf.Sin(timer * moveSwaySpeedMultY) * moveSwayCapMultY;
                waveSliceX = Mathf.Sin(timer * moveSwaySpeedMultX) * moveSwayCapMultX;
            }
            else
            {
                waveSliceY = Mathf.Sin(timer * moveSwaySpeedMultY * 2) * moveSwayCapMultY * 2;
                waveSliceX = Mathf.Sin(timer * moveSwaySpeedMultX * 2) * moveSwayCapMultX * 2;
            }
            finalPosition = new Vector3(waveSliceX, waveSliceY, 0); //0 keeps z axis the same 
            transform.localPosition = Vector3.Slerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime);
        } 
    }
}

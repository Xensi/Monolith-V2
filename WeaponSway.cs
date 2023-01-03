using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class WeaponSway : MonoBehaviour
{
    [Header("Position")]
    public float amount = 0.1f; //weapon sway amount
    private float actualAmount; //amount used in calculations
    public float maxAmount = 1f;
    public float smoothAmount = 6f; //smoothing amount

    [Header("Rotation")]
    public float rotationAmount = 8f;
    private float actualRotationAmount;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;
     
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true; 

    private Vector3 initialPosition; //initial pos, where weapon will return to
    private Quaternion initialRotation;
    private Vector2 moveAxis;

    private float inputX;
    private float inputY;

    private float moveX;
    private float moveY;
    private Vector3 finalPosition;

    public float jumpY;
    public float moveTimer;

    public bool idle = true;

    public float waveslice;
    public float wavesliceX;

    void Start()
    {
        initialPosition = transform.localPosition; //local position respects parent pos
        initialRotation = transform.localRotation;
    }

    private void Awake()
    {
        actualAmount = amount; //actual amount and amount are used just so we can remember starting val
        actualRotationAmount = rotationAmount; 
    }

    // Update is called once per frame
    void Update()
    { 
        ADSAdjust(); //lower sway while adsing 

        MoveSway(); //clamp and apply sway 
    }  
    private void MoveSway()
    {
        if (jumpY < 0)
        {
            jumpY += .01f;
        }
        if (jumpY > 0)
        {
            jumpY -= .01f;
        }
        if (jumpY > .03f)
        {
            jumpY = .03f;
        }
        //Debug.Log(jumpY);
        moveX = Mathf.Clamp(inputX * actualAmount, -maxAmount, maxAmount); //limit the max movement sway
        moveY = Mathf.Clamp(inputY * actualAmount, -maxAmount, maxAmount);
        moveTimer += Time.deltaTime;
        //movetimer is determined by custom controller.cs

        if (idle == true)
        {
            waveslice = Mathf.Sin(moveTimer / 80) / 80; //divide movetimer to slow it down, then divide sin to make it small enough to use
            wavesliceX = Mathf.Sin(moveTimer / 120) / 160;
            /*if (Mouse.current.rightButton.isPressed)
            {
                waveslice = Mathf.Sin(moveTimer / 80) / 100; //ads has same speed, but smaller size
                wavesliceX = Mathf.Sin(moveTimer / 120) / 180;
            }*/
        }
        /*else if (!Keyboard.current.shiftKey.isPressed) //while walking
        {
            waveslice = Mathf.Sin(moveTimer / 15) / 60; //divide movetimer to slow it down, then divide sin to make it small enough to use
            wavesliceX = Mathf.Sin(moveTimer / 30) / 80;

            *//*if (Mouse.current.rightButton.isPressed)
            {
                waveslice = Mathf.Sin(moveTimer / 15) / 80;//ads has same speed, but smaller size
                wavesliceX = Mathf.Sin(moveTimer / 30) / 100;
            }*//*

        }*/
        else
        { //running

            waveslice = Mathf.Sin(moveTimer / 15) / 30; //divide movetimer to slow it down, then divide sin to make it small enough to use
            wavesliceX = Mathf.Sin(moveTimer / 30) / 40;
            /*if (Mouse.current.rightButton.isPressed)
            {
                waveslice = Mathf.Sin(moveTimer / 15) / 40;//ads has same speed, but smaller size
                wavesliceX = Mathf.Sin(moveTimer / 30) / 50;
            }*/
        }

        //Debug.Log(waveslice);
        finalPosition = new Vector3(moveX + wavesliceX, moveY + jumpY + waveslice, 0); //0 keeps z axis the same


        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount); //linear interpolation. move from 1 pos to another pos based on speed val, returns to default pos

    } 
    private void ADSAdjust()
    {
        /*if (Mouse.current.rightButton.isPressed) //if aiming down sights
        {
            actualAmount = amount / 4;
            actualRotationAmount = rotationAmount / 4;
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            actualAmount = amount;
            actualRotationAmount = rotationAmount;
        }*/
    }

}

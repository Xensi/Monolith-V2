using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 5;
    public LayerMask interactableMask;
    public Rigidbody pickedUpObject;
    public Transform holdPos;
    public float moveForce = 150;
    public float autoDropDistance = 3;
     
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) )
        {
            if (pickedUpObject == null)
            {
                Vector3 shootDirection = transform.forward;
                shootDirection.Normalize();
                Ray ray = new Ray(transform.position, shootDirection);
                RaycastHit hit; //otherwise, make raycast

                if (Physics.Raycast(ray, out hit, interactionRange, interactableMask)) //if raycast hits something  
                {
                    if (hit.rigidbody != null && !hit.collider.gameObject.isStatic)
                    {
                        PickupObject(hit.rigidbody);
                    }
                }
            }
            else
            {
                DropObject();
            }
        }

        if (pickedUpObject != null)
        {
            MoveObject();
        }        
    } 
    private void MoveObject()
    {
        if (Vector3.Distance(pickedUpObject.transform.position, holdPos.position) > 0.1f)
        {
            Vector3 moveDir = (holdPos.position - pickedUpObject.transform.position);
            pickedUpObject.AddForce(moveDir * moveForce);
        }
        if (Vector3.Distance(pickedUpObject.transform.position, holdPos.position) >= autoDropDistance)
        {
            DropObject();
        }
    }
    private void PickupObject(Rigidbody body)
    { 
        pickedUpObject = body;
        pickedUpObject.useGravity = false;
        pickedUpObject.drag = 10;
        pickedUpObject.constraints = RigidbodyConstraints.FreezeRotation;
        pickedUpObject.transform.parent = holdPos;
    }
    private void DropObject()
    { 
        pickedUpObject.useGravity = true;
        pickedUpObject.constraints = RigidbodyConstraints.None;
        pickedUpObject.drag = 1;
        pickedUpObject.transform.parent = null;
        pickedUpObject.velocity = PlayerMovement.Instance.controller.velocity;
        pickedUpObject = null;
    }
}

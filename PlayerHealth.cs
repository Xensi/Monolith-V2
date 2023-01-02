using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public PlayerMovement movementScript;
    public static PlayerHealth Instance { get; private set; }
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
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        movementScript.speed = 0;
    }

}

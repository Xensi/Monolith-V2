using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100;
    public GameObject parent;
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
        Destroy(parent);
    }
}

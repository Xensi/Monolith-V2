using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100;
    public GameObject parent;

    public List<GameObject> itemsToDrop;

    public GameObject deathEffect;

    public void Die()
    {
        Instantiate(itemsToDrop[Random.Range(0, itemsToDrop.Count)], transform.position, Quaternion.identity);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(parent);
    }
}

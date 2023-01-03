using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour
{
    public int damage = 20;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 9) //hitbox
        {
            PlayerHealth.Instance.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
    private void Start()
    {
        Destroy(gameObject, 10);
    }
}

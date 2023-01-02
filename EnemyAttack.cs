using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool inRangeToAttack = false;
    public bool attackCooling = false;
    public float attackCooldownTime = 1;
    public float attackDistance = 0.5f;
    public LayerMask playerMask;
    public float attackDamage = 30f; 
     
    void Update()
    {
        if (!attackCooling)
        { 
            inRangeToAttack = Physics.CheckSphere(transform.position, attackDistance, playerMask);
        }
        else
        {
            inRangeToAttack = false;
        }
        if (inRangeToAttack)
        {
            PlayerHealth.Instance.TakeDamage(attackDamage);
            attackCooling = true;
            Invoke("AttackCooldownFinishes", attackCooldownTime);
        }
    }

    void AttackCooldownFinishes()
    {
        attackCooling = false;
    }

}

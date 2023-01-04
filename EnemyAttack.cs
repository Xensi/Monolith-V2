using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Projectile,
        Raycast
    }
    public AttackType attackType;
    public GameObject projectilePrefab;
    public float spreadRangeY = 0.01f;
    public float spreadRangeX = 0.01f;
    public Transform shootPoint;
    public float shootForce = 100;

    public bool inRangeToAttack = false;
    public bool attackCooling = false;
    public float attackCooldownTime = 1;
    public float attackDistance = 0.5f;
    public LayerMask playerMask;
    public float attackDamage = 30f; //melee only
    public AudioSource movingSource;
    public AudioSource attackSource;
    public AudioClip attackSound;

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
            Attack();
        }
    }
    private void Attack()
    {
        switch (attackType)
        {
            case AttackType.Melee:
                PlayerHealth.Instance.TakeDamage(attackDamage);
                break;
            case AttackType.Projectile:
                Vector3 startPoint;
                if (shootPoint != null)
                {
                    startPoint = shootPoint.position;
                }
                else
                {
                    startPoint = transform.position;
                }
                Vector3 direction = PlayerHealth.Instance.transform.position - startPoint;
                direction += new Vector3(Random.Range(-spreadRangeX, spreadRangeX), Random.Range(-spreadRangeY, spreadRangeY), 0); //add spread

                GameObject projectile = Instantiate(projectilePrefab, startPoint, Quaternion.identity);
                projectile.transform.forward = direction.normalized; //rotate towards target
                projectile.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);  
                break;
            case AttackType.Raycast:
                break;
            default:
                break;
        }
        PlayAttackSound();
        attackCooling = true;
        Invoke("AttackCooldownFinishes", attackCooldownTime);
    }

    private void PlayAttackSound()
    {
        attackSource.PlayOneShot(attackSound, 1);
    }
    void AttackCooldownFinishes()
    {
        attackCooling = false;
    }

}

using UnityEngine;
using Pathfinding;

public class DamageableHitbox : MonoBehaviour
{
    public EnemyHealth healthScript;
    public float damageMultiplier = 1;
    public float damageDealtToThisPart = 0;
    public float damageToBreakPart = 50;
    public float damageToInstantKill = -1;
    public float speedDebuff = 5; 

    public enum PartBrokenBehavior
    {
        None,
        RemoveSight,
        ImpairMovement,
        LoseShield,
    }
    public PartBrokenBehavior brokenBehavior;
    public Renderer rendererToSwapMat;
    public Material swapMaterial;
    public EnemyBehavior behavior;
    public bool partBroken = false;
    public GameObject partBrokenEffect;
    public void TakeDamage(float damage)
    {
        float totalDamage = damage * damageMultiplier;
        if (!partBroken)
        {
            healthScript.health -= totalDamage;
            damageDealtToThisPart += totalDamage;
            if (damageDealtToThisPart >= damageToBreakPart)
            {
                BreakPart();
            }
            if (healthScript.health <= 0)
            {
                healthScript.Die();
            }
        }
        else if (damageToInstantKill > 0) //part broken and can be used to instant kill
        { 
            healthScript.health -= totalDamage;
            damageDealtToThisPart += totalDamage; 
            if (healthScript.health <= 0 || damageDealtToThisPart >= damageToInstantKill)
            {
                healthScript.Die();
            }
        }
    }
    private void BreakPart()
    {
        partBroken = true;
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (partBrokenEffect != null) Instantiate(partBrokenEffect, transform.position + col.center, Quaternion.identity);
        switch (brokenBehavior)
        {
            case PartBrokenBehavior.None:
                break;
            case PartBrokenBehavior.RemoveSight:
                behavior.ImpairVision();
                if (rendererToSwapMat != null) rendererToSwapMat.material = swapMaterial;
                break;
            case PartBrokenBehavior.LoseShield:
                break;
            case PartBrokenBehavior.ImpairMovement:
                behavior.DebuffSpeed(speedDebuff);
                break;
            default:
                break;
        }
    }
}

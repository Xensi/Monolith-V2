using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum BulletType
    {
        Raycast,
        Projectile
    }
    public enum FiringType
    {
        Single,
        Automatic,
        Charge,
        Burst, //not implemented 
        PumpAction
    }
    [Header("Cosmetic")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public float bulletSpeed = 500;
    public GameObject fakeBulletPrefab;

    [Header("Gun Settings")]
    public float bulletDamage = 1;
    public int bulletsPerShot = 1;
    public float firingFrequency = 0.1f;
    public float bulletForce = 100;
    public float spreadRange = 0.01f;
    public float range = 1000;
    public float reloadTime = 1f;
    public int bulletsReloadedPerReload = -1;
    public BulletType bulletType;
    public GameObject projectilePrefab;
    public float instantExplosionDistance = 2;
    public FiringType firingType;


    [Header("Ammo Settings")]
    public int spareAmmo = 100;
    public int maxMagSize = 10;
    public int bulletsInMag;
    public bool bulletInChamber = true;
    public int ammoDrainPerShot = 1;

    [Header("Camera Recoil")]
    public float recoilX = -2; //up/down
    public float recoilY = 2;
    public float recoilZ = 0.35f;
    [Header("Procedural Gun Recoil")]
    public float joltX = -25; //up/down
    public float joltY = 2;
    public float joltZ = 2;
    public float joltReturnSpeed = 100;

    [Header("Sound Settings")]
    public List<AudioClip> shootSounds;
    public AudioSource weaponAudioSource;
    public AudioClip reloadSound;
    public AudioClip emptyFireSound;
    public AudioClip pumpSound;
    public AudioClip pumpReleaseSound;
    public float pitchVariance = 0.1f;

    [Header("Box Settings")]
    public int spareAmmoWhenTakenFromBox = 50; 
    [Header("Procedural Reload Settings")]
    public float pullBackTime = .1f;
    public float releaseTime = .1f;
    public float delayTime = .5f;
    public Transform heldBackPosition;
    public Transform heldForwardPosition;
    public GameObject pumpBone; 

    public bool pumpingTheAction = false;
    public bool pushingTheAction = false;
    public float pumpTimer = 0;
    public float pushTimer = 0;
    public float delayTimer = 0;

    //Does not need to be seen
    public bool reloading = false;
    [HideInInspector] public bool coolingDown = false;

    //public float firingFrequencyIncreasePerBullet = 0f; //speed up over time?
    //public float minimumFiringFrequency = 0.01f;
    //public float spreadIncreasePerBullet = 0;

    public bool dryFired = false;


    private void Start()
    {
        bulletsInMag = maxMagSize; 
    }

    public GameObject crosshair;
    private void OnEnable()
    {
        if (crosshair != null) crosshair.SetActive(true);
    }
    private void OnDisable()
    {
        reloading = false;
        if (crosshair != null) crosshair.SetActive(false);
    }
    public void Reload()
    {
        reloading = true;
        PlayLoadShellSound();
        /*if (firingType == FiringType.PumpAction)
        {
            PlayLoadShellSound();
        }
        else
        { 
            PlayReloadSound();
        }*/
    }
    private void PlayLoadShellSound()
    { 
        float defaultPitch = 1;
        weaponAudioSource.pitch = defaultPitch + Random.Range(-pitchVariance, pitchVariance);
        weaponAudioSource.PlayOneShot(reloadSound, 1f);
    }
    public void PlayReloadSound()
    {
        float defaultPitch = 1;
        weaponAudioSource.pitch = defaultPitch + Random.Range(-pitchVariance, pitchVariance); 
        weaponAudioSource.PlayOneShot(pumpSound, 1f);
    }
    public void PlayReloadSoundPart2()
    {
        float defaultPitch = 1;
        weaponAudioSource.pitch = defaultPitch + Random.Range(-pitchVariance, pitchVariance);
        weaponAudioSource.PlayOneShot(pumpReleaseSound, 1f);
    }
    public void FinishReload()
    {
        if (bulletsReloadedPerReload == -1)
        {
            while (bulletsInMag < maxMagSize && spareAmmo > 0)
            {
                bulletsInMag++;
                spareAmmo--;
            }
        }
        else
        {
            for (int i = 0; i < bulletsReloadedPerReload; i++)
            {
                if (bulletsInMag < maxMagSize && spareAmmo > 0)
                {
                    bulletsInMag++;
                    spareAmmo--;
                }
            }
        }
        
        reloading = false;
        dryFired = false;
        returningMag = false;
        reloadTimer = 0;
        backwardsTimer = 0;
        delayTimer = 0;
    }
    public void PumpAction()
    {
        pumpingTheAction = true;
        pushingTheAction = false;
        PlayReloadSound();
    }
    public void ReleaseTheAction()
    {
        pumpingTheAction = false;
        pushingTheAction = true;
        PlayReloadSoundPart2();
    }
    public void FinishPumpingAction()
    { 
        bulletInChamber = true;
        pumpTimer = 0;
        pushTimer = 0;

        pumpingTheAction = false;
        pushingTheAction = false;
    }
    private void GoldenEyeReload()
    {
        float halfTime = reloadTime / 2;
        if (reloading && reloadTimer < halfTime)
        {
            reloadTimer += Time.deltaTime;
            float normalizedTime = reloadTimer / halfTime; // will = 1
            transform.position = Vector3.Lerp(WeaponSwitcher.Instance.gunNormalPos.position, WeaponSwitcher.Instance.loweredPosition.position, normalizedTime); //return to zero based on return speed 
        }
        else if (reloading && reloadTimer >= halfTime && backwardsTimer < halfTime)
        {
            backwardsTimer += Time.deltaTime;
            float normalizedTime = backwardsTimer / halfTime; // will = 1
            transform.position = Vector3.Lerp(WeaponSwitcher.Instance.loweredPosition.position, WeaponSwitcher.Instance.gunNormalPos.position, normalizedTime); //return to zero based on return speed 
        }
        else if (reloading && backwardsTimer >= halfTime)
        {
            FinishReload();
        }
    }
    private void Update()
    {
        if (firingType == FiringType.PumpAction)
        {
            if (reloading)
            {
                GoldenEyeReload();
            }
            else
            {
                if (pumpingTheAction && pumpTimer < pullBackTime)
                {
                    pumpTimer += Time.deltaTime;
                    float normalizedTime = pumpTimer / pullBackTime; // will = 1
                    pumpBone.transform.position = Vector3.Lerp(heldForwardPosition.position, heldBackPosition.position, normalizedTime); //return to zero based on return speed 
                }
                else if (pushingTheAction && pumpTimer >= pullBackTime && pushTimer < releaseTime)
                {
                    pushTimer += Time.deltaTime;
                    float normalizedTime = pushTimer / releaseTime; // will = 1
                    pumpBone.transform.position = Vector3.Lerp(heldBackPosition.position, heldForwardPosition.position, normalizedTime); //return to zero based on return speed 
                }
                else if (pushingTheAction && pushTimer >= releaseTime)
                {
                    FinishPumpingAction();
                }
            }  
        }
        else if (reloading)
        {
            GoldenEyeReload();
            /*if (reloadTimer < pullBackTime)
            {
                reloadTimer += Time.deltaTime;
                float normalizedTime = reloadTimer / pullBackTime; // will = 1
                pumpBone.transform.position = Vector3.Lerp(heldForwardPosition.position, heldBackPosition.position, normalizedTime); //return to zero based on return speed  
            }
            else if (delayTimer < delayTime)
            {
                delayTimer += Time.deltaTime;
            }
            else if (backwardsTimer < releaseTime)
            {
                if (!returningMag)
                {
                    PlayReloadSoundPart2();
                    returningMag = true;
                }
                backwardsTimer += Time.deltaTime;
                float normalizedTime = backwardsTimer / releaseTime; // will = 1
                pumpBone.transform.position = Vector3.Lerp(heldBackPosition.position, heldForwardPosition.position, normalizedTime); //return to zero based on return speed  
            }
            else
            { 
                FinishReload();
            } */
        } 
    }
    public float reloadTimer = 0;
    public float backwardsTimer = 0;
    public bool returningMag = false;
    public void Shoot(bool raycast = true)
    {
        CancelInvoke("TreatAsNotFiring");
        WeaponSwitcher.Instance.firing = true;
        Invoke("TreatAsNotFiring", 0.1f);


        //fire bullet in chamber
        bulletInChamber = false;
        switch (firingType)
        {
            case FiringType.Single: 
            case FiringType.Automatic:
                bulletInChamber = true;
                bulletsInMag -= ammoDrainPerShot;
                break;
            case FiringType.Charge:
                break;
            case FiringType.Burst:
                break;
            case FiringType.PumpAction:
                bulletsInMag -= ammoDrainPerShot;
                break;
            default:
                break;
        }

        //cosmetics
        Recoil.Instance.RecoilFire();
        Jolt.Instance.FireJolt();
        if (muzzleFlash != null) muzzleFlash.Play();
        PlayShootSound();
        //

        Vector3 bulletOrigin;
        if (muzzleFlash != null)
        {
            bulletOrigin = muzzleFlash.transform.position;
        }
        else
        {
            bulletOrigin = transform.position;
        }
        for (int i = 0; i < bulletsPerShot; i++) //fire bullets
        {
            Vector3 shootDirection = transform.forward + new Vector3(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange));
            shootDirection.Normalize();

            switch (bulletType)
            {
                case BulletType.Raycast:
                    Ray ray = new Ray(transform.position, shootDirection);
                    RaycastHit hit; //otherwise, make raycast

                    if (Physics.Raycast(ray, out hit, range, WeaponSwitcher.Instance.canHitLayerMask)) //if raycast hits something  
                    {
                        GameObject bullet = Instantiate(fakeBulletPrefab, bulletOrigin, Quaternion.identity); //uses muzzle flash as starting pos
                        bullet.transform.forward = shootDirection; //face direction

                        StartCoroutine(SpawnBulletCosmetic(bullet, hit.point, true, hit.normal));

                        if (hit.rigidbody != null) //shooting rigidbodies pushes them because it's fun
                        {
                            hit.rigidbody.AddForceAtPosition(shootDirection * bulletForce, hit.point);
                            //hit.rigidbody.AddForce(-hit.normal * bulletForce);
                        }

                        DamageableHitbox hitbox = hit.transform.GetComponent<DamageableHitbox>(); //get target component 
                        if (hitbox != null) //if target exists
                        {
                            hitbox.TakeDamage(bulletDamage); //call target damage function
                        }
                    }
                    else //if raycast doesn't hit, still fire a fake bullet
                    {
                        GameObject bullet = Instantiate(fakeBulletPrefab, bulletOrigin, Quaternion.identity); //uses muzzle flash as starting pos
                        StartCoroutine(SpawnBulletCosmetic(bullet, shootDirection * range, false, Vector3.zero));
                    }
                    break;
                case BulletType.Projectile:

                    GameObject projectile = Instantiate(projectilePrefab, bulletOrigin, Quaternion.identity);
                    Rigidbody rigid = projectile.GetComponent<Rigidbody>();
                    ImpactProjectile impact = projectile.GetComponent<ImpactProjectile>();

                    Ray ray2 = new Ray(transform.position, shootDirection);
                    RaycastHit hit2;  
                    if (Physics.Raycast(ray2, out hit2, instantExplosionDistance, WeaponSwitcher.Instance.canHitLayerMask)) //if raycast hits something  
                    {
                        impact.transform.position = hit2.point;
                        impact.Explode(impact.transform.position, impact.damageRadius);
                    }
                    else
                    { 
                        rigid.AddForce(shootDirection * bulletForce);
                    }
                    break;
                default:
                    break;
            } 
        }

        coolingDown = true;
        Invoke("FinishCooldown", firingFrequency);
    }

    public void DryFire()
    {
        float defaultPitch = 1;
        weaponAudioSource.pitch = defaultPitch + Random.Range(-pitchVariance, pitchVariance);
        weaponAudioSource.PlayOneShot(emptyFireSound, 1);
        dryFired = true;
    }
    private void TreatAsNotFiring()
    {
        WeaponSwitcher.Instance.firing = false;
    }
    private void PlayShootSound()
    {
        if (shootSounds.Count > 0)
        {
            AudioClip clip = shootSounds[Random.Range(0, shootSounds.Count)];
            //weaponAudioSource.clip = clip;
            float defaultPitch = 1;
            weaponAudioSource.pitch = defaultPitch + Random.Range(-pitchVariance, pitchVariance);
            weaponAudioSource.PlayOneShot(clip, 1f);
        }
    }
    void FinishCooldown()
    {
        coolingDown = false;
    }

    private IEnumerator SpawnBulletCosmetic(GameObject bullet, Vector3 endPoint, bool hitSomething, Vector3 normal)
    { 
        Vector3 startPos = bullet.transform.position;
        float distance = Vector3.Distance(startPos, endPoint);
        float startDistance = distance;
        while (distance > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPos, endPoint, 1 - (distance/startDistance));
            distance -= Time.deltaTime * bulletSpeed;
            yield return null;
        }
        ParticleSystem hitParticleEffect = Instantiate(impactEffect, endPoint, Quaternion.LookRotation(normal)); //create a particle effect on shot
        Destroy(hitParticleEffect.gameObject, 2);
        
        bullet.transform.position = endPoint;
        Destroy(bullet);
    }
}

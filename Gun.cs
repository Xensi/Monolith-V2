using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
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
     
    [Header("Ammo Settings")]
    public int spareAmmo = 100;
    public int maxAmmo = 10;
    public int currentAmmo;
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
    public float pitchVariance = 0.1f;

    //Does not need to be seen
    private bool reloading = false;
    private bool coolingDown = false;

    //public float firingFrequencyIncreasePerBullet = 0f; //speed up over time?
    //public float minimumFiringFrequency = 0.01f;
    //public float spreadIncreasePerBullet = 0;


    public enum FiringType
    {
        Single,
        Automatic,
        Burst, //not implemented
        Projectile,
        Beam
    }
    public FiringType firingType;
    private void Start()
    {
        currentAmmo = maxAmmo; 
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && spareAmmo > 0 && !reloading)
        {
            Reload();
            return;
        }
        if (currentAmmo > 0 && !reloading)
        { 
            if (firingType == FiringType.Single || firingType == FiringType.Burst)
            {
                if (Input.GetButtonDown("Fire1") && !coolingDown)
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButton("Fire1") && !coolingDown)
                {
                    Shoot();
                }
            }
        } 
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
    void Reload()
    {
        reloading = true;
        PlayReloadSound();

        Invoke("FinishReload", reloadTime);
    }
    public void PlayReloadSound()
    { 
        weaponAudioSource.clip = reloadSound;
        weaponAudioSource.PlayOneShot(reloadSound, 1f);
    }
    void FinishReload()
    {
        while (currentAmmo < maxAmmo && spareAmmo > 0)
        {
            currentAmmo++;
            spareAmmo--;
        }
        reloading = false;
    }
    void Shoot()
    {
        CancelInvoke("TreatAsNotFiring");
        WeaponSwitcher.Instance.firing = true;
        Invoke("TreatAsNotFiring", 0.1f);

        currentAmmo -= ammoDrainPerShot;

        Recoil.Instance.RecoilFire();
        Jolt.Instance.FireJolt();

        for (int i = 0; i < bulletsPerShot; i++) //fire bullets
        {
            Vector3 shootDirection = transform.forward + new Vector3(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange), 0);
            shootDirection.Normalize();
            Ray ray = new Ray(transform.position, shootDirection);
            RaycastHit hit; //otherwise, make raycast

            if (Physics.Raycast(ray, out hit, range, WeaponSwitcher.Instance.canHitLayerMask)) //if raycast hits something  
            {
                GameObject bullet = Instantiate(fakeBulletPrefab, muzzleFlash.transform.position, Quaternion.identity); //uses muzzle flash as starting pos
                bullet.transform.forward = shootDirection; //face direction

                StartCoroutine(SpawnBulletCosmetic(bullet, hit.point, true, hit.normal)); 

                if (hit.rigidbody != null) //shooting rigidbodies pushes them because it's fun
                {
                    hit.rigidbody.AddForceAtPosition(shootDirection * bulletForce, hit.point);
                    //hit.rigidbody.AddForce(-hit.normal * bulletForce);
                }

                EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>(); //get target component 
                if (enemy != null) //if target exists
                {
                    enemy.TakeDamage(bulletDamage); //call target damage function
                }
            }
            else //if raycast doesn't hit, still fire a fake bullet
            {

                GameObject bullet = Instantiate(fakeBulletPrefab, muzzleFlash.transform.position, Quaternion.identity); //uses muzzle flash as starting pos
                StartCoroutine(SpawnBulletCosmetic(bullet, shootDirection * range, false, Vector3.zero)); 
            }
        } 
        muzzleFlash.Play();
        PlayShootSound();

        coolingDown = true;
        Invoke("FinishCooldown", firingFrequency);
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

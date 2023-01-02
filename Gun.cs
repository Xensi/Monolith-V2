using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float bulletDamage = 1;
    public int bulletsPerShot = 1;
    public float firingFrequency = 0.1f; 
    public bool coolingDown = false;
    public float spreadRange = 0.01f;
    public float range = 1000;
    public float bulletForce = 1;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public List<AudioClip> shootSounds;
    public AudioSource weaponAudioSource;

    public int spareAmmo = 100;
    public int maxAmmo = 10;
    public int currentAmmo;
    public int ammoDrainPerShot = 1;
    public float reloadTime = 1f;

    public AudioClip reloadSound;

    public bool reloading = false;
    //public LayerMask excludePlayerMask;

    public enum FiringType
    {
        Single,
        Burst,
        Automatic
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
    private void OnDisable()
    {
        reloading = false;
    }
    void Reload()
    {
        reloading = true;
        weaponAudioSource.clip = reloadSound;
        weaponAudioSource.PlayOneShot(reloadSound, 1f);

        Invoke("FinishReload", reloadTime);
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
        currentAmmo -= ammoDrainPerShot;
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shootDirection = transform.forward + transform.TransformDirection(new Vector3(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange))); 
            Ray ray = new Ray(transform.position, shootDirection);
            RaycastHit hit; //otherwise, make raycast

            if (Physics.Raycast(ray, out hit, range)) //if raycast hits something //~excludePlayerMask
            {
                ParticleSystem hitParticleEffect = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); //create a particle effect on shot
                Destroy(hitParticleEffect, 2);

                if (hit.rigidbody != null) //shooting rigidbodies pushes them because it's fun
                {
                    hit.rigidbody.AddForceAtPosition(shootDirection * bulletForce, hit.point);
                }

                EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>(); //get target component 
                if (enemy != null) //if target exists
                {
                    enemy.TakeDamage(bulletDamage); //call target damage function
                }
            }
        }

        muzzleFlash.Play();
        PlayShootSound();

        coolingDown = true;
        Invoke("FinishCooldown", firingFrequency);
    }
    private void PlayShootSound()
    {
        if (shootSounds.Count > 0)
        {
            AudioClip clip = shootSounds[Random.Range(0, shootSounds.Count)];
           /* if (weaponAudioSource.isPlaying && RapidFire)
            {
                weaponAudioSource.Stop();
            }*/
            weaponAudioSource.clip = clip;
            weaponAudioSource.PlayOneShot(clip, 1f);
        }
    }
    void FinishCooldown()
    {
        coolingDown = false;
    }

}

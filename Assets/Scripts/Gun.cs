using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public float damage = 5;
    public int projectilePerMag;
    public float reloadTime;
    float nextShootTime;
    AudioSource audioSource;
    Transform bullets;
    public AudioClip shootAudioClip;
    public AudioClip reloadAudioClip;
    public Transform shell;
    public Transform shellEjection;
    [Tooltip("连发true，单发false")]
    public bool shootMode;
    MuzzleFlash muzzleFlash;
    int projectileRemainging;
    bool isReloading;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        bullets = GameObject.Find("Bullets").transform;
        muzzleFlash = GetComponent<MuzzleFlash> ();
        projectileRemainging = projectilePerMag;
    }
    public void Shoot()
    {
        if (!isReloading && Time.time > nextShootTime && projectileRemainging > 0) {
            nextShootTime = Time.time + msBetweenShots / 1000;
            projectileRemainging--;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation, bullets);
            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.SetDamage(damage);
            audioSource.PlayOneShot(shootAudioClip);
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            if (projectileRemainging <= 0) {
                Reload();
            }
        }
    }

    public void Reload()
    {
        if (!isReloading && projectileRemainging != projectilePerMag) {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        audioSource.PlayOneShot(reloadAudioClip);
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        projectileRemainging = projectilePerMag;
    }
}

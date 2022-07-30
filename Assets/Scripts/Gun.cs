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
    float nextShootTime;
    AudioSource audioSource;
    Transform bullets;
    public AudioClip audioClip;
    public Transform shell;
    public Transform shellEjection;
    [Tooltip("连发true，单发false")]
    public bool shootMode;
    MuzzleFlash muzzleFlash;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        bullets = GameObject.Find("Bullets").transform;
        muzzleFlash = GetComponent<MuzzleFlash> ();
    }
    public void Shoot()
    {
        if (Time.time > nextShootTime) {
            nextShootTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation, bullets);
            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.SetDamage(damage);
            audioSource.PlayOneShot(audioClip);
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
        }
    }
}

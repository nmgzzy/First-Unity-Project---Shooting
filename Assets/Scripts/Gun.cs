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
    public AudioClip audioClip;

    public void Shoot()
    {
        if (Time.time > nextShootTime) {
            nextShootTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.SetDamage(damage);
            audioSource.PlayOneShot(audioClip);
        }
    }
    
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // void Update()
    // {
        
    // }
}

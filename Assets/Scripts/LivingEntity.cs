using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100;
    protected float heath;
    protected bool dead = false;
    public event System.Action OnDeath;

    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        heath -= damage;
        if(heath <= 0 && !dead) {
            Die();
        }
    }

    public void Revive()
    {
        heath = startingHealth;
        dead = false;
    }

    protected void Die() 
    {
        dead = true;
        if (OnDeath != null) {
            OnDeath();
        }
        GameObject.Destroy(this.gameObject);
    }

    protected virtual void Start()
    {
        heath = startingHealth;
    }

    void Update()
    {
        
    }
}

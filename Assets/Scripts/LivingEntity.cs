using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100;
    protected float heath;
    protected bool dead = false;
    public event System.Action OnDeath;

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
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

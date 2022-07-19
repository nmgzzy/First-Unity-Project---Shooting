using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 5;
    protected float heath;
    protected bool dead = false;
    public event System.Action OnDeath;
    public void TakeHit(float damage, RaycastHit hit)
    {
        heath -= damage;
        if(heath <= 0 && !dead) {
            Die();
        }
    }
    protected void Die() 
    {
        dead = true;
        if (OnDeath != null) {
            OnDeath();
        }
        GameObject.Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        heath = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

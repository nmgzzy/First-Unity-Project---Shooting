using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10;
    public float maxLifeTime = 5;
    public LayerMask collisionMask;
    float damage = 5;
    float skinWidth = 0.1f;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    void Start()
    {
        Object.Destroy(this.gameObject, maxLifeTime);
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0) {
            OnHitObject(initialCollisions[0]);
        }
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        this.transform.Translate(Vector3.forward * moveDistance);
        CheckCollisions(moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        // print(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable> ();
		if (damageableObject != null) {
			damageableObject.TakeHit(this.damage, hit);
		}
        GameObject.Destroy(this.gameObject);
    }

    void OnHitObject(Collider c)
    {
        // print(hit.collider.gameObject.name);
        IDamageable damageableObject = c.GetComponent<IDamageable> ();
		if (damageableObject != null) {
			damageableObject.TakeDamage(this.damage);
		}
        GameObject.Destroy(this.gameObject);
    }
}

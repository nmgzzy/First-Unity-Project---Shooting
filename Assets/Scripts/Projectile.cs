using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10;
    public float maxLifeTime = 5;
    public LayerMask collisionMask;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Start()
    {
        Object.Destroy(this.gameObject, maxLifeTime);
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

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        // print(hit.collider.gameObject.name);
        GameObject.Destroy(this.gameObject);
        GameObject.Destroy(hit.collider.gameObject);
    }
}

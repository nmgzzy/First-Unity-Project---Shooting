using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10;
    public float maxLifeTime = 10;
    public void SetSpeed(float newSpeed){
        speed = newSpeed;
    }

    void Start()
    {
        Object.Destroy(this.gameObject, maxLifeTime);
    }

    void Update()
    {
        this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        Debug.Log(this.gameObject.name);
        Debug.Log(this.transform.position);
    }
}

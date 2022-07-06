using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigitbody;
    // Start is called before the first frame update
    void Start()
    {
        myRigitbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        myRigitbody.MovePosition(myRigitbody.position + velocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 _velocity) 
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 point)
    {
        Vector3 correctPoint = new Vector3(point.x, this.transform.position.y, point.z);
        this.transform.LookAt(correctPoint);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public float maxTorque = 5000f;
    public float motorForce = 5000f;
    public int currentNode;

    private List<Transform> nodes;
    private bool isBrake = false;
    private Transform brakePosition;
    private Rigidbody rb;
    private float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        wheelFL.motorTorque = motorForce;
        wheelFR.motorTorque = motorForce;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (nodes[currentNode].name == "brake")
        {
            isBrake = true;
            brakePosition = nodes[currentNode];
        }
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3)
        {
            currentNode++;
            // 如果index超出List長度
            currentNode = currentNode % nodes.Count;
        }

        ApplySteer();
        CheckSpeed();
        if (Vector3.Distance(transform.position, brakePosition.position) < 3)
        {
            SlowDown();
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void SlowDown()
    {
        rb.drag = 2;
    }

    private void CheckSpeed()
    {
        speed = rb.velocity.magnitude;
        if(speed > 15)
        {
            rb.drag = 1;
        }
        else
        {
            rb.drag = 0;
        }
    }
}

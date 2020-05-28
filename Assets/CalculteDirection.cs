using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculteDirection : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 destinationPoint;
    float angleDiff;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        destinationPoint = new Vector2(VROption.endPos.x, VROption.endPos.z);
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // Use default or distance
        RaycastHit hit = CreateRaycast();

        // If nothing is hit, set do default length
        float colliderDistance = hit.distance;
        float targetLength = colliderDistance;

        // Default
        Vector3 endPosition = transform.position + (transform.forward * targetLength);
        endPoint = new Vector2(endPosition.x, endPosition.z);
        startPoint = new Vector2(transform.position.x, transform.position.z);

        // Calculate angle
        angleDiff = Vector2.Angle(endPoint - startPoint, destinationPoint - startPoint);

        // Set linerenderer
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, endPosition);
        //lineRenderer.SetPosition(2, destinationPoint);
    }

    public float GetAngleDiff()
    {
        Debug.Log(angleDiff);
        return angleDiff;
    }

    private RaycastHit CreateRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, 100);

        return hit;
    }
}
